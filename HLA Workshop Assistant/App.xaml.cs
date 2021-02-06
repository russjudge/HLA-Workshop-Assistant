using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HLA_Workshop_Assistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStart(object sender, StartupEventArgs e)
        {
         
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Configuration.Current.Save();
        }
    }
}
