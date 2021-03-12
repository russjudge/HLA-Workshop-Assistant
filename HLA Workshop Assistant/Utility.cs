using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Windows.Media.Imaging;
using System.Web.Script.Serialization;
using System.Windows;

namespace HLA_Workshop_Assistant
{
    public static class Utility
    {
        public const string MyHomePage = "https://gamebanana.com/tools/6944";
        public const string AlyxWorkshopListURL = "https://steamcommunity.com/app/546560/workshop/";

        const string SteamRegistryKey = @"SOFTWARE\Valve\Steam";
        const string SteamRegistrySteamPathValue = "SteamPath";
        const string SteamAppsRelativeFolder = "steamApps";
        const string SteamLibraryFolderList = "libraryFolders.vdf";

        const string SteamAddOnListURL = "https://steamcommunity.com/workshop/browse/?appid=546560&browsesort=trend&section=readytouseitems&days=90&numperpage=30&p={0}";
        //<span class="pagebtn disabled">&lt;</span>&nbsp;1&nbsp;&nbsp;<a class="pagelink" href="https://steamcommunity.com/workshop/browse/?appid=546560&browsesort=trend&section=readytouseitems&days=90&numperpage=30&actualsort=trend&p=2">2</a>&nbsp;&nbsp;<a class="pagelink" href="https://steamcommunity.com/workshop/browse/?appid=546560&browsesort=trend&section=readytouseitems&days=90&numperpage=30&actualsort=trend&p=3">3</a>&nbsp;...&nbsp;<a class="pagelink" href="https://steamcommunity.com/workshop/browse/?appid=546560&browsesort=trend&section=readytouseitems&days=90&numperpage=30&actualsort=trend&p=26">26</a>&nbsp;<a class='pagebtn' href="https://steamcommunity.com/workshop/browse/?appid=546560&browsesort=trend&section=readytouseitems&days=90&numperpage=30&actualsort=trend&p=2">&gt;</a>					</div>
        const string HLAWorkshopPath = @"steamapps\workshop\content";
        const string HLAKey = "546560";
        const string WorkshopInfoFile = "publish_data.txt";
        //https://steamcommunity.com/profiles/76561198260717307/myworkshopfiles/?appid=546560 
        //https://steamcommunity.com/profiles/cj_beans/myworkshopfiles/?appid=546560
        //const string authorURLFormat = "https://steamcommunity.com/profiles/{0}/myworkshopfiles/?appid=" + HLAKey;
        const string authorURLFormat = "https://steamcommunity.com/id/{0}";

        /*
        const string GCFScapeRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\GCFScape_is1";
        const string SOFTWARE = "SOFTWARE";
        const string Microsoft = "Microsoft";
        const string Windows = "Windows";
        const string CurrentVersion = "CurrentVersion";
        const string Uninstall = "Uninstall";
        const string GCFScape = "GCFScape_is1";
        
        const string GCFScapeRegistryValue = "InstallLocation";
        */
        public const string GCFScapeHomePage = @"https://gamebanana.com/tools/26";
        public const string GCFScapeDownloadURL = @"https://gamebanana.com/tools/download/26";
        public const string GCFScapeDownloadInstallerURL = @"https://gamebanana.com/dl/468817";

        public const string GCFScapeEXE = "GCFScape.exe";

        public const string VRFHomePage = "https://vrf.steamdb.info/";
        public const string VRFEXE = "VRF.exe";

