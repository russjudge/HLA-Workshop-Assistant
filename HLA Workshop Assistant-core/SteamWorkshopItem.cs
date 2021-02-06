using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using HLA_Workshop_Assistant.Wpf;
namespace HLA_Workshop_Assistant
{
    public class SteamWorkshopItem :  UIDependencyObject
    {
        public event EventHandler LoadCompleted;
        //static SteamWorkshopItem()
        //{
        //    noteList = Utility.GetNotes();
        //}
        public bool IsInitializing = true;
        public SteamWorkshopItem(string key, SteamInfo info) : base()
        {
            IsLoading = true;
            NotesSupported = (noteList != null);

            Key = key;
            IsInstalled = true;
            DateTime dt;
            _info = info;
            Title = info.GetValue("title");
            if (NotesSupported)
            {
                if (noteList.ContainsKey(key))
                {
                    Note = noteList[Key];
                }
            }
            IsInitializing = false;
            if (DateTime.TryParse(info.GetValue("publish_time_readable"), out dt))
            {
                PublishTime = dt;
            }
        
            BeginPoolInvoke(LoadWorkshopPage);
            
        }
        public SteamWorkshopItem(string key) : base()
        {
            IsLoading = true;
            NotesSupported = (noteList != null);

            Key = key;
            _info = null;
            if (NotesSupported)
            {
                if (noteList.ContainsKey(key))
                {
                    Note = noteList[Key];
                }
            }
            IsInitializing = false;
            BeginPoolInvoke(LoadWorkshopPage);
        }

        static Dictionary<string, string> noteList = Utility.GetNotes();


        string pageData = null;
        void LoadWorkshopPage(object state)
        {

            pageData = Utility.GetWorkshopWebpage(this.Key);
            if (!string.IsNullOrEmpty(pageData))
            {
                Author = Utility.ExtractAuthor(pageData);
                Description = Utility.ExtractDescription(pageData);
                if (_info == null)
                {
                    Title = Utility.ExtractTitle(pageData);
                    var times = Utility.ExtractLastUpdate(pageData);
                    if (times != null)
                    {
                        CreatedTime = times.Item1;
                        PublishTime = times.Item2;
                        Size = times.Item3;
                    }
                }
                else
                {
                    var t = Utility.ExtractLastUpdate(pageData);
                    if (t != null)
                    {
                        CreatedTime = t.Item1;
                        Size = t.Item3;
                    }
                    else
                    {

                    }
                }

                var imgURL = Utility.ExtractImageURL(pageData);
                if (string.IsNullOrEmpty(imgURL))
                {
                }
                else
                {
                    Image = CreateInstance<BitmapImageUIThreadManaged>(imgURL);
                }
            }
            IsLoading = false;
            LoadCompleted?.Invoke(this, EventArgs.Empty);
        }
        public bool IsLoading { get; private set; }



        public static readonly DependencyProperty IsInstalledProperty =
           DependencyProperty.Register(nameof(IsInstalled), typeof(bool),
           typeof(SteamWorkshopItem));

        public bool IsInstalled
        {

            get
            {
                return (bool)GetValue(IsInstalledProperty);
            }
            set
            {
                SetValue(IsInstalledProperty, value);
            }
        }


        public static readonly DependencyProperty SizeProperty =
           DependencyProperty.Register(nameof(Size), typeof(string),
           typeof(SteamWorkshopItem));

        public string Size
        {

            get
            {
                return (string)GetValue(SizeProperty);
            }
            set
            {
                SetValue(SizeProperty, value);
            }
        }

        SteamInfo _info = null;

        public static readonly DependencyProperty ImageProperty =
           DependencyProperty.Register(nameof(Image), typeof(BitmapImageUIThreadManaged),
           typeof(SteamWorkshopItem));

        public BitmapImageUIThreadManaged Image
        {

            get
            {
                return (BitmapImageUIThreadManaged)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }

        public static readonly DependencyProperty NotesSupportedProperty =
           DependencyProperty.Register(nameof(NotesSupported), typeof(bool),
           typeof(SteamWorkshopItem));

        public bool NotesSupported
        {

            get
            {
                return (bool)GetValue(NotesSupportedProperty);
            }
            set
            {
                SetValue(NotesSupportedProperty, value);
            }
        }

        public static readonly DependencyProperty AuthorProperty =
           DependencyProperty.Register(nameof(Author), typeof(string),
           typeof(SteamWorkshopItem));

        public string Author
        {
            get
            {
                return (string)GetValue(AuthorProperty);
            }
            set
            {
                SetValue(AuthorProperty, value);
            }
        }


        public static readonly DependencyProperty DescriptionProperty =
           DependencyProperty.Register(nameof(Description), typeof(string),
           typeof(SteamWorkshopItem));

        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }




        public static readonly DependencyProperty KeyProperty =
           DependencyProperty.Register(nameof(Key), typeof(string),
           typeof(SteamWorkshopItem));

        public string Key
        {
            get
            {
                return (string)GetValue(KeyProperty);
            }
            set
            {
                SetValue(KeyProperty, value);
            }
        }




        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.Register(nameof(Title), typeof(string),
           typeof(SteamWorkshopItem));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }



        public static readonly DependencyProperty PublishTimeProperty =
           DependencyProperty.Register(nameof(PublishTime), typeof(DateTime),
           typeof(SteamWorkshopItem));

        public DateTime PublishTime
        {
            get
            {
                return (DateTime)GetValue(PublishTimeProperty);
            }
            set
            {
                SetValue(PublishTimeProperty, value);
            }
        }

        public static readonly DependencyProperty CreatedTimeProperty =
          DependencyProperty.Register(nameof(CreatedTime), typeof(DateTime),
          typeof(SteamWorkshopItem));

        public DateTime CreatedTime
        {
            get
            {
                return (DateTime)GetValue(CreatedTimeProperty);
            }
            set
            {
                SetValue(CreatedTimeProperty, value);
            }
        }


        public static readonly DependencyProperty NoteProperty =
           DependencyProperty.Register(nameof(Note), typeof(string),
           typeof(SteamWorkshopItem), new PropertyMetadata(OnNoteChanged));

        private static void OnNoteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SteamWorkshopItem me = d as SteamWorkshopItem;
            if (me != null)
            {
                if (!me.IsInitializing)
                {
                    if (noteList != null)
                    {
                        if (noteList.ContainsKey(me.Key))
                        {
                            noteList.Add(me.Key, me.Note);
                            Utility.SaveNotes(noteList);

                        }
                        else
                        {
                            noteList[me.Key] = me.Note;
                            Utility.SaveNotes(noteList);
                        }
                    }
                }
            }
        }

        public string Note
        {
            get
            {
                return (string)GetValue(NoteProperty);
            }
            set
            {
                SetValue(NoteProperty, value);
            }
        }
    }
}
