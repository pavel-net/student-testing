using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
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
using TestingAdminProgram.Models;
using DevExpress.Xpf.Core.ServerMode;

namespace TestingAdminProgram
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestDbContext db;
        private bool IsResultTestLoaded = false;
        public static string ConnectionString; 
        public MainWindow()
        {
            ConnectionString = File.ReadAllText("СТРОКА ПОДКЛЮЧЕНИЯ.txt");
            InitializeComponent();
            //try
            //{
            //    WriteLog("Запуск приложения");
            //    ConnectionString = File.ReadAllText("СТРОКА ПОДКЛЮЧЕНИЯ.txt");
            //    WriteLog("InitializeComponent Start");
            //    InitializeComponent();
            //    WriteLog("InitializeComponent End");
            //    TestConnection();
            //}
            //catch (Exception e)
            //{
            //    WriteLog(e.Message, e.ToString());
            //}
        }

        //private bool TestConnection()
        //{
        //    string connStr =
        //        "data source=localhost\\SQLEXPRESS;initial catalog=master;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connStr))
        //        {
        //            connection.Open();
        //        }
        //        WriteLog("Проверка подключения к базе успешно завершена");
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        WriteLog(
        //            "Ошибка подключения к базе данных. Использовалась следующая строка подключения: " +
        //            connStr + "\r\n" + e.Message, e.ToString());
        //        return false;
        //    }
        //}

        //private void WriteError(string title, string content)
        //{
        //    MessageBox.Show("Произошла ошибка: " + title + "\r\nПодробное содержание:\r\n" + content, "Ошибка",
        //        MessageBoxButton.OK, MessageBoxImage.Error);
        //}

        //public static void WriteLog(string title, string content = null)
        //{
        //    using (StreamWriter sw = new StreamWriter("ОТЧЁТ О РАБОТЕ.txt", true, Encoding.UTF8))
        //    {
        //        sw.WriteLine();
        //        sw.WriteLine("=====");
        //        sw.WriteLine(DateTime.Now.ToString() + " " + title);
        //        if (content != null)
        //            sw.WriteLine(content);
        //    }
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            db = new TestDbContext();
            db.Disciplines.Load();
            gridDiscipline.ItemsSource = db.Disciplines.Local.ToBindingList();

            db.Teachers.Load();
            gridTeacher.ItemsSource = db.Teachers.Local.ToBindingList();

            db.Students.Load();
            gridStudent.ItemsSource = db.Students.Local.ToBindingList();
            //try
            //{
            //    WriteLog("Window_Loaded Start");
            //    db = new TestDbContext();
            //    WriteLog("Disciplines Load");
            //    db.Disciplines.Load();
            //    gridDiscipline.ItemsSource = db.Disciplines.Local.ToBindingList();

            //    WriteLog("Teachers Load");
            //    db.Teachers.Load();
            //    gridTeacher.ItemsSource = db.Teachers.Local.ToBindingList();

            //    db.Students.Load();
            //    gridStudent.ItemsSource = db.Students.Local.ToBindingList();
            //    WriteLog("Window_Loaded Finish");
            //}
            //catch (Exception ex)
            //{
            //    //WriteError(ex.Message, ex.ToString());
            //    WriteLog(ex.Message, ex.ToString());
            //}
        }

        private void LoadTestResult()
        {
            IsResultTestLoaded = true;
            db.TestResults.Include(t => t.Student).Include(t => t.Test).Load();
            gridTest.ItemsSource = db.TestResults.Local.ToBindingList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db?.Dispose();
        }

        private void BarButtonItem_Save(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            string error = CheckTableTeachers();
            if (error != null)
            {
                MessageBox.Show("Невозможно зафиксировать изменения из-за ошибки в таблице Преподавателей! " + error,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FixEdit();
            db.SaveChanges();
            MessageBox.Show("Изменения успешно сохранены",
                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FixEdit()
        {
            gridDiscipline.View.PostEditor();
            gridStudent.View.PostEditor();
            gridTeacher.View.PostEditor();
        }

        private void BarButtonItem_DisciplineAdd(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Discipline d = new Discipline();
            d.Name = "Новая дисциплина";
            db.Disciplines.Add(d);
        }

        private void BarButtonItem_DisciplineDelete(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var rows = gridDiscipline.SelectedItems;
            if (rows == null || rows.Count == 0)
                return;
            string text = rows.Count == 1
                ? ("Вы действительно хотите удалить выбранную дисциплину '" + ((Discipline) rows[0]).Name) + "'. "
                : "Вы действительно хотите удалить выбранные дисциплины (" + rows.Count + " штук). ";
            text +=
                "Внимание! Удаление дисциплины не приводит к удалению привязанных к ней тем и тестов! Но они в дальнейшем станут недоступны пользователям в интерфейсе. " +
                "Если темы всё же требуется удалить, то закройте это окно и сделайте удаление тем вручную под логином преподавателя.";
            var result = MessageBox.Show(text, "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;
            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var d = (Discipline) rows[i];
                var list = db.Topics.Where(t => t.IdDiscipline == d.Id).ToList();
                if (list.Count == 0)
                    db.Disciplines.Remove((Discipline)rows[i]);
                else
                {
                    MessageBox.Show("В заданной дисциплине присутствуют темы. Удаление отменено. Сначала очистите список тем под логином преподавателя.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BarButtonItem_TeacherAdd(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Teacher t = new Teacher()
            {
                Fio = "Иванов",
                Login = "Ivanov",
                Password = "1"
            };
            db.Teachers.Add(t);
        }

        private void BarButtonItem_TeacherDelete(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var rows = gridTeacher.SelectedItems;
            if (rows == null || rows.Count == 0)
                return;
            for (int i = rows.Count - 1; i >= 0; i--)
            {
                db.Teachers.Remove((Teacher)rows[i]);
            }
        }

        private void BarButtonItem_TestDelete(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var rows = gridTest.SelectedItems;
            if (rows == null || rows.Count == 0)
                return;
            for (int i = rows.Count - 1; i >= 0; i--)
            {
                db.TestResults.Remove((TestResult)rows[i]);
            }
        }


        private string CheckTableTeachers()
        {
            string error = null;
            if (db.Teachers.Local.Any(t => string.IsNullOrWhiteSpace(t.Login)))
                return
                    "Найдена запись, содержащая пустое поле Login. Удалите эту запись или введите логин прежде чем делать сохранение.";
            HashSet<string> loginSet = new HashSet<string>();
            foreach (var teacher in db.Teachers.Local)
            {
                teacher.Login = teacher.Login.Trim().ToLower();
                if (loginSet.Contains(teacher.Login))
                    return "Найдено 2 записи, содержащие одинаковое значение поля Login = '" + teacher.Login + "'. Удалите лишние записи прежде чем делать сохранение.";
                teacher.Password = teacher.Password?.Trim().ToLower();
                loginSet.Add(teacher.Login);
            }
            return error;
        }

        private void BarButtonItem_StudentDelete(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var rows = gridStudent.SelectedItems;
            if (rows == null || rows.Count == 0)
                return;
            for (int i = rows.Count - 1; i >= 0; i--)
            {
                db.Students.Remove((Student)rows[i]);
            }
        }

        private void tabControl_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            if (e.NewSelectedIndex == 3 && !IsResultTestLoaded)
            {
                LoadTestResult();
            }
        }
    }
}
