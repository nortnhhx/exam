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
using System.Windows.Navigation;
using System.Windows.Shapes;
using University.Models;

namespace University.Windows
{
    /// <summary>
    /// Логика взаимодействия для WorkloadPage.xaml
    /// </summary>
    public partial class WorkloadPage : Page
    {
        private UniversityEntities context;

        public WorkloadPage()
        {
            InitializeComponent();
            context = new UniversityEntities();

            if (SessionManager.Instance.IsGuest)
            {
                ButtonsPanel.Visibility = Visibility.Collapsed;
                GuestInfoText.Visibility = Visibility.Visible;
            }

            LoadWorkload();
        }

        private void LoadWorkload()
        {
            try
            {
                var workload = context.Workload
                    .Include("Teachers")
                    .Include("Subject")
                    .Include("Subject.Cycle")
                    .OrderBy(w => w.Workload_ID)
                    .ToList();

                WorkloadGrid.ItemsSource = workload;
                StatusText.Text = "Найдено записей: " + workload.Count;
            }
            catch (System.Exception ex)
            {
                StatusText.Text = "Ошибка: " + ex.Message;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Instance.IsGuest)
            {
                MessageBox.Show("Гость не может добавлять записи", "Ограничение");
                return;
            }

            var dialog = WorkloadEditDialog.ForCreate();
            if (dialog.ShowDialog() == true)
            {
                var workload = new Workload();

                if (dialog.WorkloadId.HasValue)
                    workload.Workload_ID = dialog.WorkloadId.Value;

                workload.Teacher_ID = dialog.TeacherId;
                workload.Subject_ID = dialog.SubjectId;

                context.Workload.Add(workload);
                context.SaveChanges();

                int newId = workload.Workload_ID;
                StatusText.Text = $"Нагрузка добавлена! ID: {newId}";
                LoadWorkload();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Instance.IsGuest)
            {
                MessageBox.Show("Гость не может редактировать записи", "Ограничение");
                return;
            }

            var workload = WorkloadGrid.SelectedItem as Workload;
            if (workload == null)
            {
                MessageBox.Show("Выберите запись");
                return;
            }

            var dialog = new WorkloadEditDialog(workload);
            if (dialog.ShowDialog() == true)
            {
                workload.Teacher_ID = dialog.TeacherId;
                workload.Subject_ID = dialog.SubjectId;

                context.SaveChanges();
                StatusText.Text = $"Нагрузка ID: {workload.Workload_ID} обновлена";
                LoadWorkload();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Instance.IsGuest)
            {
                MessageBox.Show("Гость не может удалять записи", "Ограничение");
                return;
            }

            var workload = WorkloadGrid.SelectedItem as Workload;
            if (workload == null)
            {
                MessageBox.Show("Выберите запись");
                return;
            }

            int id = workload.Workload_ID;
            string teacher = workload.Teachers?.FullName ?? "Неизвестно";
            string subject = workload.Subject?.SubjectName ?? "Неизвестно";

            if (MessageBox.Show($"Удалить нагрузку?\nID: {id}\nПреподаватель: {teacher}\nПредмет: {subject}",
                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                context.Workload.Remove(workload);
                context.SaveChanges();
                StatusText.Text = $"Нагрузка ID: {id} удалена";
                LoadWorkload();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadWorkload();
        }
    }
}
