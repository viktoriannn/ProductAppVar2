using Microsoft.EntityFrameworkCore;
using ProductDemo.Models;
using ProductDemo.Service;
using ProductDemo.View;
using ProductDemo.ViewModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
namespace ProductDemo
{
    public partial class ProductsWindow : Window
    {
        private ProductShoesContext _db = new ProductShoesContext();
        private List<ProductViewModel> _productvm = new List<ProductViewModel>();
        private MessageService _messageService = new MessageService();
        private ImageService _imageService = new ImageService();
        private bool _isAdmin;
        private User _user = new User();
        private string[] _sortTypes = new string[]
       {
            "По умолчанию", "По убыванию", "По возрастанию"
       };
        private List<string> _filterTypes = new List<string>()
        {
            "Все поставщики"
        };
        public ProductsWindow(User? user = null)
        {
            InitializeComponent();
            userNameTxt.Text = user != null ? $"{user.Surname} {user.Name} {user.Patronymic}" : "Гость";
            _user = user;
            LoadRole();
            LoadProduct();
            LoadData();
        }
        private void LoadRole()
        {
            string? role = _user?.IdRoleNavigation?.Role1;
            bool canManage = role == "Администратор" || role == "Менеджер";
            _isAdmin = role == "Администратор";
            panelFind.Visibility = canManage ? Visibility.Visible : Visibility.Collapsed;
            paneAddDel.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            OrderOutBtn.Visibility = canManage ? Visibility.Visible : Visibility.Collapsed;
        }
        private void LoadProduct()
        {
            try
            {
                var products = _db.ProductLists
                 .AsNoTracking()
                 .Include(p => p.IdProductTypeNavigation)
                 .Include(p => p.IdCategoryNavigation)
                 .Include(p => p.IdUnitNavigation)
                 .Include(p => p.IdProductionNavigation)
                 .Include(p => p.IdProviderNavigation)
                 .ToList();
                _productvm = products.Select(p => new ProductViewModel
                {
                    Id = p.IdProduct,
                    ProductType = p.IdProductTypeNavigation.ProductType1,
                    Image = p.Image,
                    Category = p.IdCategoryNavigation.Category1,
                    Description = p.Description,
                    Price = p.Price,
                    Production = p.IdProductionNavigation.Production1,
                    Provider = p.IdProviderNavigation.Provider1,
                    Unit = p.IdUnitNavigation.Unit1,
                    Count = p.Count,
                    Discount = p.Discount,
                    CanDelete = _isAdmin
                }).ToList();
                productList.ItemsSource = _productvm;
            }
            catch (Exception ex)
            {
                _messageService.Error("Не удалось подключиться к БД"+ ex.Message);
            }
        }
        public void LoadData()
        {
            try
            {
                SortBox.ItemsSource = _sortTypes;
                _filterTypes = new List<string>() { "Все поставщики" };
                _filterTypes.AddRange(_db.Providers.AsNoTracking().Select(c => c.Provider1).ToList());
                FilterBox.ItemsSource = _filterTypes;
                FilterBox.SelectedIndex = 0;
                SortBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                _messageService.Error("Не удалось загрузить список поставщиков.\n" + ex.Message);
            }
        }
        private void RefreshProducts()
        {
            string? provider = FilterBox.SelectedItem?.ToString();
            int sort = SortBox.SelectedIndex;
            LoadProduct();
            LoadData();
            if (provider != null && _filterTypes.Contains(provider))
                FilterBox.SelectedItem = provider;
            SortBox.SelectedIndex = sort < 0 ? 0 : sort;
            FindSortFilter();
        }
        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_messageService.Question("Вы точно хотите выйти?"))
                return;
            new LoginWindow().Show();
            Close();
        }
        private void FindSortFilter()
        {
            IEnumerable<ProductViewModel> items = _productvm;

            string search = FindTxt.Text.Trim();

            if (search.Length > 0)
            {
                items = items.Where(p =>
                    (p.ProductType != null && p.ProductType.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Category != null && p.Category.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Production != null && p.Production.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Provider != null && p.Provider.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Unit != null && p.Unit.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (FilterBox.SelectedIndex > 0)
            {
                string provider = FilterBox.SelectedItem.ToString();
                items = items
                    .Where(p => p.Provider == provider);
            }

            if (SortBox.SelectedIndex == 1)
                items = items
                    .OrderByDescending(p => p.Count);
            else if (SortBox.SelectedIndex == 2)
                items = items
                    .OrderBy(p => p.Count);

            var result = items.ToList();
            productList.ItemsSource = result;
            resultTxt.Visibility = result.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OpenEditor(ProductViewModel? product)
        {
            var window = new AddEditProdWindow(product) { Owner = this };
            if (window.ShowDialog() == true)
                RefreshProducts();
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenEditor(null);
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
                return;
            Button button = (Button)sender;
            ProductViewModel product = (ProductViewModel)button.DataContext;
            try
            {
                if (_db.OrderItems.Any(o => o.IdProduct == product.Id))
                {
                    _messageService.Warning($"Товар «{product.ProductType}» нельзя удалить, тк он присутствует в заказе");
                    return;
                }
                if (!_messageService.Question($"Удалить товар «{product.ProductType}»? Это действие нельзя отменить."))
                    return;
                var entity = _db.ProductLists.Find(product.Id);
                if (entity != null)
                {
                    string? image = entity.Image;
                    _db.ProductLists.Remove(entity);
                    _db.SaveChanges();
                    _imageService.DeleteImage(image);
                }
                RefreshProducts();
            }
            catch (Exception ex)
            {
                _messageService.Error("Не удалось удалить товар"+ ex.Message);
            }
        }

        private void ProductCard_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_isAdmin)
                return;
            if (IsInsideButton(e.OriginalSource as DependencyObject))
                return;
            Border card = (Border)sender;
            ProductViewModel product = (ProductViewModel)card.DataContext;
            OpenEditor(product);
        }
        private static bool IsInsideButton(DependencyObject? element)
        {
            while (element != null)
            {
                if (element is Button)
                    return true;
                element = element is System.Windows.Media.Visual
                    ? System.Windows.Media.VisualTreeHelper.GetParent(element)
                    : LogicalTreeHelper.GetParent(element);
            }
            return false;
        }
        private void FilterBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FindSortFilter();
        }

        private void SortBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FindSortFilter();
        }

        private void FindTxt_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FindSortFilter();
        }

        private void OrderOutBtn_Click(object sender, RoutedEventArgs e)
        {
            new OrdersWindow(_user).Show();
            Close();
        }
    }
}