using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static SALG.Defaults;
using static SALG.FilePaths;
using static SALG.SaveHandler;

namespace SALG;

/// <summary>
/// Logika interakcji dla klasy Setup.xaml
/// </summary>
public partial class Setup
{
    public UserData UData { get; }
    private readonly Department _currentDepartment;
    public string Notes { get; private set; }
    public Setup(UserData dat, Department dep, string not)
    {
        InitializeComponent();
        UData = dat;
        _currentDepartment = dep;
        Notes = not;
        IconFor.Source = new BitmapImage(new Uri("/" + _currentDepartment + "_dark.png", UriKind.Relative));
        IconBack.Source = new BitmapImage(new Uri("/" + _currentDepartment + "_dark.png", UriKind.Relative));
        txtUsername.Text = UData.Username;
        switch (_currentDepartment)
        {
            case Department.DoS:
                cbxCurrentDoS.Visibility = Visibility.Visible;
                cbxCurrentDoS.Text = UData.DoS.ToString().PrepRankString();
                break;
            case Department.TRT:
                cbxCurrentTRT.Visibility = Visibility.Visible;
                cbxCurrentTRT.Text = UData.Trt.ToString().PrepRankString();
                break;
            case Department.Alpha1:
                cbxCurrentA1.Visibility = Visibility.Visible;
                cbxCurrentA1.Text = UData.Alpha1.ToString().PrepRankString();
                break;
            case Department.Epsilon11:
                cbxCurrentE11.Visibility = Visibility.Visible;
                cbxCurrentE11.Text = UData.Epsilon11.ToString().PrepRankString();
                break;
            case Department.Nu7:
                cbxCurrentN7.Visibility = Visibility.Visible;
                cbxCurrentN7.Text = UData.Nu7.ToString().PrepRankString();
                break;
        }
        numQuotaDone.Text = UData.QuotaDone.ToString();
        numTotalTime.Text = UData.TotalTime.ToString();
        numCurrentQuota.Text = UData.CurrentQuota.ToString();
        txtPermanentNote.Text = Notes;
    }

    private void Save()
    {
        UData.RankString = _currentDepartment switch
        {
            Department.DoS => cbxCurrentDoS.Text.StringToRankName(_currentDepartment),
            Department.TRT => cbxCurrentTRT.Text.StringToRankName(_currentDepartment),
            Department.Alpha1 => cbxCurrentA1.Text.StringToRankName(_currentDepartment),
            Department.Epsilon11 => cbxCurrentE11.Text.StringToRankName(_currentDepartment),
            Department.Nu7 => cbxCurrentN7.Text.StringToRankName(_currentDepartment),
            _ => "None"
        };
        UData.Username = txtUsername.Text;
        UData.QuotaDone = int.TryParse(numQuotaDone.Text, out var qDone) ? qDone : 0;
        UData.TotalTime = int.TryParse(numTotalTime.Text, out var tTime) ? tTime : 0;
        UData.CurrentQuota = int.TryParse(numCurrentQuota.Text, out var cQuota) ? cQuota : 0;
        Notes = txtPermanentNote.Text;
        SaveUserData(UData);
        File.WriteAllText(NotesPath, Notes + NotesMessage);
    }

    private void InputNumCheck(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out _) && e.Text != "";
    }

    private void PasteNumCheck(object sender, ExecutedRoutedEventArgs e)
    {
        e.Handled = e.Command == ApplicationCommands.Paste && !int.TryParse(Clipboard.GetText(), out _);
    }

    private void btnClose_Click(object sender, RoutedEventArgs e) => Close();

    private void Window_Close(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
            string.IsNullOrWhiteSpace(numQuotaDone.Text) ||
            string.IsNullOrWhiteSpace(numTotalTime.Text) ||
            string.IsNullOrWhiteSpace(numCurrentQuota.Text))
        {
            e.Cancel = true;
            MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        Save();
        DialogResult = true;
    }

    private void txtTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void btnSaveButton_Click(object sender, RoutedEventArgs e) => Close();
}