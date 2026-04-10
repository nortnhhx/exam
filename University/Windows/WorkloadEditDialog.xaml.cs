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
    /// Логика взаимодействия для WorkloadEditDialog.xaml
    /// </summary>
    public partial class WorkloadEditDialog : Window
    {
        public int? WorkloadId { get; private set; }
        public int? TeacherId { get; private set; }
        public int? SubjectId { get; private set; }
        public bool IsNewWorkload { get; private set; }

        private UniversityEntities context;
        private int workloadId;

        private WorkloadEditDialog(bool isEdit)
        {
            InitializeComponent();
            context = new UniversityEntities();
            LoadData();
            IsNewWorkload = !isEdit;
        }

        public static WorkloadEditDialog ForCreate()
        {
            var dialog = new WorkloadEditDialog(false);
            dialog.Title = "Добавление нагрузки";
            dialog.ManualIdLabel.Visibility = Visibility.Visible;
            dialog.ManualIdTextBox.Visibility = Visibility.Visible;
            return dialog;
        }

        public WorkloadEditDialog(Workload workload) : this(true)
        {
            workloadId = workload.Workload_ID;

            IdLabel.Visibility = Visibility.Visible;
            IdText.Visibility = Visibility.Visible;
            IdText.Text = workload.Workload_ID.ToString();

            ManualIdLabel.Visibility = Visibility.Collapsed;
            ManualIdTextBox.Visibility = Visibility.Collapsed;

            if (workload.Teacher_ID.HasValue)
            {
                var teacher = context.Teachers.Find(workload.Teacher_ID.Value);
                if (teacher != null)
                    TeacherComboBox.SelectedItem = teacher;
            }

            if (workload.Subject_ID.HasValue)
            {
                var subject = context.Subject.Find(workload.Subject_ID.Value);
                if (subject != null)
                    SubjectComboBox.SelectedItem = subject;
            }
        }

        private void LoadData()
        {
            var teachers = context.Teachers.OrderBy(t => t.FullName).ToList();
            TeacherComboBox.ItemsSource = teachers;
            if (teachers.Any())
                TeacherComboBox.SelectedIndex = 0;

            var subjects = context.Subject
                .Include("Cycle")
                .OrderBy(s => s.SubjectName)
                .ToList();
            SubjectComboBox.ItemsSource = subjects;
            if (subjects.Any())
                SubjectComboBox.SelectedIndex = 0;

            SubjectComboBox.SelectionChanged += (s, e) => UpdateSubjectInfo();
            UpdateSubjectInfo();
        }

        private void UpdateSubjectInfo()
        {
            if (SubjectComboBox.SelectedItem is Subject subject)
            {
                string cycle = subject.Cycle?.CycleName ?? "Не указан";
                string hours = subject.HourseAmount?.ToString() ?? "0";
                SubjectInfoText.Text = $"Цикл: {cycle}\nОбъем часов: {hours}\nID предмета: {subject.Subject_ID}";
            }
            else
            {
                SubjectInfoText.Text = "";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherComboBox.SelectedItem == null)
            {
                ErrorText.Text = "Выберите преподавателя";
                return;
            }

            if (SubjectComboBox.SelectedItem == null)
            {
                ErrorText.Text = "Выберите предмет";
                return;
            }

            if (IsNewWorkload && !string.IsNullOrWhiteSpace(ManualIdTextBox.Text))
            {
                if (!int.TryParse(ManualIdTextBox.Text.Trim(), out int manualId))
                {
                    ErrorText.Text = "ID должен быть числом";
                    return;
                }

                if (context.Workload.Any(w => w.Workload_ID == manualId))
                {
                    ErrorText.Text = $"Запись с ID {manualId} уже существует";
                    return;
                }

                WorkloadId = manualId;
            }

            TeacherId = ((Teachers)TeacherComboBox.SelectedItem).Teacher_ID;
            SubjectId = ((Subject)SubjectComboBox.SelectedItem).Subject_ID;

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
