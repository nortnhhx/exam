using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace University.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadUserInfo();
            TeachersButton_Click(null, null);
        }

        private void LoadUserInfo()
        {
            var session = SessionManager.Instance;

            if (session.IsGuest)
            {
                UserInfoTextBlock.Text = "Гостевой режим";
                GuestLabel.Visibility = Visibility.Visible;
            }
            else if (session.IsAuthenticated)
            {
                UserInfoTextBlock.Text = "Пользователь: " + session.CurrentUser.Login;
                GuestLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void TeachersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TeachersPage());
        }

        private void WorkloadButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WorkloadPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Instance.EndSession();
            new LoginWindow().Show();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SessionManager.Instance.EndSession();
        }
    }
}
