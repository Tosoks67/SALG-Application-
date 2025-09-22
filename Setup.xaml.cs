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

namespace SALG__Application_
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public Setup()
        {
            InitializeComponent();
            string department = ReadMisc()[1] == "Y" ? "TRT" : "DoS";
            IconFor.Source = new BitmapImage(new Uri("/" + department + "_dark.png", UriKind.Relative));
            IconBack.Source = new BitmapImage(new Uri("/" + department + "_dark.png", UriKind.Relative));
            if (File.Exists("data"))
            {
                string[] data = ReadData();
                if (CheckUp(data))
                {
                    txtUsername.Text = data[0];
                    RankStringToEnum(data[1].Replace(' ', '_'), out Rank rankDoS, Rank.Cadet);
                    TRTStringToEnum(data[1].Replace(' ', '_'), out TRTRank rankTRT, TRTRank.Cadet);
                    if (ReadMisc()[1] == "Y")
                    {
                        cbxCurrentTRT.Visibility = Visibility.Visible;
                        cbxCurrentTRT.Text = rankTRT.ToString().Replace('_', ' ');
                    }
                    else
                    {
                        cbxCurrentRank.Visibility = Visibility.Visible;
                        cbxCurrentRank.Text = rankDoS.ToString().Replace('_', ' ');
                    }
                    numQuotaDone.Text = data[2];
                    numTotalTime.Text = data[3];
                    numCurrentQuota.Text = data[4];
                    txtPermamentNote.Text = File.Exists("notes") ? File.ReadAllText("notes") : "";
                }
            }
        }

        private void InputNumCheck(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void PasteNumCheck(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste && !int.TryParse(Clipboard.GetText(), out _))
            {
                e.Handled = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnSaveButton_Click(Object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text;
            string rank = ReadMisc()[1] == "Y" ? cbxCurrentTRT.Text : cbxCurrentRank.Text;
            string qDone = numQuotaDone.Text;
            string tTime = numTotalTime.Text;
            string cQuota = numCurrentQuota.Text;
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(rank) || string.IsNullOrWhiteSpace(qDone) || string.IsNullOrWhiteSpace(tTime) || string.IsNullOrWhiteSpace(cQuota))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string sQDone = ReadData().Length == 6 ? ReadData()[5] : "Y";
            WriteData(user, rank, qDone, tTime, cQuota, sQDone);
            File.WriteAllText("notes", txtPermamentNote.Text);
            this.Close();
        }
    }
}
