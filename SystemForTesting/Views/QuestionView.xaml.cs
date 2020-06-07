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
using SystemForTesting.ViewModels;
using Catel.Windows;
using DevExpress.Xpf.Grid;

namespace SystemForTesting.Views
{
    /// <summary>
    /// Логика взаимодействия для QuestionView.xaml
    /// </summary>
    public partial class QuestionView : DataWindow
    {
        public QuestionView() : base(DataWindowMode.OkCancel)
        {
            InitializeComponent();
           
        }

        //void OnGridCustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        //{
        //    //if (e.Column.FieldName == "FlagCorrectly2")
        //    //{
        //    //    var row = e.Source.GetRowByListIndex(e.ListSourceRowIndex) as AnswerViewModel;
        //    //    if (e.IsGetData)
        //    //    {
        //    //        e.Value = row.FlagCorrectly == "Y";
        //    //    }
        //    //    if (e.IsSetData)
        //    //    {
        //    //        row.FlagCorrectly = (bool)e.Value ? "Y" : "N";
        //    //    }
        //    //}
        //}
    }
}
