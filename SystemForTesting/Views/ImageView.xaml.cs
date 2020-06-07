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
using Catel.Windows;

namespace SystemForTesting.Views
{
    /// <summary>
    /// Логика взаимодействия для ImageView.xaml
    /// </summary>
    public partial class ImageView : DataWindow 
    {
        public ImageView() : base(DataWindowMode.Close)
        {
            InitializeComponent();
        }
    }
}
