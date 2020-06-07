using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;

namespace TestingAdminProgram
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ApplicationThemeHelper.ApplicationThemeName = Theme.Office2007BlueName;
            //ApplicationThemeHelper.ApplicationThemeName = Theme.DeepBlue.Name;
            base.OnStartup(e);
        }

        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {
            //DevExpress.Xpf.Core.
            DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
        }

    }

}
