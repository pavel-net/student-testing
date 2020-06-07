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
using SystemForTesting.ViewModels;
using Catel.Windows;

namespace SystemForTesting
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DataWindow
    {
        public MainWindow() : base(DataWindowMode.Custom)
        {
            //try
            //{
                InitializeComponent();
            //}
            //catch (Exception e)
            //{
            //    MainWindowViewModel.WriteLog(e.Message, e.ToString());
            //}
        }
    }
}
