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

namespace HLA_Workshop_Assistant.Wpf
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            AppVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).FileVersion;
            InitializeComponent();
        }

        public static readonly DependencyProperty AppVersionProperty =
               DependencyProperty.Register(nameof(AppVersion), typeof(string),
               typeof(About));
        public string AppVersion
        {
            get
            {
                return (string)GetValue(AppVersionProperty);
            }
            set
            {
                this.SetValue(AppVersionProperty, value);
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
