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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            GCFScapeFolder = Utility.GetGCFScapeInstallFolder();
            VRFFolder = Utility.GetVRFInstallFolder();
        }

        public static readonly DependencyProperty GCFScapeFolderProperty =
            DependencyProperty.Register(nameof(GCFScapeFolder), typeof(string),
            typeof(SettingsWindow));
        public string GCFScapeFolder
        {
            get
            {
                return (string)GetValue(GCFScapeFolderProperty);
            }
            set
            {
                this.SetValue(GCFScapeFolderProperty, value);
            }
        }
        public static readonly DependencyProperty VRFFolderProperty =
            DependencyProperty.Register(nameof(VRFFolder), typeof(string),
            typeof(SettingsWindow));
        public string VRFFolder
        {
            get
            {
                return (string)GetValue(VRFFolderProperty);
            }
            set
            {
                this.SetValue(VRFFolderProperty, value);
            }
        }

        private void OnInstallGCFScape(object sender, RoutedEventArgs e)
        {
            InstallGCFScape();
        }
        void InstallGCFScape()
        {

            System.Diagnostics.Process.Start(Utility.GCFScapeHomePage);
        }

        private void OnInstallVRF(object sender, RoutedEventArgs e)
        {
            InstallVRF();
        }
        void InstallVRF()
        {

            System.Diagnostics.Process.Start(Utility.VRFHomePage);

        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }


        private void OnLocateGCFScape(object sender, RoutedEventArgs e)
        {
            LocateGCF();
        }
        void LocateGCF()
        {
            string folder = LocateAppInstallFolder(Utility.GCFScapeEXE, "GCFScape");
            if (!string.IsNullOrEmpty(folder))
            {
                Configuration.Current.GCFScapeInstallLocation = folder;
                Configuration.Current.Save();
                GCFScapeFolder = folder;
            }
        }
        void LocateVRF()
        {
            string folder = LocateAppInstallFolder(Utility.VRFEXE, "VRF");
            if (!string.IsNullOrEmpty(folder))
            {
                Configuration.Current.VRFInstallLocation = folder;
                Configuration.Current.Save();
                VRFFolder = folder;
            }
        }
        string LocateAppInstallFolder(string exe, string appName)
        {
            string retVal;
            System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog();

            diag.ShowNewFolderButton = false;
            diag.Description = string.Format("Select path {0}", appName);
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folder = diag.SelectedPath;
                if (System.IO.File.Exists(System.IO.Path.Combine(folder, exe)))
                {
                    retVal = folder;

                }
                else
                {
                    retVal = null;
                    MessageBox.Show(string.Format("{0} executable not found.", appName),
                        string.Format("Locating {0}", appName),
                         MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                retVal = null;
            }
            return retVal;
        }

        private void OnLocateVRF(object sender, RoutedEventArgs e)
        {
            LocateVRF();
        }

        private void OnInstallHLAWorkshopAssistant(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Utility.MyHomePage);
        }
    }
}
