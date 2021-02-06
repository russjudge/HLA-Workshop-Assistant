using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace HLA_Workshop_Assistant
{
    /// <summary>
    /// Interaction logic for SteamWorkshopItemControl.xaml
    /// </summary>
    public partial class SteamWorkshopItemControl : UserControl
    {
        public SteamWorkshopItemControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ActiveWorkshopItemsProperty =
            DependencyProperty.Register(nameof(ActiveWorkshopItems), typeof(ObservableCollection<SteamWorkshopItem>),
            typeof(SteamWorkshopItemControl));
        public ObservableCollection<SteamWorkshopItem> ActiveWorkshopItems
        {
            get
            {
                return (ObservableCollection<SteamWorkshopItem>)GetValue(ActiveWorkshopItemsProperty);
            }
            set
            {
                this.SetValue(ActiveWorkshopItemsProperty, value);
            }
        }
        public static readonly DependencyProperty GCFScapeConfiguredProperty =
            DependencyProperty.Register(nameof(GCFScapeConfigured), typeof(bool),
            typeof(SteamWorkshopItemControl));
        public bool GCFScapeConfigured
        {
            get
            {
                return (bool)GetValue(GCFScapeConfiguredProperty);
            }
            set
            {
                this.SetValue(GCFScapeConfiguredProperty, value);
            }
        }


        public static readonly DependencyProperty VRFConfiguredProperty =
           DependencyProperty.Register(nameof(VRFConfigured), typeof(bool),
           typeof(SteamWorkshopItemControl));
        public bool VRFConfigured
        {
            get
            {
                return (bool)GetValue(VRFConfiguredProperty);
            }
            set
            {
                this.SetValue(VRFConfiguredProperty, value);
            }
        }
        private void OnOpenInGCFScape(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                SteamWorkshopItem key = btn.CommandParameter as SteamWorkshopItem;
                OpenInGCFScape(key.Key);
            }
        }
        private void OpenInGCFScape(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (Utility.IsGCFScapeInstalled())
                {
                    if (Utility.OpenInGCFScape(key))
                    {

                    }
                    else
                    {
                        MessageBox.Show("Unable to open GCFScape:\r\n" + Utility.LastErrorMessage, "Open In GCFScape", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    if (MessageBox.Show("GCFScape is not installed.  Would you like to go to the Download page for GCFScape?", "GCFScape", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Utility.GCFScapeHomePage);
                    }
                }
            }
        }
        private void OpenInVRF(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (Utility.IsVRFInstalled())
                {
                    if (Utility.OpenInVRF(key))
                    {
                    }
                    else
                    {
                        MessageBox.Show("Unable to open VRF:\r\n" + Utility.LastErrorMessage, "Open In VRF", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    if (MessageBox.Show("VRF is not installed.  Would you like to go to the Download page for VRF?", "VRF", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Utility.VRFHomePage);
                    }
                }
            }
        }
        private void OnOpenInVRF(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                SteamWorkshopItem key = btn.CommandParameter as SteamWorkshopItem;
                OpenInVRF(key.Key);
            }
        }

        private void OnItemSelected(object sender, MouseButtonEventArgs e)
        {
            ListView me = sender as ListView;
            if (me != null)
            {
                var selectedItem = me.SelectedItem as SteamWorkshopItem;
                if (selectedItem != null)
                {
                    string key = selectedItem.Key;
                    OpenInGCFScape(key);
                }
            }

        }

        private void OnOpenFolder(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                SteamWorkshopItem item = btn.CommandParameter as SteamWorkshopItem;
                if (item != null)
                {
                    var path = System.IO.Path.Combine(Utility.GetHLAWorkshopFolder(), item.Key);
                    System.Diagnostics.Process.Start("\"" + path + "\"");
                }
            }
        }

        private void OnOpenWorkshopWebpage(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                SteamWorkshopItem item = btn.CommandParameter as SteamWorkshopItem;
                if (item != null)
                {
                    System.Diagnostics.Process.Start("\"" + Utility.GetWorkshopWebpageURL(item.Key) + "\"");
                }
            }
        }
        private void OnEnterNote(object sender, RoutedEventArgs e)
        {

            Button me = sender as Button;

            if (me != null)
            {
                var data = me.CommandParameter as SteamWorkshopItem;
                NoteWindow win = new NoteWindow();

                win.Note = data.Note;
                string title = data.Title;
                string author = data.Author;
                if (string.IsNullOrEmpty(author))
                {
                    author = "???";
                }

                win.Title = string.Format("Notes for AddOn \"{0}\" by {1}", title, author);

                win.ShowDialog();
                data.Note = win.Note;
            }
        }
    }
}
