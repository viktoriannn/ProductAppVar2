using Microsoft.EntityFrameworkCore;
using ProductDemo.Models;
using ProductDemo.Service;
using System.Windows;

namespace ProductDemo.View
{
    public partial class LoginWindow : Window
    {
        private ProductShoesContext _db = new ProductShoesContext();
        private MessageService _messageService = new MessageService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(loginTxt.Text) || string.IsNullOrEmpty(passwordTxt.Password))
            {
                _messageService.Error("Введите логин и пароль");
                return;
            }
            var user = _db.Users
                 .Include(u => u.IdRoleNavigation)
                 .FirstOrDefault(u => (u.Login == loginTxt.Text) && (u.Password == passwordTxt.Password));
            if (user == null)
            {
                _messageService.Error("Неверный логин или пароль");
                return;
            }
            new MainWindow(user).Show();
            Close();
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}