        //const string GCFScapeCommandFormat = "\"{0}\" \"{1}\"";
        public static string GetURLPage(string url)
        {
            string retVal;
            try
            {
                using (WebClient client = new WebClient())
                {
                    retVal = client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                retVal = null;
            }

            return retVal;
        }
        public static int ExtractTotalPageCount(string page)
        {
            int retVal = 1;
            //<div class="workshopBrowsePaging">
            var match1 = "<div class=\"workshopBrowsePagingControls\">";
            var match2 = "&gt;";
            var match3 = "&p=";

            int i = page.IndexOf(match1);
            int k = page.IndexOf(match2, i);
            i = page.LastIndexOf(match3, k);
            i = page.LastIndexOf(match3, --i) + match3.Length; ;
            k = i + 1;
            while (page[k] >= '0' && page[k] <= '9')
            {
                k++;
            }
            string pg = page.Substring(i, k - i);

            if (int.TryParse(pg, out retVal))
            {

            }
            return retVal;
        }

        static string ExtractNextWorkshopID(string page, ref int startAt)
        {
            string retVal;
            if (startAt < 0)
            {
                startAt = 0;
            }
            const string match1 = "data-publishedfileid=\"";
            int i = page.IndexOf(match1, startAt) + match1.Length;
            if (i > match1.Length)
            {
                startAt = page.IndexOf("\"", i);
                if (startAt > i)
                {
                    retVal = page.Substring(i, startAt - i);
                }
                else
                {
                    retVal = null;
                }
            }
            else
            {
                retVal = null;
            }
            return retVal;
        }
        public static string[] ExtractWorkshopIDList(string page)
        {
            //data-publishedfileid="2284657931"
            string id;
            List<string> retVal = new List<string>();
            int i = 0;
            while ((id = ExtractNextWorkshopID(page, ref i)) != null)
            {
                if (!loadedItemDict.ContainsKey(id))
                {
                    retVal.Add(id);
                    totalAddOns++;
                }
            }
            return retVal.ToArray();

        }
        public static event EventHandler<NewSteamWorkshopItemEventArgs> NewWorkshopItemRetrieved;
        static Dictionary<string, SteamWorkshopItem> loadedItemDict = new Dictionary<string, SteamWorkshopItem>();
        public static void GetAllSteamWorkshopItems(IEnumerable<SteamWorkshopItem> loadedItems)
        {

          
            //Go to page2: https://steamcommunity.com/workshop/browse/?appid=546560&browsesort=trend&section=readytouseitems&days=90&actualsort=trend&p=2
            
            foreach (var item in loadedItems)
            {
                if (loadedItemDict.ContainsKey(item.Key))
                {
                    loadedItemDict[item.Key] = item;
                }
                else
                {
                    loadedItemDict.Add(item.Key, item);
                }
            }
            List<SteamWorkshopItem> retVal = new List<SteamWorkshopItem>();
            try
            {
                string page = GetURLPage(string.Format(SteamAddOnListURL,1));
                int totalPages = ExtractTotalPageCount(page);

                System.Threading.ThreadPool.QueueUserWorkItem(ProcessWorkshopIDList, ExtractWorkshopIDList(page));

                for (int i = 2; i <= totalPages; i++)
                {
                    page = GetURLPage(string.Format(SteamAddOnListURL, i));
                    System.Threading.ThreadPool.QueueUserWorkItem(ProcessWorkshopIDList, ExtractWorkshopIDList(page));
                }
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
            }
        }

        static void ProcessWorkshopIDList(object state)
        {
            string[] idList = state as string[];
            if (idList != null)
            {
                foreach (var addOn in idList)
                {
                    SteamWorkshopItem item = UIDependencyObject.CreateInstance<SteamWorkshopItem>(addOn);
                    NewWorkshopItemRetrieved?.BeginInvoke(null, new NewSteamWorkshopItemEventArgs(item), null, null);
                }
            }
        }

        public static string GetSteamInstallFolder()
        {
            LastErrorMessage = "";
            string retVal;

            try
            {
                var reg = Registry.CurrentUser.OpenSubKey(SteamRegistryKey);
                if (reg == null)
                {
                    retVal = null;
                }
                else
                { 
                    var steamPath = reg.GetValue(SteamRegistrySteamPathValue);
                    retVal = steamPath as string;
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                LastErrorMessage = ex.Message;
            }

            return retVal;
        }
        
        public static string[] GetSteamLibraryFolders()
        {
            List<string> retVal = new List<string>();


            var steamFolder = GetSteamInstallFolder();
            if (Directory.Exists(steamFolder))
            {
                retVal.Add(steamFolder);
                string libraryListFileName = Path.Combine(steamFolder, SteamAppsRelativeFolder, SteamLibraryFolderList);
                
                if (File.Exists(libraryListFileName))
                {
                    var info = new SteamInfo(libraryListFileName);

                    foreach (var key in info.GetKeys())
                    {
                        int libraryNumber = 0;
                        if (int.TryParse(key, out libraryNumber))
                        {
                            retVal.Add(info.GetValue(key));
                        }
                    }
                }
            }
            return retVal.ToArray();
        }
        public static string GetWorkshopWebpage(string key)
        {
            return GetURLPage(GetWorkshopWebpageURL(key));
        }
        
        public static string ExtractImageURL(string page)
        {
            
            string retVal;
            const string matchkey = "<img id=\"previewImageMain\" class=\"workshopItemPreviewImageMain\" src=\"";
            int i = page.IndexOf(matchkey) + matchkey.Length;

            if (i > matchkey.Length)
            {
                int k = page.IndexOf("\"", i + 1);
                if (k > i)
                {
                    retVal = page.Substring(i, k - i);
                }
                else
                {
                    retVal = null;
                }
            }
            else
            {
                retVal = null;
            }

            if (string.IsNullOrEmpty(retVal))
            {
                retVal = ExtractFirstImageURL(page);
            }
            return retVal;
        }
        public static string ExtractFirstImageURL(string page)
        {
            string retVal;
            const string matchkey= "<a onclick=\"ShowEnlargedImagePreview( '";
            int i = page.IndexOf(matchkey) + matchkey.Length;
            if (i > matchkey.Length)
            {
                int k = page.IndexOf("'", i + 1);
                if (k > i)
                {
                    retVal = page.Substring(i, k - i);
                }
                else
                {
                    retVal = null;
                }
            }
            else
            {
                retVal = null;
            }
            return retVal;
        }
        public static byte[] DownloadBitmapBytes(string url)
        {
            byte[] bmp;

            try
            {
                using (WebClient client = new WebClient())
                {
                    var image = client.DownloadData(url);
                    bmp = image;
                }
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                bmp = null;
            }
            return bmp;
        }
      
        static void SpecialHTML(StringBuilder data, string matchTag)
        {
            int i = 0;
            string txt = data.ToString();
            while ((i = txt.IndexOf("<" + matchTag, StringComparison.InvariantCultureIgnoreCase)) > -1)
            {
                int j = txt.IndexOf(">",i, StringComparison.InvariantCultureIgnoreCase) +1;
                if (j < 0)
                {
                    data.Remove(i, data.Length - i);
                }
                else
                {
                    if (j < 0)
                    {
                        data.Remove(i, data.Length - i);

                    }
                    else
                    {
                        //string xx = data.ToString();
                        data.Remove(i, j - i);

                    }
                }
                txt = data.ToString();
            }
            
        }
        static void StringHTML(StringBuilder data, string matchTag)
        {
            int i = 0;
            string txt = data.ToString();

            while ((i = txt.IndexOf("<" + matchTag, StringComparison.InvariantCultureIgnoreCase)) > -1)
            {
                int j = txt.IndexOf("</" + matchTag + ">", StringComparison.InvariantCultureIgnoreCase) + matchTag.Length + 3;
                if (j - i < 0)
                {
                    data.Remove(i, data.Length - i);
                    
                }
                else
                {
                    data.Remove(i, j - i);
                    
                }
                txt = data.ToString();
            }
            
        }

        public static string ExtractDescription(string page)
        {
            const string matchkey = "<div class=\"workshopItemDescription\" id=\"highlightContent\">";
            const string endMatchKey1 = "<script>";
            const string endMatchKey2 = "</div>";
            StringBuilder sb = new StringBuilder();
            int i = page.IndexOf(matchkey, StringComparison.InvariantCultureIgnoreCase) + matchkey.Length;
            if (i < matchkey.Length)
            {
                return null;
            }

            int k = page.IndexOf(endMatchKey1, i, StringComparison.InvariantCultureIgnoreCase);
            if (k > i)
            {
                k = page.LastIndexOf(endMatchKey2, k);
                if (k > i)
                {
                    k = page.LastIndexOf(endMatchKey2, k - 1);
                    if (k > i)
                    {
                        string wrk = page.Substring(i, k - i);
                        sb.Append(wrk);
                    }
                }
            }

            
            //if (wrk.ToLowerInvariant().Contains("<div"))
            //{
            //    k = page.IndexOf("</div>", k + 1, StringComparison.InvariantCultureIgnoreCase);
            //}

            //StringBuilder sb = new StringBuilder();
            //string txt = page.Substring(i, k - i);
            //sb.Append(txt);
            //sb.Replace("&quot;", "\"");
            //sb.Replace("&amp;", "&");
            //sb.Replace("&euro;", "\u20AC");
            //sb.Replace("&copy;", "©");
            //sb.Replace("&reg", "®­");



            //sb.Replace("<br>", " ");

            //StringHTML(sb, "ul");
            //StringHTML(sb, "a");
            //StringHTML(sb, "b");
            //StringHTML(sb, "span");



            //SpecialHTML(sb, "div");
            //SpecialHTML(sb, "/div");
            //SpecialHTML(sb, "img");

            //sb.Replace("&lt;", "<");
            //sb.Replace("&gt;", ">");

            
            return sb.ToString().Trim();
        }
        static int completedAddOns = 0;
        public static event EventHandler LoadCompleted;
        public static Tuple<DateTime,DateTime,string> ExtractLastUpdate(string page)
        {
            LastErrorMessage = "";
            DateTime updatedDateTime;
            DateTime postedDateTime;
            string size = null;
            const string matchkey = "<div class=\"detailsStatLeft\">";
            const string matchKey2 = "<div class=\"detailsStatRight\">";
            const string endMatchKey3 = "</div>";
            Tuple<DateTime, DateTime, string> retVal;
            DateTime dt;
            string[] detailsStatRight = new string[3];

            try
            {
                int i = page.IndexOf(matchkey) + matchkey.Length;
                if (i > matchkey.Length)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        i = page.IndexOf(matchKey2, i) + matchKey2.Length;
                        if (i < matchKey2.Length)
                        {
                            //There is a problem.
                            break;
                        }
                        else
                        {
                            int j = page.IndexOf(endMatchKey3, i);
                            if (j > i)
                            {
                                detailsStatRight[k] = page.Substring(i, j - i);
                            }
                        }
                    }

                    size = detailsStatRight[0];

                    //If current year: Jan 24 @ time;
                    //May 15, 2020 @ 1:59pm


                    if (DateTime.TryParse(fixDate(detailsStatRight[1]), out dt))
                    {
                        postedDateTime = dt;
                    }
                    else
                    {
                        postedDateTime = DateTime.MinValue;
                    }
                    if (!string.IsNullOrEmpty(detailsStatRight[2]) && DateTime.TryParse(fixDate(detailsStatRight[2]), out dt))
                    {
                        updatedDateTime = dt;
                    }
                    else
                    {
                        updatedDateTime = DateTime.MinValue;
                    }
                    retVal = new Tuple<DateTime, DateTime, string>(postedDateTime, updatedDateTime, size);

                }
                else
                {
                    postedDateTime = DateTime.MinValue;
                    updatedDateTime = DateTime.MinValue;
                    retVal = null;
                }


            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                postedDateTime = DateTime.MinValue;
                updatedDateTime = DateTime.MinValue;

                retVal = null;
            }
            //if (updatedDateTime.CompareTo(DateTime.MinValue) == 0 && postedDateTime.CompareTo(DateTime.MinValue) > 0)
            //{
            //    updatedDateTime = postedDateTime;
            //}

            return retVal;
        }
        static string fixDate(string steamDate)
        {
            string retVal = steamDate;
            //If current year: Jan 24 @ time;
            //May 15, 2020 @ 1:59pm
            int i = steamDate.IndexOf(", ");
            if (i < 0)
            {
                retVal = retVal.Replace(" @", ", " + DateTime.Today.Year.ToString() + " @");
            }
            return retVal.Replace("@", string.Empty);
            
        }
        public static string ExtractTitle(string page)
        {
            LastErrorMessage = "";
            string retVal;
            try
            {
                const string matchkey = "<title>";
                const string endMatchKey = "</title>";
                int i = page.IndexOf(matchkey) + matchkey.Length;
                if (i > matchkey.Length)
                {
                    int k = page.IndexOf(endMatchKey, i);
                    if (k > i)
                    {
                        retVal = page.Substring(i, k - i)
                            .Replace("Steam Workshop::", string.Empty)
                            .Replace("&quot;", "\"")
                            .Replace("&amp;", "&");
                    }
                    else
                    {
                        retVal = null;
                    }
                }
                else
                {
                    LastErrorMessage = "Title Not Found";
                    retVal = null;
                }
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                retVal = null;
            }
            return retVal;
        }
        //17
     
