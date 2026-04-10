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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;

            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text;

            PasswordBox.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = ShowPasswordCheckBox.IsChecked == true
                ? PasswordTextBox.Text
                : PasswordBox.Password;

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

            using (var context = new UniversityEntities())
            {
                var user = context.Authorization
                    .FirstOrDefault(u => u.Login == username && u.Password == password);

                if (user != null)
                {
                    SessionManager.Instance.StartSession(user);
                    new MainWindow().Show();
                    Close();
                }
                else
                {
                    MessageTextBlock.Text = "Неверный логин или пароль";
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().Show();
            Close();
        }
        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Instance.StartGuestSession();
            new MainWindow().Show();
            Close();
        }
    }
}
