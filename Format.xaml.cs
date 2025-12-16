using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static SALG.Defaults;
using static SALG.FilePaths;
using static SALG.Functions;

namespace SALG;

public partial class Format
{
    public string CurrentFormat { get; private set; }
    public Prefs Preferences { get; }

    private bool _saving;
    public Format(string format, Prefs prefs)
    {
        InitializeComponent();
        CurrentFormat = format;
        Preferences = prefs;
        txtFormat.Text = CurrentFormat;

        chkDoSEnabled.IsChecked = Preferences.DoSPrefix.Enabled;
        txtDoSPrefix.Text = Preferences.DoSPrefix.Text;
        txtDoSPrefix.IsEnabled = Preferences.DoSPrefix.Enabled;

        chkTRTEnabled.IsChecked = Preferences.TrtPrefix.Enabled;
        txtTRTPrefix.Text = Preferences.TrtPrefix.Text;
        txtTRTPrefix.IsEnabled = Preferences.TrtPrefix.Enabled;

        chkA1Enabled.IsChecked = Preferences.Alpha1Prefix.Enabled;
        txtA1Prefix.Text = Preferences.Alpha1Prefix.Text;
        txtA1Prefix.IsEnabled = Preferences.Alpha1Prefix.Enabled;

        chkE11Enabled.IsChecked = Preferences.Epsilon11Prefix.Enabled;
        txtE11Prefix.Text = Preferences.Epsilon11Prefix.Text;
        txtE11Prefix.IsEnabled = Preferences.Epsilon11Prefix.Enabled;

        chkNu7Enabled.IsChecked = Preferences.Nu7Prefix.Enabled;
        txtNu7Prefix.Text = Preferences.Nu7Prefix.Text;
        txtNu7Prefix.IsEnabled = Preferences.Nu7Prefix.Enabled;
    }

    private void btnSaveButton_Click(object sender, RoutedEventArgs e)
    {
        _saving = true;
        this.Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!_saving)
        {
            var save = MessageBox.Show("Do you wish to save changes?", "Confirm", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (save == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                _saving = false;
                return;
            }
        }
        if (string.IsNullOrWhiteSpace(txtFormat.Text))
        {
            var result = MessageBox.Show("Your format is empty. Do you wish to use the default format instead?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            CurrentFormat = result == MessageBoxResult.No ? txtFormat.Text : DefaultFormat;
        }
        else
            CurrentFormat = txtFormat.Text;

        Preferences.DoSPrefix.Text = txtDoSPrefix.Text;
        Preferences.DoSPrefix.Enabled = chkDoSEnabled.IsChecked ?? true;

        Preferences.TrtPrefix.Text = txtTRTPrefix.Text;
        Preferences.TrtPrefix.Enabled = chkTRTEnabled.IsChecked ?? true;

        Preferences.Alpha1Prefix.Text = txtA1Prefix.Text;
        Preferences.Alpha1Prefix.Enabled = chkA1Enabled.IsChecked ?? true;

        Preferences.Epsilon11Prefix.Text = txtE11Prefix.Text;
        Preferences.Epsilon11Prefix.Enabled = chkE11Enabled.IsChecked ?? true;

        Preferences.Nu7Prefix.Text = txtNu7Prefix.Text;
        Preferences.Nu7Prefix.Enabled = chkNu7Enabled.IsChecked ?? true;

        DialogResult = true;
        Log(Preferences.ToString());
        Log(CurrentFormat);
        File.WriteAllText(FormatPath, CurrentFormat);
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb)
        {
            cb.CheckTextBoxEnabled();
        }
    }
}

internal static class FormatFunctions
{
    public static void CheckTextBoxEnabled(this CheckBox cb)
    {
        var parent = (Grid) cb.Parent;
        if (parent is null) return;
        parent.Children.OfType<TextBox>().First().IsEnabled = cb.IsChecked ?? true;
    }
}