using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static SALG__Application_.Defaults;
using static SALG__Application_.FilePaths;
using static SALG__Application_.Functions;
using static SALG__Application_.SaveHandler;

namespace SALG__Application_;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static readonly FrameworkElementFactory BtnFefLight = new(typeof(Border)) { Name = "border" };
    private static readonly FrameworkElementFactory BtnFefDark = new(typeof(Border)) { Name = "border" };
    private static readonly FrameworkElementFactory ContentPresenterLight = new(typeof(ContentPresenter));
    private static readonly FrameworkElementFactory ContentPresenterDark = new(typeof(ContentPresenter));
    private static readonly Storyboard ToWhiteBg = new() { Children = { new ColorAnimation { To = Color.FromRgb(255, 255, 255), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
    private static readonly Storyboard ToWhiteFg = ToWhiteBg.Clone();
    private static readonly Storyboard ToGray = new() { Children = { new ColorAnimation { To = Color.FromRgb(120, 120, 120), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
    private static readonly Storyboard ToBlackBg = new() { Children = { new ColorAnimation { To = Color.FromRgb(0, 0, 0), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
    private static readonly Storyboard ToBlackFg = ToBlackBg.Clone();
    private static Style _darkButtonStyle = new(typeof(Button));
    private static Style _lightButtonStyle = new(typeof(Button));

    private Prefs _prefs = LoadPreferences();
    private UserData _userData = LoadUserData();
    private string _format = File.Exists(FormatPath) ? File.ReadAllText(FormatPath) : DefaultFormat;
    private string _note = LoadNotes();

    private bool _isDarkMode;
    private Department _loadedDep = Department.DoS;

    private int _logQDone;
    private int _logTTime;
    public MainWindow()
    {
        Directory.CreateDirectory(AppFolder);
        StyleSetup();
        InitializeComponent();
        mitDarkMode.IsChecked = _prefs.DarkMode;
        if (!CheckRanks(_userData, out var udSafe, _prefs.Department))
        {
            Setup setup = new(udSafe, _prefs.Department, _note);
            setup.ShowDialog();
            if (setup.DialogResult ?? false)
            {
                _userData = setup.UData;
                _note = setup.Notes;
                ReloadData(true);
            }
        }
        else
        {
            ReloadData(true);
        }
        DepartmentChecker();
        DarkModeChecker();
    }

    /// <summary>
    /// Setup styles to later use in DarkModeChecker()
    /// </summary>
    private static void StyleSetup()
    {
        BtnFefLight.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
        BtnFefLight.SetBinding(Border.PaddingProperty, new Binding("Padding") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
        BtnFefLight.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
        BtnFefLight.SetValue(Border.BorderThicknessProperty, new Thickness(0.5));
        BtnFefLight.SetValue(Border.BorderBrushProperty, Brushes.Black);
        ContentPresenterLight.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        ContentPresenterLight.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        BtnFefLight.AppendChild(ContentPresenterLight);

        BtnFefDark.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
        BtnFefDark.SetBinding(Border.PaddingProperty, new Binding("Padding") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
        BtnFefDark.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
        BtnFefDark.SetValue(Border.BorderThicknessProperty, new Thickness(0.5));
        BtnFefDark.SetValue(Border.BorderBrushProperty, Brushes.White);
        ContentPresenterDark.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        ContentPresenterDark.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        BtnFefDark.AppendChild(ContentPresenterDark);

        Storyboard.SetTargetProperty(ToWhiteBg.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
        Storyboard.SetTargetProperty(ToWhiteFg.Children[0], new PropertyPath("(Foreground).(SolidColorBrush.Color)"));
        Storyboard.SetTargetProperty(ToGray.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
        Storyboard.SetTargetProperty(ToBlackBg.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
        Storyboard.SetTargetProperty(ToBlackFg.Children[0], new PropertyPath("(Foreground).(SolidColorBrush.Color)"));
        _darkButtonStyle = new(typeof(Button))
        {
            Setters =
            {
                new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.White)),
                new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.Black)),
                new Setter(Button.TemplateProperty, new ControlTemplate(typeof(Button))
                {
                    Triggers =
                    {
                        new Trigger
                        {
                            Property = Button.IsPressedProperty,
                            Value = true,
                            EnterActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = ToGray
                                }
                            },
                            ExitActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = ToBlackBg
                                }
                            }
                        },
                        new MultiTrigger
                        {
                            Conditions =
                            {
                                new Condition
                                {
                                    Property = Button.IsMouseOverProperty,
                                    Value = true
                                },
                                new Condition
                                {
                                    Property = Button.IsPressedProperty,
                                    Value = false
                                }
                            },
                            EnterActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = ToWhiteBg
                                }
                            },
                            ExitActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = ToBlackBg
                                }
                            }
                        }
                    },
                    VisualTree = BtnFefDark
                })
            },
            Triggers =
            {
                new MultiTrigger
                {
                    Conditions =
                    {
                        new Condition
                        {
                            Property = Button.IsMouseOverProperty,
                            Value = true
                        },
                        new Condition
                        {
                            Property = Button.IsPressedProperty,
                            Value = false
                        }
                    },
                    EnterActions =
                    {
                        new BeginStoryboard
                        {
                            Storyboard = ToBlackFg
                        }
                    },
                    ExitActions =
                    {
                        new BeginStoryboard
                        {
                            Storyboard = ToWhiteFg
                        }
                    }
                }
            }
        };
        _lightButtonStyle = new(typeof(Button))
        {
            Setters =
            {
                new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.Black)),
                new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.White)),
                new Setter(Button.TemplateProperty, new ControlTemplate(typeof(Button))
                {
                    Triggers =
                    {
                        new Trigger
                        {
                            Property = Button.IsPressedProperty,
                            Value = true,
                            EnterActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = ToBlackBg
                                }
                            },
                            ExitActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = ToWhiteBg
                                }
                            }
                        },
                        new MultiTrigger
                        {
                            Conditions =
                            {
                                new Condition
                                {
                                    Property = Button.IsMouseOverProperty,
                                    Value = true
                                },
                                new Condition
                                {
                                    Property = Button.IsPressedProperty,
                                    Value = false
                                }
                            },
                            EnterActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = ToGray
                                }
                            },
                            ExitActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = ToWhiteBg
                                }
                            }
                        }
                    },
                    VisualTree = BtnFefLight
                })
            },
            Triggers =
            {
                new MultiTrigger
                {
                    Conditions =
                    {
                        new Condition
                        {
                            Property = Button.IsMouseOverProperty,
                            Value = true
                        },
                        new Condition
                        {
                            Property = Button.IsPressedProperty,
                            Value = false
                        }
                    },
                    EnterActions =
                    {
                        new BeginStoryboard
                        {
                            Storyboard = ToWhiteFg
                        }
                    },
                    ExitActions =
                    {
                        new BeginStoryboard
                        {
                            Storyboard = ToBlackFg
                        }
                    }
                }
            }
        };
    }

    private void DepartmentChecker()
    {
        ReloadCheckBoxes();
        if (!CheckRanks(_userData, out var udSafe, _prefs.Department))
        {
            Setup setup = new(udSafe, _prefs.Department, _note);
            setup.ShowDialog();
            if (setup.DialogResult ?? false)
            {
                _userData = setup.UData;
                _note = setup.Notes;
                ReloadData(true);
            }
            else
            {
                _prefs.Department = _loadedDep;
                ReloadCheckBoxes();
            }
        }
            
        _loadedDep = _prefs.Department;
    }

    /// <summary>
    /// Check if Dark Mode is enabled and change styles accordingly
    /// </summary>
    private void DarkModeChecker()
    {
        var imgSource = _prefs.Department + (_prefs.DarkMode ? "_dark" : "");

        BitmapImage img = new(new Uri("/" + imgSource + ".png", UriKind.Relative));
        IconFor.Source = img;
        IconBack.Source = img;

        mitDarkMode.IsChecked = _prefs.DarkMode;
        _isDarkMode = _prefs.DarkMode;

        _ = Enum.TryParse(imgSource, out DepartmentColors colorEnum);
        var colorString = "#" + ((int)colorEnum).ToString("X").PadLeft(6, '0');
        gstStart.Color = (Color)ColorConverter.ConvertFromString(colorString);

        if (_prefs.DarkMode)
        {
            gstStop.Color = Color.FromRgb(0, 0, 0);
            Resources[typeof(Label)] = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter(ForegroundProperty, Brushes.White)
                }
            };
            Resources[typeof(MenuItem)] = new Style(typeof(MenuItem))
            {
                Setters =
                {
                    new Setter(ForegroundProperty, Brushes.White)
                }
            };
            Resources[typeof(TextBox)] = new Style(typeof(TextBox))
            {
                Setters =
                {
                    new Setter(ForegroundProperty, Brushes.White),
                    new Setter(BackgroundProperty, Brushes.Black),
                    new Setter(OpacityProperty, 0.5)
                }
            };
            Resources[typeof(Button)] = _darkButtonStyle;
        }
        else
        {
            gstStop.Color = Color.FromRgb(255, 255, 255);
            Resources[typeof(Label)] = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter(ForegroundProperty, Brushes.Black)
                }
            };
            Resources[typeof(MenuItem)] = new Style(typeof(MenuItem))
            {
                Setters =
                {
                    new Setter(ForegroundProperty, Brushes.Black)
                }
            };
            Resources[typeof(TextBox)] = new Style(typeof(TextBox))
            {
                Setters =
                {
                    new Setter(ForegroundProperty, Brushes.Black),
                    new Setter(BackgroundProperty, Brushes.White),
                    new Setter(OpacityProperty, 0.5)
                }
            };
            Resources[typeof(Button)] = _lightButtonStyle;
        }
    }

    /// <summary>
    /// Reload all data, optionally the note and
    /// <br/>additionally check dark mode and department
    /// </summary>
    private void ReloadData(bool resetNote = false)
    {
        txtUsername.Content = _userData.Username;
        txtCurrentRank.Content = _userData.RankString.PrepRankString();
        numQuotaDone.Content = _userData.QuotaDone + " / " + _userData.CurrentQuota + " minutes";
        numTotalTime.Content = _userData.TotalTime + " minute(s)";
        if (resetNote) txtNote.Text = _note;
        mitShowQuota.IsChecked = _userData.ShowQuotaDone;

            

        mitDarkMode.IsChecked = _prefs.DarkMode;

        if (_isDarkMode != _prefs.DarkMode)
            DarkModeChecker();
        if (_loadedDep != _prefs.Department)
            DepartmentChecker();
    }

    /// <summary>
    /// Reloads all the Department Checkboxes to reflect
    /// <br/>the current department in prefs.Department
    /// </summary>
    private void ReloadCheckBoxes()
    {
        mitDoS.IsChecked = _prefs.Department == Department.DoS;
        mitTRT.IsChecked = _prefs.Department == Department.TRT;
        mitE11.IsChecked = _prefs.Department == Department.Epsilon11;
        mitA1.IsChecked = _prefs.Department == Department.Alpha1;
        mitN7.IsChecked = _prefs.Department == Department.Nu7;
    }
















    private void mitSetup_Click(object sender, RoutedEventArgs e)
    {
        CheckRanks(_userData, out var udSafe, _prefs.Department);
        Setup setup = new(udSafe, _prefs.Department, _note);
        setup.ShowDialog();
        if (setup.DialogResult ?? false)
        {
            _userData = setup.UData;
            _note = setup.Notes;
            ReloadData(true);
        }
    }

    private void mitDarkMode_Click(object sender, RoutedEventArgs e)
    {
        _prefs.DarkMode = mitDarkMode.IsChecked;
        DarkModeChecker();
        SavePreferences(_prefs);
    }

    private void mitShowQuota_Click(object sender, RoutedEventArgs e)
    {
        _userData.ShowQuotaDone = mitShowQuota.IsChecked;
        ReloadData();
        SaveUserData(_userData);
    }

    private void btnGenerateLog_Click(object sender, RoutedEventArgs e)
    {
        ReloadData();
        if (!int.TryParse(numStartHour.Text, out var startHour) || !int.TryParse(numStartMinute.Text, out var startMinutes) || !int.TryParse(numEndHour.Text, out var endHour) || !int.TryParse(numEndMinute.Text, out var endMinutes)) return;
        TimeSpan startTime = new(startHour, startMinutes, 0);
        TimeSpan endTime = new(endHour, endMinutes, 0);
        if (endTime < startTime)
        {
            endTime = endTime.Add(TimeSpan.FromDays(1));
        }
        var difference = (int)(endTime - startTime).TotalMinutes;
        var tTime = _userData.TotalTime + difference;
        var qDone = _userData.QuotaDone + difference;
        _logQDone = qDone;
        _logTTime = tTime;
        txtLog.Text = GenerateLog(_prefs, _userData, startTime.ToString(@"hh\:mm"), endTime.ToString(@"hh\:mm"), difference, txtNote.Text, _format);
        grdLogGrid.Visibility = Visibility.Visible;
    }

    private void InputNumCheck(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out _) && e.Text != "";
        if (!e.Handled && sender is TextBox tBox && tBox.GetLineLength(0) >= 2)
        {
            switch (tBox.Name)
            {
                case "numStartHour":
                    numStartMinute.Focus();
                    numStartMinute.CaretIndex = 0;
                    break;
                case "numStartMinute":
                    numEndHour.Focus();
                    numEndHour.CaretIndex = 0;
                    break;
                case "numEndHour":
                    numEndMinute.Focus();
                    numEndMinute.CaretIndex = 0;
                    break;
                case "numEndMinute":
                    btnGenerateLog.Focus();
                    break;
            }
        }
    }

    private void KeyDownNumCheck(object sender, KeyEventArgs e)
    {
        if (sender is TextBox tBox)
        {
            if (e.Key == Key.Left)
            {
                if (tBox.CaretIndex > 0) return;
                e.Handled = true;
                switch (tBox.Name)
                {
                    case "numStartMinute":
                        if (tBox.CaretIndex == 0) { numStartHour.Focus(); numStartHour.CaretIndex = numStartHour.GetLineLength(0); }
                        break;
                    case "numEndHour":
                        if (tBox.CaretIndex == 0) { numStartMinute.Focus(); numStartMinute.CaretIndex = numStartMinute.GetLineLength(0); }
                        break;
                    case "numEndMinute":
                        if (tBox.CaretIndex == 0) { numEndHour.Focus(); numEndHour.CaretIndex = numEndHour.GetLineLength(0); }
                        break;
                }
            }
            else if (e.Key == Key.Right)
            {
                if (tBox.CaretIndex < tBox.GetLineLength(0)) return;
                e.Handled = true;
                switch (tBox.Name)
                {
                    case "numStartHour":
                        if (tBox.CaretIndex == tBox.GetLineLength(0)) { numStartMinute.Focus(); numStartHour.CaretIndex = 0; }
                        break;
                    case "numStartMinute":
                        if (tBox.CaretIndex == tBox.GetLineLength(0)) { numEndHour.Focus(); numEndHour.CaretIndex = 0; }
                        break;
                    case "numEndHour":
                        if (tBox.CaretIndex == tBox.GetLineLength(0)) { numEndMinute.Focus(); numStartMinute.CaretIndex = 0; }
                        break;
                }
            }
        }
    }

    private void PasteNumCheck(object sender, ExecutedRoutedEventArgs e)
    {
        e.Handled = e.Command == ApplicationCommands.Paste && !int.TryParse(Clipboard.GetText(), out _);
    }

    private async void btnCopyLog_Click(object sender, RoutedEventArgs e)
    {
        var copied = false;
        for (var i = 0; i < 3 && !copied; i++)
        {
            try
            {
                Clipboard.SetText(txtLog.Text);
                copied = true;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                await Task.Delay(100);
            }
        }
        btnCopyLog.Content = "Copied!";
        _userData.TotalTime = _logTTime;
        _userData.QuotaDone = _logQDone;
        SaveUserData(_userData);
        ReloadData();
        await Task.Delay(1000);
        btnCopyLog.Content = "Copy Log (Save Data)";
        grdLogGrid.Visibility = Visibility.Hidden;
    }

    private void btnQuotaReset_Click(object sender, RoutedEventArgs e)
    {
        _userData.QuotaDone = 0;
        SaveUserData(_userData);
        SavePreferences(_prefs);
        ReloadData();
    }

    private void mitLicense_Click(object sender, RoutedEventArgs e)
    {
        new LicWindow().ShowDialog();
    }

    private void mitAbout_Click(object sender, RoutedEventArgs e)
    {
        new AboutWindow().ShowDialog();
    }

    private void mitFormat_Click(object sender, RoutedEventArgs e)
    {
        Format f = new(_format, _prefs);
        f.ShowDialog();
        if (f.DialogResult ?? false)
        {
            _format = f.CurrentFormat;
            _prefs = f.Preferences;
        }
        SavePreferences(_prefs);
    }

    private void departmentMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem element && element.Parent is MenuItem parent)
        {
            foreach (var item in parent.Items.OfType<MenuItem>())
            {
                if (item != element) item.IsChecked = false;
            }
            element.IsChecked = true;
            _prefs.Department = element.Name switch
            {
                "mitDoS" => Department.DoS,
                "mitTRT" => Department.TRT,
                "mitA1" => Department.Alpha1,
                "mitE11" => Department.Epsilon11,
                "mitN7" => Department.Nu7,
                _ => _prefs.Department
            };
            DepartmentChecker();
            DarkModeChecker();
            SaveUserData(_userData);
            SavePreferences(_prefs);
        }
    }
}