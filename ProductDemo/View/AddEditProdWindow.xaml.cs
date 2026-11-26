
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ProductDemo.Models;
using ProductDemo.Service;
using ProductDemo.ViewModel;
using System.Globalization;
using System.Text;
using System.Windows;

namespace ProductDemo.View
{
    public partial class AddEditProdWindow : Window
    {
        private ProductShoesContext _db = new ProductShoesContext();
        private bool _isEdit;
        private ProductList _productList = new ProductList();
        private MessageService _messageService = new MessageService();
        private ImageService _imageService = new ImageService();
        private int? _id;
        private string? _newImagePath;

        public AddEditProdWindow(ProductViewModel? product)
        {
            InitializeComponent();
            if (product != null)
            {
                _isEdit = true;
                _id = product.Id;

                _productList = _db.ProductLists
                    .Include(p => p.IdProductTypeNavigation)
                    .Include(p => p.IdCategoryNavigation)
                    .Include(p => p.IdProductionNavigation)
                    .Include(p => p.IdProviderNavigation)
                    .Include(p => p.IdUnitNavigation)
                    .FirstOrDefault(p => p.IdProduct == _id);

                if (_productList == null)
                {
                    _messageService.Error("Товар не найден");
                    Close();
                    return;
                }
            }

            LoadData();
        }

        private void LoadData()
        {
            ProductionTxt.ItemsSource = _db.Productions.ToList();
            ProductionTxt.DisplayMemberPath = "Production1";
            ProductionTxt.SelectedValuePath = "IdProduction";

            CategoryTxt.ItemsSource = _db.Categories.ToList();
            CategoryTxt.DisplayMemberPath = "Category1";
            CategoryTxt.SelectedValuePath = "IdCategory";

            if (_isEdit)
                LoadProduct();
            else
            {
                IdPanel.Visibility = Visibility.Collapsed;
                CountTxt.Text = "0";
                DiscountTxt.Text = "0";
                PhotoImg.Source = _imageService.LoadImage(null);
            }
        }

        private void LoadProduct()
        {
            IdTxt.Text = _productList.IdProduct.ToString();
            ArticleTxt.Text = _productList.Article;
            PhotoImg.Source = _imageService.LoadImage(_productList.Image);
            ProductTypeTxt.Text = _productList.IdProductTypeNavigation?.ProductType1;
            DescriptionTxt.Text = _productList.Description;
            ProviderTxt.Text = _productList.IdProviderNavigation?.Provider1;
            PriceTxt.Text = _productList.Price.ToString("0.##");
            UnitTxt.Text = _productList.IdUnitNavigation?.Unit1;
            CountTxt.Text = _productList.Count.ToString();
            DiscountTxt.Text = (_productList.Discount ?? 0).ToString("0");

            ProductionTxt.SelectedValue = _productList.IdProduction;
            CategoryTxt.SelectedValue = _productList.IdCategory;
        }

        private void PhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                if (!_imageService.CheckSize(dialog.FileName))
                {
                    _messageService.Error("Максимальный размер фото: 300×200.");
                    return;
                }

