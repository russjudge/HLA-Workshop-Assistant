using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HLA_Workshop_Assistant.Wpf
{
    /// <summary>
    /// Allows for the creation of a BitmapImage without blocking, thus the image is created asynchronously, but still on the UI thread.
    /// </summary>
    public sealed class BitmapImageUIThreadManaged : UIDependencyObject
    {
        public BitmapImageUIThreadManaged() { }
        public BitmapImageUIThreadManaged(Uri uriSource)
        {
            BeginImageRetrieval(uriSource);
        }
        public BitmapImageUIThreadManaged(string Source)
        {
            BeginImageRetrieval(Source);
        }
        public void BeginImageRetrieval(string path)
        {
            BeginImageRetrieval(new Uri(path));
        }

        public void BeginImageRetrieval(Uri uriSource)
        {
            BeginPoolInvoke(LoadImage, uriSource);
        }
        void LoadImage(object source)
        {

            Uri uriSource = (Uri)source;
            RequestCachePolicy policy =
                new RequestCachePolicy(RequestCacheLevel.Revalidate);

            BeginInvoke(new Action(() => { ImageSource = new BitmapImage(uriSource, policy); }));
        }


        public static readonly DependencyProperty ImageSourceProperty =
          DependencyProperty.Register(nameof(ImageSource), typeof(BitmapImage),
          typeof(BitmapImageUIThreadManaged));


        public BitmapImage ImageSource
        {
            get
            {
                return (BitmapImage)GetValue(ImageSourceProperty);
            }
            private set
            {
                BeginSetValue(ImageSourceProperty, value);
            }
        }

    }
}
