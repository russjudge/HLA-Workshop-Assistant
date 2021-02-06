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

namespace HLA_Workshop_Assistant
{
    /// <summary>
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        public NoteWindow()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty NoteProperty =
            DependencyProperty.Register(nameof(Note), typeof(string),
            typeof(NoteWindow));
        public string Note
        {
            get
            {
                return (string)GetValue(NoteProperty);
            }
            set
            {
                this.SetValue(NoteProperty, value);
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
