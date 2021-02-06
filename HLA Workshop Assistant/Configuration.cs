using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HLA_Workshop_Assistant
{
    public class Configuration
    {
        static Configuration()
        {
            Initialize();
        }

        private Configuration()
        {
            Notes = new Dictionary<string, string>();
            Load();
        }
        void Load()
        {
            string gcf = Properties.Settings.Default.GCFScapeInstallLocation;
            string vrf = Properties.Settings.Default.VRFInstallLocation;
            int version = Properties.Settings.Default.ConfigVersion;
            string notes = Properties.Settings.Default.Notes;

            if (version == defaultPropertiesVersion && System.IO.File.Exists(backupConfig))
            {
                string data;
                lock (lockObject)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(backupConfig))
                    {
                        data = sr.ReadToEnd();
                    }
                }
                Load(data);
                return;
            }
            else 
            {
                GCFScapeInstallLocation = gcf;
                VRFInstallLocation = vrf;
                ConfigVersion = version;

                LoadNotes(notes);
            }
            
            
        }
        static readonly string backupConfig = System.Reflection.Assembly.GetExecutingAssembly().Location + ".configuration";
        const int defaultPropertiesVersion = 0;  //This is what is factory set.

        void Load(string data)
        {
            char vers = '0';
            if (data.Length > 0)
            {
                vers = data[0];
            }

            int i;
            if (int.TryParse(vers.ToString(), out i))
            {
                ConfigVersion = i;
            }
            else
            {
                ConfigVersion = 0;
            }
            switch (ConfigVersion)
            {
                case 0:
                case 1:
                    //nop--not possible.
                    LoadVersion1(data);
                    break;
                case 2:
                    LoadVersion2(data);
                    break;
                default:
                    LoadVersion2(data);
                    break;
            }
        }
        void LoadVersion1(string data)
        {
            //notes are JSON format.
            //Not going to happen.
            LoadNotesVersion1(data);
            
        }
        void LoadNotes(string data)
        {
            switch (ConfigVersion)
            {
                case 0:
                case 1:
                    LoadNotesVersion1(data);
                    break;
                case 2:
                    LoadNotesVersion2(data);
                    break;
                default:
                    LoadNotesVersion2(data);
                    break;
            }
        }
        void LoadNotesVersion1(string data)
        {
            //notes are JSON format.
            try
            {
                var noteList = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(data);

                foreach (var key in noteList.Keys)
                {
                    Notes.Add(key, noteList[key]);
                }
            }
            catch (Exception ex)
            {

            }
        }
        void LoadVersion2(string data)
        {
            var parms = data.Split(FS);
            if (parms.Length > 1)
            {
                GCFScapeInstallLocation = parms[1];

                if (parms.Length > 2)
                {
                    VRFInstallLocation = parms[2];

                    if (parms.Length > 3)
                    {
                        LoadNotesVersion2(parms[3]);
                    }
                }
            }
        }
        const char FS = '\x1c';
        const char GS = '\x1d';
        const char RS = '\x1e';
        const char US = '\x1f';
        void LoadNotesVersion2(string data)
        {
            DeserializeNotes(data);

        }
        static Configuration _current = new Configuration();
        public static Configuration Current
        {
            get
            {
                return _current;
            }
        }
        static void Initialize()
        {
            if (HLA_Workshop_Assistant.Properties.Settings.Default.UpgradeRequired)
            {
                HLA_Workshop_Assistant.Properties.Settings.Default.Upgrade();
                HLA_Workshop_Assistant.Properties.Settings.Default.UpgradeRequired = false;
                HLA_Workshop_Assistant.Properties.Settings.Default.Save();
            }
        }
        const int thisConfigVersion = 2;
        public int ConfigVersion { get; private set; }
        public string GCFScapeInstallLocation { get; set; }
        public string VRFInstallLocation { get; set; }

        public Dictionary<string, string> Notes { get; private set; }
        /*
         * GCFScapeInstallLocation
         * VRFInstallLocation
         * Notes  (stored as JSON).
         * */

        string SerializeNotes()
        {
            List<string> entries = new List<string>();
            foreach (var key in Notes.Keys)
            {
                entries.Add(key + GS + Notes[key]);
            }
            return string.Join(RS.ToString(), entries.ToArray());
        }
        void DeserializeNotes(string notes)
        {
            Notes.Clear();
            var entries = notes.Split(RS);
            foreach (var entry in entries)
            {
                var keyValue = entry.Split(GS);
                if (keyValue.Length > 1)
                {
                    Notes.Add(keyValue[0], keyValue[1]);
                }
            }
        }
        object lockObject = new object();
        public void Save()
        {
            HLA_Workshop_Assistant.Properties.Settings.Default.GCFScapeInstallLocation = GCFScapeInstallLocation;
            HLA_Workshop_Assistant.Properties.Settings.Default.VRFInstallLocation = VRFInstallLocation;
            var notes = SerializeNotes();
            HLA_Workshop_Assistant.Properties.Settings.Default.Notes = notes;
            HLA_Workshop_Assistant.Properties.Settings.Default.ConfigVersion = thisConfigVersion;
            HLA_Workshop_Assistant.Properties.Settings.Default.Save();

            List<string> data = new List<string>();
            data.Add(thisConfigVersion.ToString());
            data.Add(GCFScapeInstallLocation);
            data.Add(VRFInstallLocation);
            data.Add(notes);
            lock (lockObject)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(backupConfig))
                {

                    sw.WriteLine(string.Join(FS.ToString(), data.ToArray()));
                }
            }

        }
    }
}
