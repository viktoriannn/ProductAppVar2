using Microsoft.EntityFrameworkCore;
using ProductDemo.Models;
using ProductDemo.Service;
using ProductDemo.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace ProductDemo.View
{
    public partial class OrdersWindow : Window
    {
        private ProductShoesContext _db = new ProductShoesContext();
        private List<OrderViewModel> _ordervm = new List<OrderViewModel>();
        private MessageService _messageService = new MessageService();
        private DiscountService _discountService = new DiscountService();
        private User? _user;
        private bool _isAdmin;
        public OrdersWindow(User? user)
        {
            InitializeComponent();
            _user = user;
            userNameTxt.Text = user != null ? $"{user.Surname} {user.Name} {user.Patronymic}" : "Гость";
            _isAdmin = user?.IdRoleNavigation?.Role1 == "Администратор";
            paneAddDel.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            LoadOrders();
        }
        private void LoadOrders()
        {
            try
            {
                var orders = _db.Orders
                    .AsNoTracking()
                    .Include(o => o.IdPvzNavigation)
                    .ThenInclude(p => p.IdCityNavigation)
                    .Include(o => o.IdUserNavigation)
                    .Include(o => o.OrderItems)
                    .ThenInclude(i => i.IdProductNavigation)
                    .ThenInclude(p => p.IdProductTypeNavigation)
                    .Include(o => o.OrderItems)
                    .ThenInclude(i => i.IdProductNavigation)
                    .ThenInclude(p => p.IdUnitNavigation)
                    .OrderBy(o => o.IdOrder)
                    .ToList();
                _ordervm = orders.Select(o => new OrderViewModel
                {
                    Id = o.IdOrder,
                    Code = o.Code,
                    Status = o.Status,
                    DataOrder = o.DataOrder,
                    DataDelivery = o.DataDelivery,
                    Pvz = $"{o.IdPvzNavigation.Index}, г. {o.IdPvzNavigation.IdCityNavigation.City1}, ул. {o.IdPvzNavigation.Street}, {o.IdPvzNavigation.HumberHouse}",
                    Client = $"{o.IdUserNavigation.Surname} {o.IdUserNavigation.Name} {o.IdUserNavigation.Patronymic}",
                    Composition = string.Join("; ", o.OrderItems.Select(i => $"{i.IdProductNavigation.IdProductTypeNavigation.ProductType1} {i.Count} {i.IdProductNavigation.IdUnitNavigation.Unit1}")),
                    Total = o.OrderItems.Sum(i => _discountService.GetFinalPrice(i.IdProductNavigation.Price, i.IdProductNavigation.Discount) * i.Count),
                    CanDelete = _isAdmin
                }).ToList();
                orderList.ItemsSource = _ordervm;
            }
            catch (Exception ex)
            {
                _messageService.Error("Не удалось загрузить заказы из БД" + ex.Message);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            new ProductsWindow(_user).Show();
            Close();
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_messageService.Question("Вы точно хотите выйти?"))
                return;
            new LoginWindow().Show();
            Close();
        }

        private void OpenEditor(OrderViewModel? order)
        {
            var window = new AddEditOrderWindow(order) { Owner = this };
            if (window.ShowDialog() == true)
                LoadOrders();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenEditor(null);
        }

        private void OrderCard_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_isAdmin)
                return;
            Border card = (Border)sender;
            OrderViewModel order = (OrderViewModel)card.DataContext;
            OpenEditor(order);
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
                return;
            Button button = (Button)sender;
            OrderViewModel order = (OrderViewModel)button.DataContext;
            if (!_messageService.Question($"Удалить заказ №{order.Id}?"))
                return;
            try
            {
                var orders = _db.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.IdOrder == order.Id);
                if (orders != null)
                {
                    _db.OrderItems.RemoveRange(orders.OrderItems.ToList());
                    _db.Orders.Remove(orders);
                    _db.SaveChanges();
                }
                LoadOrders();
            }
            catch (Exception ex)
            {
                _messageService.Error("Не удалось удалить заказ\n" + ex.Message);
            }
        }
    }
}