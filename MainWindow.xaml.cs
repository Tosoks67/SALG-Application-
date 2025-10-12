using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SALG__Application_.FilePaths;
using static SALG__Application_.Functions;
using static SALG__Application_.SaveHandler;
using static SALG__Application_.Defaults;

namespace SALG__Application_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FrameworkElementFactory btnFEFLight = new(typeof(Border)) { Name = "border" };
        private FrameworkElementFactory btnFEFDark = new(typeof(Border)) { Name = "border" };
        private FrameworkElementFactory contentPresenterLight = new(typeof(ContentPresenter));
        private FrameworkElementFactory contentPresenterDark = new(typeof(ContentPresenter));
        private Storyboard backgroundToWhite = new() { Children = { new ColorAnimation { To = Color.FromRgb(255, 255, 255), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
        private Storyboard foregroundToWhite = new() { Children = { new ColorAnimation { To = Color.FromRgb(255, 255, 255), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
        private Storyboard backgroundToGray = new() { Children = { new ColorAnimation { To = Color.FromRgb(120, 120, 120), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
        private Storyboard backgroundToBlack = new() { Children = { new ColorAnimation { To = Color.FromRgb(0, 0, 0), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
        private Storyboard foregroundToBlack = new() { Children = { new ColorAnimation { To = Color.FromRgb(0, 0, 0), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
        private Style darkButtonStyle = new(typeof(Button));
        private Style lightButtonStyle = new(typeof(Button));

        private Prefs prefs = LoadPreferences();
        private UserData userData = LoadUserData();
        private string format = File.Exists(FormatPath) ? File.ReadAllText(FormatPath) : DefaultFormat;
        private string note = LoadNotes();

        private bool isDarkMode = false;
        private bool isTRT = false;

        private int log_qDone = 0;
        private int log_tTime = 0;
        public MainWindow()
        {
            Directory.CreateDirectory(AppFolder);
            StyleSetup();
            InitializeComponent();
            mitDarkMode.IsChecked = prefs.DarkMode;
            if (!CheckRanks(userData, out UserData udSafe, prefs.TRT))
            {
                Setup setup = new(udSafe, prefs.TRT, note);
                setup.ShowDialog();
                if (setup.DialogResult ?? false)
                {
                    userData = setup.UData;
                    note = setup.notes;
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
        private void StyleSetup()
        {
            btnFEFLight.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
            btnFEFLight.SetBinding(Border.PaddingProperty, new Binding("Padding") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
            btnFEFLight.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            btnFEFLight.SetValue(Border.BorderThicknessProperty, new Thickness(0.5));
            btnFEFLight.SetValue(Border.BorderBrushProperty, Brushes.Black);
            contentPresenterLight.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterLight.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            btnFEFLight.AppendChild(contentPresenterLight);

            btnFEFDark.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
            btnFEFDark.SetBinding(Border.PaddingProperty, new Binding("Padding") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
            btnFEFDark.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            btnFEFDark.SetValue(Border.BorderThicknessProperty, new Thickness(0.5));
            btnFEFDark.SetValue(Border.BorderBrushProperty, Brushes.White);
            contentPresenterDark.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterDark.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            btnFEFDark.AppendChild(contentPresenterDark);

            Storyboard.SetTargetProperty(backgroundToWhite.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
            Storyboard.SetTargetProperty(foregroundToWhite.Children[0], new PropertyPath("(Foreground).(SolidColorBrush.Color)"));
            Storyboard.SetTargetProperty(backgroundToGray.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
            Storyboard.SetTargetProperty(backgroundToBlack.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
            Storyboard.SetTargetProperty(foregroundToBlack.Children[0], new PropertyPath("(Foreground).(SolidColorBrush.Color)"));
            darkButtonStyle = new(typeof(Button))
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
                                            Storyboard = backgroundToGray
                                        }
                                    },
                                    ExitActions =
                                    {
                                        new BeginStoryboard
                                        {
                                            Storyboard = backgroundToBlack
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
                                            Storyboard = backgroundToWhite
                                        }
                                    },
                                    ExitActions =
                                    {
                                        new BeginStoryboard
                                        {
                                            Storyboard = backgroundToBlack
                                        }
                                    }
                                }
                            },
                            VisualTree = btnFEFDark
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
                                    Storyboard = foregroundToBlack
                                }
                            },
                            ExitActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = foregroundToWhite
                                }
                            }
                        }
                    }
            };
            lightButtonStyle = new(typeof(Button))
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
                                            Storyboard = backgroundToBlack
                                        }
                                    },
                                    ExitActions =
                                    {
                                        new BeginStoryboard
                                        {
                                            Storyboard = backgroundToWhite
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
                                            Storyboard = backgroundToGray
                                        }
                                    },
                                    ExitActions =
                                    {
                                        new BeginStoryboard
                                        {
                                            Storyboard = backgroundToWhite
                                        }
                                    }
                                }
                            },
                            VisualTree = btnFEFLight
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
                                    Storyboard = foregroundToWhite
                                }
                            },
                            ExitActions =
                            {
                                new BeginStoryboard
                                {
                                    Storyboard = foregroundToBlack
                                }
                            }
                        }
                    }
            };
        }

        private void DepartmentChecker()
        {
            mitTRT.IsChecked = prefs.TRT;
            if ((prefs.TRT && userData.TRTRank == TRTRank.None) || (!prefs.TRT && userData.Rank == Rank.None))
            {
                CheckRanks(userData, out UserData udSafe, prefs.TRT);
                Setup setup = new(udSafe, prefs.TRT, note);
                setup.ShowDialog();
                if (setup.DialogResult ?? false)
                {
                    userData = setup.UData;
                    note = setup.notes;
                    ReloadData(true);
                } else
                {
                    prefs.TRT = isTRT;
                    mitTRT.IsChecked = prefs.TRT;
                }
            }
            isTRT = prefs.TRT;
        }

        /// <summary>
        /// Check if Dark Mode is enabled and change styles accordingly
        /// </summary>
        private void DarkModeChecker()
        {
            string imgSource = prefs.TRT ? "TRT" : "DoS";
            imgSource = prefs.DarkMode ? imgSource + "_dark.png" : imgSource + ".png";

            IconFor.Source = new BitmapImage(new Uri("/" + imgSource, UriKind.Relative));
            IconBack.Source = new BitmapImage(new Uri("/" + imgSource, UriKind.Relative));

            mitDarkMode.IsChecked = prefs.DarkMode;
            isDarkMode = prefs.DarkMode;

            if (mitDarkMode.IsChecked)
            {
                gstStart.Color = Color.FromRgb(255, 255, 255);
                gstStop.Color = Color.FromRgb(0, 0, 0);
                this.Resources[typeof(Label)] = new Style(typeof(Label))
                {
                    Setters =
                    {
                        new Setter(Label.ForegroundProperty, Brushes.White)
                    }
                };
                this.Resources[typeof(MenuItem)] = new Style(typeof(MenuItem))
                {
                    Setters =
                    {
                        new Setter(MenuItem.ForegroundProperty, Brushes.White)
                    }
                };
                this.Resources[typeof(TextBox)] = new Style(typeof(TextBox))
                {
                    Setters =
                    {
                        new Setter(TextBox.ForegroundProperty, Brushes.White),
                        new Setter(TextBox.BackgroundProperty, Brushes.Black),
                        new Setter(TextBox.OpacityProperty, 0.5)
                    }
                };
                this.Resources[typeof(Button)] = darkButtonStyle;
            }
            else
            {
                gstStart.Color = Color.FromRgb(84, 84, 84);
                gstStop.Color = Color.FromRgb(255, 255, 255);
                this.Resources[typeof(Label)] = new Style(typeof(Label))
                {
                    Setters =
                    {
                        new Setter(Label.ForegroundProperty, Brushes.Black)
                    }
                };
                this.Resources[typeof(MenuItem)] = new Style(typeof(MenuItem))
                {
                    Setters =
                    {
                        new Setter(MenuItem.ForegroundProperty, Brushes.Black)
                    }
                };
                this.Resources[typeof(TextBox)] = new Style(typeof(TextBox))
                {
                    Setters =
                    {
                        new Setter(TextBox.ForegroundProperty, Brushes.Black),
                        new Setter(TextBox.BackgroundProperty, Brushes.White),
                        new Setter(TextBox.OpacityProperty, 0.5)
                    }
                };
                this.Resources[typeof(Button)] = lightButtonStyle;
            }
        }

        /// <summary>
        /// Reload all data, optionally the note and additionally check dark mode and department
        /// </summary>
        private void ReloadData(bool resetNote = false)
        {
            txtUsername.Content = userData.Username;
            txtCurrentRank.Content = prefs.TRT ? userData.TRTRank.ToString().Replace('_', ' ').Substring(4) : userData.Rank.ToString().Replace('_', ' ');
            numQuotaDone.Content = userData.QuotaDone + " / " + userData.CurrentQuota + " minutes";
            numTotalTime.Content = userData.TotalTime + " minute(s)";
            if (resetNote) txtNote.Text = note;
            mitShowQuota.IsChecked = userData.ShowQuotaDone;

            mitTRT.IsChecked = prefs.TRT;
            mitDarkMode.IsChecked = prefs.DarkMode;

            if (isDarkMode != prefs.DarkMode)
                DarkModeChecker();
            if (isTRT != prefs.TRT)
                DepartmentChecker();
        }
















        private void mitSetup_Click(object sender, RoutedEventArgs e)
        {
            CheckRanks(userData, out UserData udSafe, prefs.TRT);
            Setup setup = new(udSafe, prefs.TRT, note);
            setup.ShowDialog();
            if (setup.DialogResult ?? false)
            {
                userData = setup.UData;
                note = setup.notes;
                ReloadData(true);
            }
        }

        private void mitDarkMode_Click(object sender, RoutedEventArgs e)
        {
            prefs.DarkMode = mitDarkMode.IsChecked;
            DarkModeChecker();
            SavePreferences(prefs);
        }

        private void mitShowQuota_Click(object sender, RoutedEventArgs e)
        {
            userData.ShowQuotaDone = mitShowQuota.IsChecked;
            ReloadData();
            SaveUserData(userData);
        }

        private void mitTRT_Click(object sender, RoutedEventArgs e)
        {
            prefs.TRT = mitTRT.IsChecked;
            DepartmentChecker();
            DarkModeChecker();
            SaveUserData(userData);
            SavePreferences(prefs);
        }

        private void btnGenerateLog_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
            if (!int.TryParse(numStartHour.Text, out int startHour) || !int.TryParse(numStartMinute.Text, out int startMinutes) || !int.TryParse(numEndHour.Text, out int endHour) || !int.TryParse(numEndMinute.Text, out int endMinutes)) return;
            TimeSpan startTime = new TimeSpan(startHour, startMinutes, 0);
            TimeSpan endTime = new TimeSpan(endHour, endMinutes, 0);
            if (endTime < startTime)
            {
                endTime = endTime.Add(TimeSpan.FromDays(1));
            }
            int difference = (int)(endTime - startTime).TotalMinutes;
            int tTime = userData.TotalTime + difference;
            int qDone = userData.QuotaDone + difference;
            log_qDone = qDone;
            log_tTime = tTime;
            txtLog.Text = GenerateLog(userData, startTime.ToString(@"hh\:mm"), endTime.ToString(@"hh\:mm"), difference, txtNote.Text, format);
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
            bool copied = false;
            for (int i = 0; i < 3 && !copied; i++)
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
            userData.TotalTime = log_tTime;
            userData.QuotaDone = log_qDone;
            ReloadData();
            await Task.Delay(1000);
            btnCopyLog.Content = "Copy Log (Save Data)";
            grdLogGrid.Visibility = Visibility.Hidden;
        }

        private void btnQuotaReset_Click(object sender, RoutedEventArgs e)
        {
            userData.QuotaDone = 0;
            SaveUserData(userData);
            SavePreferences(prefs);
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
            Format f = new(format);
            f.ShowDialog();
            if (f.DialogResult ?? false)
                format = f.format;
        }
    }
}
