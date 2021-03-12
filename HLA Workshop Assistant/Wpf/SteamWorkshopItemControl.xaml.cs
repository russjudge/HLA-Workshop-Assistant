using HLA_Workshop_Assistant.Wpf;
using Microsoft.Win32;
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
        public static readonly DependencyProperty SelectedItemProperty =
          DependencyProperty.Register(nameof(SelectedItem), typeof(SteamWorkshopItem),
          typeof(SteamWorkshopItemControl));
        public SteamWorkshopItem SelectedItem
        {
            get
            {
                return (SteamWorkshopItem)GetValue(SelectedItemProperty);
            }
            set
            {
                this.SetValue(SelectedItemProperty, value);
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

        private void OnExport(object sender, RoutedEventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Filter = "*.csv|*.csv|All Files |*.*";
            diag.DefaultExt = "csv";
            diag.OverwritePrompt = true;
            if (diag.ShowDialog() == true)
            {
                Utility.Export(diag.FileName, ActiveWorkshopItems);
            }
        }

        private void OnAuthorProfile(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                var item = btn.CommandParameter as SteamWorkshopItem;
                if (item !=null)
                {
                    System.Diagnostics.Process.Start(item.AuthorProfileURL);
                }
            }
        }
        public void Find()
        {
            search = PromptDialog.ShowPrompt("Search", "Enter text to search (no wildcards)");
            RepeatFind();
        }
        public void RepeatFind()
        {

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToUpperInvariant();
                bool match = false;

                int startend;
                if (SelectedItem == null)
                {
                    startend = 0;
                }
                else
                {
                    startend = this.ActiveWorkshopItems.IndexOf(SelectedItem);
                }
                for (int i = startend + 1; i < ActiveWorkshopItems.Count; i++)
                {
                    var item = ActiveWorkshopItems[i];
                    match = IsMatch(item, search);

                    if (match)
                    {
                        MatchFound(item);
                        break;
                    }
                }
                if (startend > 0 && !match)
                {
                    for (int i = 0; i <= startend; i++)
                    {
                        var item = ActiveWorkshopItems[i];
                        match = IsMatch(item, search);
                        if (match)
                        {
                            MatchFound(item);
                            break;
                        }
                    }
                }


            }
        }
        string search = null;
        private void OnFind(object sender, RoutedEventArgs e)
        {
            Find();
        }
        void MatchFound(SteamWorkshopItem item)
        {
            SelectedItem = item;
            theListView.ScrollIntoView(SelectedItem);
        }
        bool IsMatch(SteamWorkshopItem item, string search)
        {
            bool match = false;

            if (!string.IsNullOrEmpty(item.Description))
            {
                match = item.Description.ToUpperInvariant().Contains(search);
            }
            if (!match && !string.IsNullOrEmpty(item.Author))
            {
                match = item.Author.ToUpperInvariant().Contains(search);
            }
            if (!match && !string.IsNullOrEmpty(item.Title))
            {
                match = item.Title.ToUpperInvariant().Contains(search);
            }
            return match;
        }

        private void OnControlKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Find();
                e.Handled = true;
            }
            if (e.Key == Key.F3)
            {
                RepeatFind();
                e.Handled = true;
            }

        }
    }
}
