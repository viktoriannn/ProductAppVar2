using System.Windows;
using Microsoft.EntityFrameworkCore;
using ProductDemo.Models;
using ProductDemo.Service;
using ProductDemo.View;
using ProductDemo.ViewModel;
namespace ProductDemo
{
    public partial class MainWindow : Window
    {
        private ProductShoesContext _db = new ProductShoesContext();
        private List<ProductViewModel> _productvm = new List<ProductViewModel>();
        private MessageService _messageService = new MessageService();
        private User _user = new User();
        private string[] _sortTypes = new string[]
       {
            "По умолчанию", "По убыванию", "По возрастанию"
       };
        private List<string> _filterTypes = new List<string>()
        {
            "Все поставщики"
        };
        public MainWindow(User? user = null)
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
            bool canSearch = role == "Администратор" || role == "Менеджер";
            bool isAdmin = role == "Администратор";
            panelFind.Visibility = canSearch ? Visibility.Visible : Visibility.Collapsed;
            paneAddDel.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }
        private void LoadProduct()
        {
            try
            {
                var products = _db.ProductLists
                 .AsTracking()
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
                    Image = string.IsNullOrWhiteSpace(p.Image) ? "/Res/picture.png" : "/Res/" + p.Image.Trim(),
                    Category = p.IdCategoryNavigation.Category1,
                    Description = p.Description,
                    Price = p.Price,
                    Production = p.IdProductionNavigation.Production1,
                    Provider = p.IdProviderNavigation.Provider1,
                    Unit = p.IdUnitNavigation.Unit1,
                    Count = p.Count,
                    Discount = p.Discount
                }).ToList();
                productList.ItemsSource = _productvm;
            }
            catch (Exception ex)
            {
                _messageService.Error("Не уддвлось подключиться к БД" + ex.Message);
            }
        }
        public void LoadData()
        {
            sortBox.ItemsSource = _sortTypes;
            var providers = _db.Providers.Select(c => c.Provider1).ToList();
            foreach (var provider in providers)
            {
                _filterTypes.Add(provider);
                filterBox.ItemsSource = _filterTypes;
            }
        }
        private void logOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_messageService.Question("Вы точно хотите выйти?"))
                return;
            new LoginWindow().Show();
            Close();
        }
        private void FindSortFilter()
        {
            IEnumerable<ProductViewModel> items = _productvm;

            string search = findTxt.Text.Trim();

            if (search.Length > 0)
            {
                items = items.Where( p =>
                    (p.ProductType != null && p.ProductType.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Category != null && p.Category.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Production != null && p.Production.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Provider != null && p.Provider.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (filterBox.SelectedIndex > 0)
            {
                string provider = filterBox.SelectedItem.ToString();
                items = items
                    .Where(p => p.Provider == provider);
            }

            if (sortBox.SelectedIndex == 1)
                items = items
                    .OrderByDescending(p => p.Count);
            else if (sortBox.SelectedIndex == 2)
                items = items
                    .OrderBy(p => p.Count);

            var result = items.ToList();
            productList.ItemsSource = result;
            resultTxt.Visibility = result.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void productCard_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void filterBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FindSortFilter();
        }

        private void sortBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FindSortFilter();
        }

        private void findTxt_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FindSortFilter();
        }
    }
}