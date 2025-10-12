using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
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
using static SALG__Application_.Functions;
using static SALG__Application_.FilePaths;
using static SALG__Application_.SaveHandler;
using static SALG__Application_.Defaults;

namespace SALG__Application_
{
    /// <summary>
    /// Logika interakcji dla klasy Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public UserData UData { get; private set; }
        private readonly bool TRT;
        public string notes;
        public Setup(UserData ud, bool tactResTean, string n)
        {
            UData = ud;
            TRT = tactResTean;
            notes = n;
            InitializeComponent();
            string department = TRT ? "TRT" : "DoS";
            IconFor.Source = new BitmapImage(new Uri("/" + department + "_dark.png", UriKind.Relative));
            IconBack.Source = new BitmapImage(new Uri("/" + department + "_dark.png", UriKind.Relative));
            txtUsername.Text = UData.Username;
            if (TRT)
            {
                cbxCurrentTRT.Visibility = Visibility.Visible;
                cbxCurrentTRT.Text = UData.TRTRank != TRTRank.None ? UData.TRTRank.ToString().Replace('_', ' ')[4..] : "Cadet";
            }
            else
            {
                cbxCurrentRank.Visibility = Visibility.Visible;
                cbxCurrentRank.Text = UData.Rank != Rank.None ? UData.Rank.ToString().Replace('_', ' ') : "Cadet";
            }
            numQuotaDone.Text = UData.QuotaDone.ToString();
            numTotalTime.Text = UData.TotalTime.ToString();
            numCurrentQuota.Text = UData.CurrentQuota.ToString();
            txtPermamentNote.Text = notes;
        }

        private void Save()
        {
            UData.RankString = TRT ? "TRT " + cbxCurrentTRT.Text : cbxCurrentRank.Text;
            UData.Username = txtUsername.Text;
            if (int.TryParse(numQuotaDone.Text, out int qDone))
                UData.QuotaDone = qDone;
            else
                UData.QuotaDone = 0;

            if (int.TryParse(numTotalTime.Text, out int tTime))
                UData.TotalTime = tTime;
            else
                UData.TotalTime = 0;

            if (int.TryParse(numCurrentQuota.Text, out int cQuota))
                UData.CurrentQuota = cQuota;
            else
                UData.CurrentQuota = 0;

            notes = txtPermamentNote.Text;
            SaveUserData(UData);
            File.WriteAllText(NotesPath, notes + NotesMessage);
        }

        private void InputNumCheck(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _) && e.Text != "";
        }

        private void PasteNumCheck(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = e.Command == ApplicationCommands.Paste && !int.TryParse(Clipboard.GetText(), out _);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string user = txtUsername.Text;
            string qDone = numQuotaDone.Text;
            string tTime = numTotalTime.Text;
            string cQuota = numCurrentQuota.Text;
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(qDone) || string.IsNullOrWhiteSpace(tTime) || string.IsNullOrWhiteSpace(cQuota))
            {
                e.Cancel = true;
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Save();
            DialogResult = true;
        }

        private void txtTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnSaveButton_Click(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