                _newImagePath = dialog.FileName;
                PhotoImg.Source = _imageService.LoadImage(dialog.FileName);
            }
        }

        private void SaveRrodBtn_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            (string Value, string Message)[] textBox =
            [
                (ArticleTxt.Text, "Введите артикул"),
                (ProductTypeTxt.Text, "Введите наименование"),
                (DescriptionTxt.Text, "Введите описание"),
                (ProviderTxt.Text, "Введите поставщика"),
                (UnitTxt.Text, "Введите единицу измерения")
            ];

            foreach (var (value, message) in textBox)
            {
                if (string.IsNullOrWhiteSpace(value))
                    errors.AppendLine(message);
            }

            if (CategoryTxt.SelectedValue == null)
                errors.AppendLine("Выберите категорию");

            if (ProductionTxt.SelectedValue == null)
                errors.AppendLine("Выберите производителя");

            string priceText = PriceTxt.Text.Trim().Replace(',', '.');

            if (!decimal.TryParse(priceText, NumberStyles.Any,
                CultureInfo.InvariantCulture, out decimal price) || price < 0)
                errors.AppendLine("Цена — неотрицательное число");

            if (!int.TryParse(CountTxt.Text.Trim(), out int count) || count < 0)
                errors.AppendLine("Количество — целое неотрицательное число");

            if (!decimal.TryParse(DiscountTxt.Text.Trim(), out decimal discount) ||
                discount < 0 || discount > 99)
                errors.AppendLine("Скидка — число от 0 до 99");

            if (errors.Length > 0)
            {
                _messageService.Error("Исправьте:\n" + errors);
                return;
            }

            string? oldImage = _productList.Image;
            string? newImage = null;

            try
            {
                if (_newImagePath != null)
                    newImage = _imageService.SaveImage(_newImagePath);

                if (_isEdit)
                    UpdateProduct(price, count, discount, newImage);
                else
                    CreateProduct(price, count, discount, newImage);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _imageService.DeleteImage(newImage);
                _messageService.Error("Не удалось сохранить товар в БД" + ex.Message);
                return;
            }

            if (newImage != null)
                _imageService.DeleteImage(oldImage);

            DialogResult = true;
        }

        private void CreateProduct(decimal price, int count, decimal discount, string? image)
        {
            _productList.IdProduct =
                (_db.ProductLists.Max(p => (int?)p.IdProduct) ?? 0) + 1;

            FillProduct(price, count, discount, image);
            _db.ProductLists.Add(_productList);
        }

        private void UpdateProduct(decimal price, int count, decimal discount, string? image)
        {
            FillProduct(price, count, discount, image);
        }

        private void FillProduct(decimal price, int count, decimal discount, string? image)
        {
            _productList.Article = ArticleTxt.Text.Trim();
            _productList.IdProductType = GetProductTypeId(ProductTypeTxt.Text.Trim());
            _productList.IdCategory = (int)CategoryTxt.SelectedValue;
            _productList.Description = DescriptionTxt.Text.Trim();
            _productList.IdProduction = (int)ProductionTxt.SelectedValue;
            _productList.IdProvider = GetProviderId(ProviderTxt.Text.Trim());
            _productList.Price = price;
            _productList.IdUnit = GetUnitId(UnitTxt.Text.Trim());
            _productList.Count = count;
            _productList.Discount = discount;

            if (image != null)
                _productList.Image = image;
        }

        private int GetProductTypeId(string name)
        {
            var type = _db.ProductTypes.FirstOrDefault(t => t.ProductType1 == name);

            if (type != null)
                return type.IdProductType;

            int id = (_db.ProductTypes.Max(t => (int?)t.IdProductType) ?? 0) + 1;

            _db.ProductTypes.Add(new ProductType
            {
                IdProductType = id,
                ProductType1 = name
            });

            return id;
        }

        private int GetProviderId(string name)
        {
            var provider = _db.Providers.FirstOrDefault(p => p.Provider1 == name);

            if (provider != null)
                return provider.IdProvider;

            int id = (_db.Providers.Max(p => (int?)p.IdProvider) ?? 0) + 1;

            _db.Providers.Add(new Provider
            {
                IdProvider = id,
                Provider1 = name
            });

            return id;
        }

        private int GetUnitId(string name)
        {
            var unit = _db.Units.FirstOrDefault(u => u.Unit1 == name);

            if (unit != null)
                return unit.IdUnit;

            int id = (_db.Units.Max(u => (int?)u.IdUnit) ?? 0) + 1;

            _db.Units.Add(new Unit
            {
                IdUnit = id,
                Unit1 = name
            });

            return id;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_messageService.Question(
                "Введённые данные не будут сохранены. Вернуться к списку товаров?"))
            {
                Close();
            }
        }
    }
}
