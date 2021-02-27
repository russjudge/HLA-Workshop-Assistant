using HLA_Workshop_Assistant.Wpf;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: Add self-updator
        public MainWindow()
        {
            Utility.NewWorkshopItemRetrieved += Utility_NewWorkshopItemRetrieved;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            
            InitializeComponent();
            this.Title = this.Title + " v. " + version;
            GCFScapeConfigured = Utility.IsGCFScapeInstalled();
            VRFConfigured = Utility.IsVRFInstalled();
            Refresh();

        }
        public static readonly DependencyProperty GCFScapeConfiguredProperty =
            DependencyProperty.Register(nameof(GCFScapeConfigured), typeof(bool),
            typeof(MainWindow));
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
           typeof(MainWindow));
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


        public static readonly DependencyProperty LoadAllWorkshopsProperty =
           DependencyProperty.Register(nameof(LoadAllWorkshops), typeof(bool),
           typeof(MainWindow));
        public bool LoadAllWorkshops
        {
            get
            {
                return (bool)GetValue(LoadAllWorkshopsProperty);
            }
            set
            {
                this.SetValue(LoadAllWorkshopsProperty, value);
            }
        }



        public static readonly DependencyProperty IsLoadingProperty =
           DependencyProperty.Register(nameof(IsLoading), typeof(bool),
           typeof(MainWindow), new PropertyMetadata(true));
        public bool IsLoading
        {
            get
            {
                return (bool)GetValue(IsLoadingProperty);
            }
            set
            {
                this.SetValue(IsLoadingProperty, value);
            }
        }


        public static readonly DependencyProperty TotalLoadingProperty =
          DependencyProperty.Register(nameof(TotalLoading), typeof(int),
          typeof(MainWindow));
        public int TotalLoading
        {
            get
            {
                return (int)GetValue(TotalLoadingProperty);
            }
            set
            {
                this.SetValue(TotalLoadingProperty, value);
            }
        }
        public static readonly DependencyProperty InstalledWorkshopItemsProperty =
            DependencyProperty.Register(nameof(InstalledWorkshopItems), typeof(ObservableCollection<SteamWorkshopItem>),
            typeof(MainWindow));
        public ObservableCollection<SteamWorkshopItem> InstalledWorkshopItems
        {
            get
            {
                return (ObservableCollection<SteamWorkshopItem>)GetValue(InstalledWorkshopItemsProperty);
            }
            set
            {
                this.SetValue(InstalledWorkshopItemsProperty, value);
            }
        }

        public static readonly DependencyProperty NotInstalledWorkshopItemsProperty =
            DependencyProperty.Register(nameof(NotInstalledWorkshopItems), typeof(ObservableCollection<SteamWorkshopItem>),
            typeof(MainWindow));
        public ObservableCollection<SteamWorkshopItem> NotInstalledWorkshopItems
        {
            get
            {
                return (ObservableCollection<SteamWorkshopItem>)GetValue(NotInstalledWorkshopItemsProperty);
            }
            set
            {
                this.SetValue(NotInstalledWorkshopItemsProperty, value);
            }
        }

        public static readonly DependencyProperty AllWorkshopItemsProperty =
            DependencyProperty.Register(nameof(AllWorkshopItems), typeof(ObservableCollection<SteamWorkshopItem>),
            typeof(MainWindow));
        public ObservableCollection<SteamWorkshopItem> AllWorkshopItems
        {
            get
            {
                return (ObservableCollection<SteamWorkshopItem>)GetValue(AllWorkshopItemsProperty);
            }
            set
            {
                this.SetValue(AllWorkshopItemsProperty, value);
            }
        }

        private void OnAbout(object sender, RoutedEventArgs e)
        {
            About win = new About();
            win.ShowDialog();
        }
       
        void LoadData()
        {
            LoadLocalData();
            StartLoadData();
        }
        void StartLoadData()
        {
            List<SteamWorkshopItem> loadedItems = new List<SteamWorkshopItem>(AllWorkshopItems);
            System.Threading.ThreadPool.QueueUserWorkItem(LoadData, loadedItems);

        }
        void LoadLocalData()
        {
            InstalledWorkshopItems = new ObservableCollection<SteamWorkshopItem>(Utility.GetHLAWorkshopList());

            foreach (var item in InstalledWorkshopItems)
            {
                if (item.IsLoading)
                {
                    item.LoadCompleted += WorkshopItem_LoadCompleted;
                    TotalLoading++;
                }
            }
            //InstalledItemsDisabled = false;
            AllWorkshopItems = new ObservableCollection<SteamWorkshopItem>(InstalledWorkshopItems);
            
            NotInstalledWorkshopItems = new ObservableCollection<SteamWorkshopItem>();
        }

        void LoadData(object state)
        {
            var loadedItems = state as List<SteamWorkshopItem>;
           
            
            
            //Utility.LoadCompleted += Utility_LoadCompleted;
            Utility.GetAllSteamWorkshopItems(loadedItems);
        }

        private void Utility_NewWorkshopItemRetrieved(object sender, NewSteamWorkshopItemEventArgs e)
        {
            
            this.Dispatcher.BeginInvoke(new Action(() => 
            {
                //DisplayChoices = true;
                AllWorkshopItems.Add(e.WorkshopItem);
                if (e.WorkshopItem.IsLoading)
                {
                    e.WorkshopItem.LoadCompleted += WorkshopItem_LoadCompleted;
                    TotalLoading++;
                }
                //if (AllWorkshopItems.Count == 1)
                //{
                //    AllItemsDisabled = true;
                //}
                bool IsInstalled = false;
                foreach (var item in InstalledWorkshopItems)
                {
                    if (item.Key == e.WorkshopItem.Key)
                    {
                        IsInstalled = true;
                        break;
                    }
                }
                if (!IsInstalled)
                {
                    NotInstalledWorkshopItems.Add(e.WorkshopItem);
                }
            }));
        }
        private void WorkshopItem_LoadCompleted(object sender, EventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() => { TotalLoading--; }));
            }
            catch (Exception)
            {

            }
        }

        bool IsAddOnInstalled(string key)
        {
            bool retVal = false;
            foreach (var item in InstalledWorkshopItems)
            {
                if (item.Key == key)
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }
        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void OnSettings(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
            GCFScapeConfigured = Utility.IsGCFScapeInstalled();
            VRFConfigured = Utility.IsVRFInstalled();
        }

        private void OnGotoSteamWorkshopList(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Utility.AlyxWorkshopListURL);
        }
        void Refresh()
        {
            if (LoadAllWorkshops)
            {
                LoadData();
            }
            else
            {
                LoadLocalData();
            }
        }
        private void OnLoadAllWorkshops(object sender, RoutedEventArgs e)
        {
            LoadAllWorkshops = true;
            StartLoadData();
        }
    }
}
