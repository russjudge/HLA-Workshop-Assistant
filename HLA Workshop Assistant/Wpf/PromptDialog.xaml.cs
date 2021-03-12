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
    /// Interaction logic for PromptDialog.xaml
    /// </summary>
    public sealed partial class PromptDialog : Window
    {

        public static string ShowPrompt(string title, string prompt)
        {
            string retVal;
            PromptDialog diag = new PromptDialog();
            diag.Title = title;
            diag.Label = prompt;
            if (diag.ShowDialog() == true)
            {
                retVal = diag.Text;
            }
            else
            {
                retVal = null;
            }
            return retVal;
        }
        public PromptDialog()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
            typeof(PromptDialog));

        public string Label
        {
            get
            {
                return (string)this.GetValue(LabelProperty);
            }
            set
            {
                this.SetValue(LabelProperty, value);
            }
        }


        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
            typeof(PromptDialog));

        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
