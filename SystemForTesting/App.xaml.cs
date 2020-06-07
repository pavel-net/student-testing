using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;

namespace SystemForTesting
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            mutex = new Mutex(false, "SystemForTestingMutexForCheckOneInstance", out createdNew);
            if (!createdNew)
            {
                bool flagExit = false;
                // м.б. не удалось создать мютекс, дополнительно проверим, запущен ли процесс с таким же именем
                Process currentProcess = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(currentProcess.ProcessName))
                {
                    if (process.Id == currentProcess.Id)
                        continue;
                    flagExit = true;    // нашли другой процесс с таким же именем
                }
                if (flagExit)
                    Shutdown();
            }
            //StyleHelper.CreateStyleForwardersForDefaultStyles();
            //ApplicationThemeHelper.ApplicationThemeName
            //ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017Blue.Name;
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
