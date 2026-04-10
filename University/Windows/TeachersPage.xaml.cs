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
    /// Логика взаимодействия для TeachersPage.xaml
    /// </summary>
    public partial class TeachersPage : Page
    {
        private UniversityEntities context;

        public TeachersPage()
        {
            InitializeComponent();
            context = new UniversityEntities();

            if (SessionManager.Instance.IsGuest)
            {
                ButtonsPanel.Visibility = Visibility.Collapsed;
                GuestInfoText.Visibility = Visibility.Visible;
            }

            LoadTeachers();
        }

        private void LoadTeachers()
        {
            try
            {
                var teachers = context.Teachers
                    .Include("Category")
                    .OrderBy(t => t.Teacher_ID)
                    .ToList();

                TeachersGrid.ItemsSource = teachers;
                StatusText.Text = "Найдено: " + teachers.Count;
            }
            catch (System.Exception ex)
            {
                StatusText.Text = "Ошибка: " + ex.Message;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = TeacherEditDialog.ForCreate();
            if (dialog.ShowDialog() == true)
            {
                var newTeacher = new Teachers();


                if (dialog.TeacherId.HasValue)
                    newTeacher.Teacher_ID = dialog.TeacherId.Value;

                newTeacher.FullName = dialog.FullName;
                newTeacher.Street = dialog.Street;
                newTeacher.House = dialog.House;
                newTeacher.Apartament = dialog.Apartment;
                newTeacher.Category_ID = dialog.CategoryId;


                context.Teachers.Add(newTeacher);
                context.SaveChanges();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Instance.IsGuest)
            {
                MessageBox.Show("Гость не может редактировать записи", "Ограничение");
                return;
            }

            var teacher = TeachersGrid.SelectedItem as Teachers;
            if (teacher == null)
            {
                MessageBox.Show("Выберите преподавателя");
                return;
            }

            var dialog = new TeacherEditDialog(teacher);
            if (dialog.ShowDialog() == true)
            {
                teacher.FullName = dialog.FullName;
                teacher.Street = dialog.Street;
                teacher.House = dialog.House;
                teacher.Apartament = dialog.Apartment;
                teacher.Category_ID = dialog.CategoryId;

                context.SaveChanges();
                StatusText.Text = $"Преподаватель ID: {teacher.Teacher_ID} обновлен";
                LoadTeachers();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Instance.IsGuest)
            {
                MessageBox.Show("Гость не может удалять записи", "Ограничение");
                return;
            }

            var teacher = TeachersGrid.SelectedItem as Teachers;
            if (teacher == null)
            {
                MessageBox.Show("Выберите преподавателя");
                return;
            }

            int id = teacher.Teacher_ID;
            string name = teacher.FullName;

            if (MessageBox.Show($"Удалить преподавателя?\nID: {id}\nФИО: {name}",
                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                context.Teachers.Remove(teacher);
                context.SaveChanges();
                StatusText.Text = $"Преподаватель ID: {id} удален";
                LoadTeachers();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTeachers();
        }
    }
}
