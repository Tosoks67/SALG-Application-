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
using static SALG__Application_.Defaults;

namespace SALG__Application_
{
    /// <summary>
    /// Logika interakcji dla klasy Format.xaml
    /// </summary>
    public partial class Format : Window
    {
        public string format { get; private set; }

        private bool saving = false;
        public Format(string f)
        {
            format = f;
            InitializeComponent();
            txtFormat.Text = format;
        }

        private void btnSaveButton_Click(object sender, RoutedEventArgs e)
        {
            saving = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!saving)
            {
                MessageBoxResult save = MessageBox.Show("Do you wish to save changes?", "Confirm", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (save == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    saving = false;
                    return;
                }
                else
                    DialogResult = save == MessageBoxResult.Yes;
            }
            if (string.IsNullOrWhiteSpace(txtFormat.Text))
            {
                MessageBoxResult result = MessageBox.Show("Your format is empty. Do you wish to use the default format instead?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                format = result == MessageBoxResult.No ? txtFormat.Text : DefaultFormat;
            }
            else
                format = txtFormat.Text;
        }
    }
}
