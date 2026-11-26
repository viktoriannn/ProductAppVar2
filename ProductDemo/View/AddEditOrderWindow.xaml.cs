using Microsoft.EntityFrameworkCore;
using ProductDemo.Models;
using ProductDemo.Service;
using ProductDemo.ViewModel;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ListItem = ProductDemo.ViewModel.ListItem;

namespace ProductDemo.View
{
    public partial class AddEditOrderWindow : Window
    {
        private ProductShoesContext _db = new ProductShoesContext();
        private bool _isEdit;
        private Order _order = new Order();
        private MessageService _messageService = new MessageService();
        private int? _id;
        private int _code;
        private List<ProductList> _products = new List<ProductList>();
        private ObservableCollection<OrderItemViewModel> _items = new ObservableCollection<OrderItemViewModel>();

        public AddEditOrderWindow(OrderViewModel? order)
        {
            InitializeComponent();
            if (order == null)
            {
                _isEdit = false;
            }
            else
            {
                _isEdit = true;
                _id = order.Id;
            }
        }
        //обновление данных при загрузке окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                _messageService.Error("Не удалось загрузить данные из БД" + ex.Message);
                Close();
            }
        }

        private void LoadData()
        {
            var users = _db.Users.OrderBy(u => u.Surname).ToList();
            UserBox.ItemsSource = users.Select(u => new ListItem
            {
                Id = u.IdUser,
                Name = $"{u.Surname} {u.Name} {u.Patronymic}"
            }).ToList();

            var pvzs = _db.Pvzs.Include(p => p.IdCityNavigation).OrderBy(p => p.IdPvz).ToList();
            PvzBox.ItemsSource = pvzs.Select(p => new ListItem
            {
                Id = p.IdPvz,
                Name = $"{p.Index}, г. {p.IdCityNavigation.City1}, ул. {p.Street}, {p.HumberHouse}"
            }).ToList();

            var statuses = _db.Orders.Select(o => o.Status).Distinct().ToList();
            if (!statuses.Contains("Новый"))
                statuses.Add("Новый");
            if (!statuses.Contains("Завершен"))
                statuses.Add("Завершен");
            StatusBox.ItemsSource = statuses;

            _products = _db.ProductLists
                .Include(p => p.IdProductTypeNavigation)
                .OrderBy(p => p.IdProduct)
                .ToList();
            ProductBox.ItemsSource = _products.Select(p => new ListItem
            {
                Id = p.IdProduct,
                Name = $"{p.IdProductTypeNavigation.ProductType1} (арт. {p.Article}), в наличии: {p.Count}"
            }).ToList();

            ItemsGrid.ItemsSource = _items;

            if (_isEdit)
                LoadOrder();
            else
            {
                IdPanel.Visibility = Visibility.Collapsed;
                _code = (_db.Orders.Max(o => (int?)o.Code) ?? 900) + 1;
                CodeTxt.Text = _code.ToString();
                OrderDatePick.SelectedDate = DateTime.Today;
                DeliveryDatePick.SelectedDate = DateTime.Today;
                StatusBox.SelectedItem = "Новый";
                UpdateTotal();
            }
        }

        private void LoadOrder()
        {
            var order = _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.IdProductNavigation)
                .ThenInclude(p => p.IdProductTypeNavigation)
                .FirstOrDefault(o => o.IdOrder == _id);

            if (order == null)
            {
                _messageService.Error("Заказ не найден в БД");
                Close();
                return;
            }

            _order = order;
            _code = _order.Code;
            IdTxt.Text = _order.IdOrder.ToString();
            CodeTxt.Text = _code.ToString();
            OrderDatePick.SelectedDate = _order.DataOrder.ToDateTime(TimeOnly.MinValue);
            DeliveryDatePick.SelectedDate = _order.DataDelivery.ToDateTime(TimeOnly.MinValue);
            UserBox.SelectedValue = _order.IdUser;
            PvzBox.SelectedValue = _order.IdPvz;
            StatusBox.SelectedItem = _order.Status;

            foreach (var item in _order.OrderItems)
            {
                _items.Add(new OrderItemViewModel
                {
                    IdProduct = item.IdProduct,
                    Name = item.IdProductNavigation.IdProductTypeNavigation.ProductType1,
                    Price = item.IdProductNavigation.Price,
                    Discount = item.IdProductNavigation.Discount,
                    Count = item.Count
                });
            }

            UpdateTotal();
        }

        private void AddItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedValue == null)
            {
                _messageService.Warning("Выберите товар");
                return;
            }
            if (!int.TryParse(ItemCountTxt.Text.Trim(), out int count) || count <= 0)
            {
                _messageService.Error("Количество - целое неотрицательное число");
                return;
            }

            int idProduct = (int)ProductBox.SelectedValue;
            ProductList product = _products.First(p => p.IdProduct == idProduct);
            OrderItemViewModel? line = _items.FirstOrDefault(i => i.IdProduct == idProduct);
            int countOrder = line != null ? line.Count : 0;

            if (countOrder + count > product.Count)
            {
                _messageService.Warning($"Недостаточно товара на складе: в наличии {product.Count}");
                return;
            }

            if (line != null)
            {
                line.Count += count;
                ItemsGrid.Items.Refresh();
            }
            else
            {
                _items.Add(new OrderItemViewModel
                {
                    IdProduct = product.IdProduct,
                    Name = product.IdProductTypeNavigation.ProductType1,
                    Price = product.Price,
                    Discount = product.Discount,
                    Count = count
                });
            }

            ItemCountTxt.Text = "1";
            UpdateTotal();
        }

        private void DelItemBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            OrderItemViewModel line = (OrderItemViewModel)button.DataContext;
            _items.Remove(line);
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal fullSum = _items.Sum(i => i.FullSum);
            decimal total = _items.Sum(i => i.Sum);
            TotalTxt.Text = $"Итого: {total:N2} руб. (скидка: {fullSum - total:N2} руб.)";
        }

        private void SaveOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            if (OrderDatePick.SelectedDate == null)
                errors.AppendLine("Укажите дату заказа");
            if (DeliveryDatePick.SelectedDate == null)
                errors.AppendLine("Укажите дату доставки");
            if (OrderDatePick.SelectedDate != null && DeliveryDatePick.SelectedDate != null
                && DeliveryDatePick.SelectedDate < OrderDatePick.SelectedDate)
                errors.AppendLine("Дата доставки не может быть раньше даты заказа");
            if (UserBox.SelectedValue == null)
                errors.AppendLine("Выберите клиента");
            if (PvzBox.SelectedValue == null)
                errors.AppendLine("Выберите пункт выдачи");
            if (StatusBox.SelectedItem == null)
                errors.AppendLine("Выберите статус заказа");
            if (_items.Count == 0)
                errors.AppendLine("Добавьте хотя бы один товар");

            if (errors.Length > 0)
            {
                _messageService.Error("Исправьте:\n" + errors);
                return;
            }

            try
            {
                if (_isEdit)
                    UpdateOrder();
                else
                    CreateOrder();

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _messageService.Error("Не удалось сохранить заказ в БД " + ex.Message);
                return;
            }

            DialogResult = true;
        }

        private void CreateOrder()
        {
            _order.IdOrder = (_db.Orders.Max(o => (int?)o.IdOrder) ?? 0) + 1;
            _order.Code = _code;
            FillOrder();
            _db.Orders.Add(_order);
        }

        private void UpdateOrder()
        {
            FillOrder();
        }

        private void FillOrder()
        {
            _order.DataOrder = DateOnly.FromDateTime(OrderDatePick.SelectedDate!.Value);
            _order.DataDelivery = DateOnly.FromDateTime(DeliveryDatePick.SelectedDate!.Value);
            _order.IdUser = (int)UserBox.SelectedValue;
            _order.IdPvz = (int)PvzBox.SelectedValue;
            _order.Status = StatusBox.SelectedItem.ToString()!;

            int itemId = (_db.OrderItems.Max(i => (int?)i.IdOrderItem) ?? 0) + 1;
            //удаление старых позиций заказа и добавление обновлённых
            _db.OrderItems.RemoveRange(_order.OrderItems.ToList());
            foreach (var line in _items)
            {
                _order.OrderItems.Add(new OrderItem
                {
                    IdOrderItem = itemId,
                    IdProduct = line.IdProduct,
                    Count = line.Count
                });
                itemId++;
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_messageService.Question("Введённые данные не будут сохранены. Вернуться к списку заказов?"))
                Close();
        }
    }
}