using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using University.Models;

namespace University.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private bool ValidatePassword(string password)
        {
            if (password.Length < 8) return false;
            bool hasUpper = Regex.IsMatch(password, "[A-Z]");
            bool hasLower = Regex.IsMatch(password, "[a-z]");
            bool hasDigit = Regex.IsMatch(password, "[0-9]");
            bool hasSpecial = Regex.IsMatch(password, "[!@#$%^&*]");
            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageTextBlock.Text = "Заполните все поля";
                return;
            }

            if (!ValidatePassword(password))
            {
                MessageTextBlock.Text = "Пароль не соответствует требованиям";
                return;
            }

            if (password != confirmPassword)
            {
                MessageTextBlock.Text = "Пароли не совпадают";
                return;
            }

            using (var context = new UniversityEntities())
            {
                if (context.Authorization.Any(u => u.Login == username))
                {
                    MessageTextBlock.Text = "Логин уже занят";
                    return;
                }

                var user = new Authorization
                {
                    Login = username,
                    Password = password
                };

                context.Authorization.Add(user);
                context.SaveChanges();

                MessageBox.Show("Регистрация успешна");
                new LoginWindow().Show();
                Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

    }
}