        public static string ExtractAuthorProfileURL(string page)
        {

            //Need to identify type--they are not compatible with each other.
            /*
            
               //https://steamcommunity.com/profiles/76561198260717307/myworkshopfiles/?appid=546560 
        //https://steamcommunity.com/profiles/cj_beans/myworkshopfiles/?appid=546560
            */
            const string matchkey = "https://steamcommunity.com/profiles/";
            const string matchkey2 = "https://steamcommunity.com/id/";
            string retVal;
            LastErrorMessage = "";
         
            try
            {
                int i = page.IndexOf(matchkey);
                int m = i + matchkey.Length;
                if (i > -1)
                {
                    
                    int k = page.IndexOf("/", m);
                    if (k - m > 17)
                    {
                        i = page.IndexOf(matchkey2);
                        if (i > -1)
                        {
                            k = page.IndexOf("\"", i);
                            retVal = page.Substring(i, k - i);
                        }
                        else
                        {
                            retVal = null;
                        }
                    }
                    else if (k > i)
                    {
                        retVal = page.Substring(i, k - i);
                    }
                    else
                    {
                       
                        retVal = null;
                    }
                }
                else
                {
                    i = page.IndexOf(matchkey2);
                    if (i > -1)
                    {
                        int k = page.IndexOf("\"", i);
                        retVal = page.Substring(i, k - i);
                    }
                    else
                    {
                        retVal = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                retVal = null;
            }
            return retVal;
        }
        public static string ExtractAuthor(string page)
        {
            LastErrorMessage = "";
            string retVal;
            try
            {
                const string matchkey = "'s Workshop";
                int i = page.IndexOf(matchkey);
                if (i > matchkey.Length)
                {
                    int k = page.LastIndexOf(">", i);
                    if (i > k)
                    {
                        k++;
                        retVal = page.Substring(k, i - k);
                    }
                    else
                    {
                        retVal = null;
                    }
                }
                else
                {
                    retVal = null;
                }
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                retVal = null;
            }
            return retVal;
        }
        public static string GetWorkshopWebpageURL(string key)
        {
            return string.Format(@"https://steamcommunity.com/sharedfiles/filedetails/?id={0}&result=1", key);
        }
        static string HLAWorkshopFolder = null;
        public static string GetHLAWorkshopFolder()
        {
         
            if (string.IsNullOrEmpty(HLAWorkshopFolder) || !Directory.Exists(HLAWorkshopFolder))
            {
                var folderList = GetSteamLibraryFolders();
                
                foreach (var path in folderList)
                {
                    string testFolder = Path.Combine(path, HLAWorkshopPath, HLAKey);
                    if (Directory.Exists(testFolder))
                    {
                        HLAWorkshopFolder = testFolder;
                        break;
                    }
                }
            }

            return HLAWorkshopFolder;
        }
        public static void Export(string filename, IEnumerable<SteamWorkshopItem> items)
        {
            string itemFormat = "\"{0}\",";
            try
            {
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    sw.WriteLine("\"Name\",\"Link\",\"Author\",\"Author Link\",\"Release Date\",\"Last Update\",\"Main Image Link\"");
                    foreach (var item in items)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat(itemFormat, item.Title);
                        sb.AppendFormat(itemFormat, item.PageURL);
                        sb.AppendFormat(itemFormat, item.Author);
                        sb.AppendFormat(itemFormat, item.AuthorProfileURL);
                        sb.AppendFormat(itemFormat, item.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.AppendFormat(itemFormat, item.PublishTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.AppendFormat("\"{0}\"", item.ImageURL);
                        sw.WriteLine(sb.ToString());
                    }
                }
                MessageBox.Show("Export complete", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save due to error: \r\n" + ex.Message, "Export", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static SteamWorkshopItem[] GetHLAWorkshopList()
        {
            List<SteamWorkshopItem> retVal = new List<SteamWorkshopItem>();
            
            string folder = GetHLAWorkshopFolder();

            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                foreach (var dir in new DirectoryInfo(folder).GetDirectories())
                {
                    string key = dir.Name;
                    string workshopFile = Path.Combine(folder, key, WorkshopInfoFile);

                    string vpkFile = GetVPKFile(key);
                    if (File.Exists(vpkFile))
                    {
                        if (File.Exists(workshopFile))
                        {
                            var info = new SteamInfo(workshopFile);
                            var data = new SteamWorkshopItem(key, info);
                            data.LoadCompleted += Data_LoadCompleted;
                            retVal.Add(data);
                            //totalAddOns++;
                        }
                        else
                        {
                            var data = new SteamWorkshopItem(key);
                            data.LoadCompleted += Data_LoadCompleted;
                            retVal.Add(data);
                            //totalAddOns++;
                        }
                    }

                }
            }
            return retVal.ToArray();
        }
        static int totalAddOns = 0;
        private static void Data_LoadCompleted(object sender, EventArgs e)
        {
            Utility.completedAddOns++;
            if (completedAddOns >= totalAddOns)
            {
                LoadCompleted?.Invoke(null, EventArgs.Empty);
            }
        }

        public static void UpdateSettings(string GCFScapeFolder, string VRFFolder)
        {
            if (Directory.Exists(GCFScapeFolder) && File.Exists(Path.Combine(GCFScapeFolder,GCFScapeEXE)))
            {
                Configuration.Current.GCFScapeInstallLocation = GCFScapeFolder;
            }
            if (Directory.Exists(VRFFolder) && File.Exists(Path.Combine(VRFFolder, VRFEXE)))
            {
                Configuration.Current.GCFScapeInstallLocation = VRFFolder;
            }
            Configuration.Current.Save();
        }
        public static bool IsVRFInstalled()
        {
            return (!string.IsNullOrEmpty(GetVRFEXE()));
        }
        public static string GetVRFInstallFolder()
        {
            string retVal;
            if (string.IsNullOrEmpty(Configuration.Current.VRFInstallLocation))
            {
                retVal = null;
            }
            else
            {
                if (Directory.Exists(Configuration.Current.VRFInstallLocation))
                {
                    retVal = Configuration.Current.VRFInstallLocation;
                }
                else
                {
                    retVal = null;
                }
            }
            return retVal;
        }
        public static string GetVRFEXE()
        {
            string retVal;
            if (string.IsNullOrEmpty(Configuration.Current.VRFInstallLocation))
            {
                retVal = null;
            }
            else
            {
                if (Directory.Exists(Configuration.Current.VRFInstallLocation))
                {
                    string location = Path.Combine(Configuration.Current.VRFInstallLocation, VRFEXE);
                    if (File.Exists(location))
                    {
                        retVal = location;
                    }
                    else
                    {
                        retVal = null;
                    }
                }
                else
                {
                    retVal = null;
                }
            }
            return retVal;
        }
        public static string GetGCFScapeInstallFolder()
        {
            string retVal;
            if (string.IsNullOrEmpty(Configuration.Current.GCFScapeInstallLocation))
            {
                retVal = null;
            }
            else
            {
                if (Directory.Exists(Configuration.Current.GCFScapeInstallLocation))
                {

                    if (File.Exists(Path.Combine(Configuration.Current.GCFScapeInstallLocation, GCFScapeEXE)))
                    {
                        retVal = Configuration.Current.GCFScapeInstallLocation;
                    }
                    else
                    {
                        var reg = Registry.ClassesRoot.OpenSubKey(".gcf").GetValue(string.Empty, null);
                        if (reg == null)
                        {
                            retVal = null;
                        }
                        else
                        {
                            reg = Registry.ClassesRoot.OpenSubKey(reg.ToString()).OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command").GetValue(string.Empty, null);

                            if (reg == null)
                            {
                                retVal = null;
                            }
                            else
                            {
                                var command = reg.ToString().Replace("\" \"%1\"", string.Empty);
                                if (command.Length > 1)
                                {
                                    command = command.Substring(1);
                                    var fle = new FileInfo(command);
                                    if (fle.Exists)
                                    {
                                        retVal = fle.DirectoryName;
                                    }
                                    else
                                    {
                                        retVal = null;
                                    }
                                }
                                else
                                {
                                    retVal = null;
                                }
                            }
                        }
                    }
                }
                else
                {
                    retVal = null;
                }
            }

            return retVal;
        }

        public static string GetGCFScapeEXE()
        {
            string retVal;
            string path = GetGCFScapeInstallFolder();
            if (string.IsNullOrEmpty(path))
            {
                retVal = null;
            }
            else
            {
                string location = GetGCFScapeInstallFolder();
                if (Directory.Exists(location))
                {
                    string p = Path.Combine(location, GCFScapeEXE);
                    if (File.Exists(p))
                    {
                        retVal = p;
                    }
                    else
                    {
                        retVal = null;
                    }
                }
                else
                {
                    retVal = null;
                }

            }
            return retVal;
        }

        public static bool IsGCFScapeInstalled()
        {
            bool retVal;
            string exe = GetGCFScapeEXE();
            if (string.IsNullOrEmpty(exe))
            {

                retVal = false;
            }
            else
            {
                retVal = File.Exists(GetGCFScapeEXE());
            }
            return retVal;

        }
        /// <summary>
        /// Open the add on in GCFScape
        /// </summary>
        /// <param name="HLAAddOnKey">The AddOn key number only</param>
        public static bool OpenInGCFScape(string HLAAddOnKey)
        {
            LastErrorMessage = "";
            bool retVal;
            if (IsGCFScapeInstalled())
            {
                retVal = OpenInExternalApplication(HLAAddOnKey, GetGCFScapeEXE());
            }
            else
            {
                LastErrorMessage = "GCFScape is not installed or not found.";
                retVal = false;

            }
            return retVal;
        }
        private static string GetVPKFile(string HLAAddOnKey)
        {
            return Path.Combine(GetHLAWorkshopFolder(), HLAAddOnKey, HLAAddOnKey + ".vpk");
        }
        private static bool OpenInExternalApplication(string HLAAddOnKey, string applicationPath)
        {
            LastErrorMessage = "";
            bool retVal;
            try
            {
                string vpkFile = string.Format("\"{0}\"", Utility.GetVPKFile(HLAAddOnKey));
                ProcessStartInfo startInfo = new ProcessStartInfo(applicationPath, vpkFile);
                Process.Start(startInfo);
                retVal = true;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                retVal = false;
            }
            return retVal;
        }
        public static string LastErrorMessage { get; private set; }
        public static bool OpenInVRF(string HLAAddOnKey)
        {
            LastErrorMessage = "";
            bool retVal;
            if (IsVRFInstalled())
            {
                retVal = OpenInExternalApplication(HLAAddOnKey, GetVRFEXE());



                
            }
            else
            {
                LastErrorMessage = "VRF is not installed or not found.";
                retVal = false;
            }
            return retVal;
        }
        //public static bool SaveNotes(Dictionary<string, string> noteList)
        //{
        //    LastErrorMessage = "";
        //    bool retVal;
        //    try
        //    {
        //        var json = new JavaScriptSerializer().Serialize(noteList);
        //        Properties.Settings.Default.Notes = json;
        //        Properties.Settings.Default.Save();
        //        retVal = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = false;
        //        LastErrorMessage = ex.Message;
        //    }
        //    return retVal;
        //}
        //public static Dictionary<string, string> GetNotes()
        //{
        //    LastErrorMessage = "";
        //    Dictionary<string, string> retVal;
        //    try
        //    {
        //        var json = Properties.Settings.Default.Notes;
        //        if (!string.IsNullOrEmpty(json))
        //        {
        //            var noteList = new JavaScriptSerializer().Deserialize<Dictionary<string,string>>(json);
        //            retVal = noteList;
        //        }
        //        else
        //        {
        //            retVal = new Dictionary<string, string>();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LastErrorMessage = ex.Message;
        //        retVal = null;
        //    }
        //    return retVal;
        //}
    }
}
