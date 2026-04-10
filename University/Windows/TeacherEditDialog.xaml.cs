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
using University.Models;

namespace University.Windows
{
    /// <summary>
    /// Логика взаимодействия для TeacherEditDialog.xaml
    /// </summary>
    public partial class TeacherEditDialog : Window
    {
        public int? TeacherId { get; private set; }
        public string FullName { get; private set; }
        public string Street { get; private set; }
        public string House { get; private set; }
        public string Apartment { get; private set; }
        public int? CategoryId { get; private set; }
        public bool IsNewTeacher { get; private set; }

        private UniversityEntities context;
        private int teacherId;
        private bool isEditMode;

        public TeacherEditDialog() : this(false)
        {
        }

        private TeacherEditDialog(bool isEdit)
        {
            InitializeComponent();
            context = new UniversityEntities();
            LoadCategories();
            isEditMode = isEdit;

            if (SessionManager.Instance.IsGuest)
            {
                Close();
            }
        }


        public static TeacherEditDialog ForCreate()
        {
            var dialog = new TeacherEditDialog(false);
            dialog.Title = "Добавление преподавателя";
            dialog.IsNewTeacher = true;


            dialog.IdLabel.Visibility = Visibility.Collapsed;
            dialog.IdTextBox.Visibility = Visibility.Collapsed;
            dialog.ManualIdLabel.Visibility = Visibility.Visible;
            dialog.ManualIdTextBox.Visibility = Visibility.Visible;

            return dialog;
        }


        public TeacherEditDialog(Teachers teacher) : this(true)
        {
            if (SessionManager.Instance.IsGuest)
                return;

            Title = "Редактирование преподавателя";
            teacherId = teacher.Teacher_ID;
            IsNewTeacher = false;


            IdLabel.Visibility = Visibility.Visible;
            IdTextBox.Visibility = Visibility.Visible;
            IdTextBox.Text = teacher.Teacher_ID.ToString();
            IdTextBox.IsEnabled = false;

            ManualIdLabel.Visibility = Visibility.Collapsed;
            ManualIdTextBox.Visibility = Visibility.Collapsed;

            FullNameTextBox.Text = teacher.FullName;
            StreetTextBox.Text = teacher.Street;
            HouseTextBox.Text = teacher.House;
            ApartmentTextBox.Text = teacher.Apartament;

            if (teacher.Category_ID.HasValue)
            {
                var cat = context.Category.Find(teacher.Category_ID.Value);
                if (cat != null)
                    CategoryComboBox.SelectedItem = cat;
            }
        }

        private void LoadCategories()
        {
            var categories = context.Category.ToList();
            CategoryComboBox.ItemsSource = categories;
            if (categories.Any())
                CategoryComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Instance.IsGuest)
            {
                MessageBox.Show("Гость не может сохранять изменения", "Ограничение");
                return;
            }


            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                ErrorText.Text = "Введите ФИО преподавателя";
                return;
            }

            if (string.IsNullOrWhiteSpace(StreetTextBox.Text))
            {
                ErrorText.Text = "Введите улицу";
                return;
            }

            if (string.IsNullOrWhiteSpace(HouseTextBox.Text))
            {
                ErrorText.Text = "Введите номер дома";
                return;
            }


            if (IsNewTeacher && !string.IsNullOrWhiteSpace(ManualIdTextBox.Text))
            {
                if (!int.TryParse(ManualIdTextBox.Text.Trim(), out int manualId))
                {
                    ErrorText.Text = "ID должен быть числом";
                    return;
                }


                if (context.Teachers.Any(t => t.Teacher_ID == manualId))
                {
                    ErrorText.Text = $"Преподаватель с ID {manualId} уже существует";
                    return;
                }

                TeacherId = manualId;
            }

            FullName = FullNameTextBox.Text.Trim();
            Street = StreetTextBox.Text.Trim();
            House = HouseTextBox.Text.Trim();
            Apartment = ApartmentTextBox.Text.Trim();

            if (CategoryComboBox.SelectedItem is Category cat)
                CategoryId = cat.Category_ID;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
