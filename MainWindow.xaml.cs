using System.IO;
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
using static SALG__Application_.Functions;

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
        public MainWindow()
        {
            StyleSetup();
            InitializeComponent();
            File.Delete("log.tmp");
            if (!File.Exists("misc") || !CheckMisc(ReadMisc())) { File.WriteAllText("misc", "N|N"); }
            if (ReadMisc()[0] == "Y") { mitDarkMode.IsChecked = true; }
            if (!File.Exists("data") || !CheckUp(ReadData())) { Setup setup = new(); setup.ShowDialog(); ReloadData(); } else { ReloadData(); }
            DepartmentChecker();
            DarkModeChecker();
        }

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
            if (ReadMisc()[1] == "Y")
            {
                mitTRT.IsChecked = true;
                TRTStringToEnum(ReadData()[1].Replace(' ', '_'), out TRTRank rankTRT);
                if (rankTRT == TRTRank.None)
                {
                    Setup setup = new();
                    setup.ShowDialog();
                    ReloadData();
                }
            }
            else
            {
                mitTRT.IsChecked = false;
                RankStringToEnum(ReadData()[1].Replace(' ', '_'), out Rank rankDos);
                if (rankDos == Rank.None)
                {
                    Setup setup = new();
                    setup.ShowDialog();
                    ReloadData();
                }
            }
        }

        private void DarkModeChecker()
        {
            string department = ReadMisc()[1] == "Y" ? "TRT" : "DoS";

            if (mitDarkMode.IsChecked == true)
            {
                File.WriteAllText("misc", "Y|" + ReadMisc()[1]);
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
                IconFor.Source = new BitmapImage(new Uri("/" + department + "_dark.png", UriKind.Relative));
                IconBack.Source = new BitmapImage(new Uri("/" + department + "_dark.png", UriKind.Relative));
            }
            else
            {
                File.WriteAllText("misc", "N|" + ReadMisc()[1]);
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
                IconFor.Source = new BitmapImage(new Uri("/" + department + ".png", UriKind.Relative));
                IconBack.Source = new BitmapImage(new Uri("/" + department + ".png", UriKind.Relative));

            }
        }

        private void ReloadData()
        {
            string[] data = ReadData();
            if (CheckUp(data))
            {
                txtUsername.Content = data[0];
                RankStringToEnum(data[1].Replace(' ', '_'), out Rank rankDoS);
                TRTStringToEnum(data[1].Replace(' ', '_'), out TRTRank rankTRT);
                txtCurrentRank.Content = ReadMisc()[1] == "Y" ? rankTRT.ToString().Replace('_', ' ').Substring(4) : rankDoS.ToString().Replace('_', ' ');
                numQuotaDone.Content = data[2] + " / " + data[4] + " minutes";
                numTotalTime.Content = data[3] + " minute(s)";
                txtNote.Text = File.Exists("notes") ? File.ReadAllText("notes") : "";
                mitShowQuota.IsChecked = data[5] == "Y";
            }
        }

        private static string GenerateLog(string username, string rank, string sTime, string eTime, string qDone, string tOnSite, string tTime, string quota, string showQDone, string notes = "")
        {
            RankStringToEnum(rank.Replace(' ', '_'), out Rank rankDoS);
            TRTStringToEnum(rank.Replace(' ', '_'), out TRTRank rankTRT);
            rank = rankDoS != Rank.None ? "Security " + rank : rankTRT != TRTRank.None ? "Tactical Response " + rank.Substring(4) : rank;

            showQDone = (showQDone.ToUpper() == "Y") ? "\r\n-# Quota: " + qDone + " / " + quota : "";

            notes = (notes != "") ? "\r\n**Note(s): **" + notes : "";

            return 
                "**Username: **" + username + 
                "\r\n**Rank: **" + rank + 
                "\r\n**Start Time: **" + sTime + 
                "\r\n**End time: **" + eTime + 
                "\r\n**Total time on-site: **" + tOnSite + " minutes" +
                "\r\n**Total time: **" + tTime + " minutes" + 
                showQDone + 
                "\r\n__**Evidence: **__" + 
                notes;
        }



        private void mitSetup_Click(object sender, RoutedEventArgs e)
        {
            Setup setup = new();
            setup.ShowDialog();
            ReloadData();
        }

        private void mitDarkMode_Click(object sender, RoutedEventArgs e)
        {
            DarkModeChecker();
        }

        private void mitShowQuota_Click(object sender, RoutedEventArgs e)
        {
            string sQDone = mitShowQuota.IsChecked == true ? "Y" : "N";
            string[] data = GetAndCheck();
            WriteData(data[0], data[1], data[2], data[3], data[4], sQDone);
            ReloadData();
        }

        private void mitTRT_Click(object sender, RoutedEventArgs e)
        {
            if (ReadMisc()[1] == "Y")
            {
                File.WriteAllText("misc", ReadMisc()[0] + "|N");
                DepartmentChecker();
                DarkModeChecker();
            }
            else
            {
                File.WriteAllText("misc", ReadMisc()[0] + "|Y");
                DepartmentChecker();
                DarkModeChecker();
            }
        }

        private void btnGenerateLog_Click(object sender, RoutedEventArgs e)
        {
            string[] data = GetAndCheck();
            ReloadData();
            string sTime = numStartHour.Text.PadLeft(2, '0') + ":" + numStartMinute.Text.PadLeft(2, '0');
            string eTime = numEndHour.Text.PadLeft(2, '0') + ":" + numEndMinute.Text.PadLeft(2, '0');
            TimeSpan startTime = new TimeSpan(Convert.ToInt32(numStartHour.Text), Convert.ToInt32(numStartMinute.Text), 0);
            TimeSpan endTime = new TimeSpan(Convert.ToInt32(numEndHour.Text), Convert.ToInt32(numEndMinute.Text), 0);
            if (endTime < startTime)
            {
                endTime = endTime.Add(TimeSpan.FromDays(1));
            }
            int difference = (int)(endTime - startTime).TotalMinutes;
            string tTime = (Convert.ToInt32(data[3]) + difference).ToString();
            string qDone = (Convert.ToInt32(data[2]) + difference).ToString();
            File.Delete("log.tmp");
            File.WriteAllText("log.tmp", tTime + "\n" + qDone);
            File.SetAttributes("log.tmp", FileAttributes.Hidden);
            txtLog.Text = GenerateLog(data[0], data[1], sTime, eTime, qDone, difference.ToString(), tTime, data[4], data[5], txtNote.Text);
            grdLogGrid.Visibility = Visibility.Visible;
        }

        private void InputNumCheck(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
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
                    if (tBox.CaretIndex > 0) { tBox.CaretIndex -= 1; return; }
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
                    if (tBox.CaretIndex < tBox.GetLineLength(0)) { tBox.CaretIndex += 1; return; }
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
                else if (e.Key == Key.Back && tBox.CaretIndex > 0 && tBox.Text.Length > 0)
                {
                    tBox.Text = tBox.Text.Remove(tBox.CaretIndex - 1, 1);
                    tBox.CaretIndex -= tBox.CaretIndex - 1;
                    e.Handled = true;
                }
            }
        }

        private void PasteNumCheck(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste && !int.TryParse(Clipboard.GetText(), out _) || Clipboard.GetText().Length > 2)
            {
                e.Handled = true;
            }
        }

        private async void btnCopyLog_Click(object sender, RoutedEventArgs e)
        {
            /* syntax:
                    logData[0] = Total Time
                    logData[1] = Quota Done
             */
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
            string[] data = GetAndCheck();
            string[] logData = File.ReadAllLines("log.tmp");
            WriteData(data[0], data[1], logData[1], logData[0], data[4], data[5]);
            ReloadData();
            await Task.Delay(1000);
            btnCopyLog.Content = "Copy Log (Save Data)";
            File.Delete("log.tmp");
            grdLogGrid.Visibility = Visibility.Hidden;
        }

        private void btnQuotaReset_Click(object sender, RoutedEventArgs e)
        {
            string[] data = GetAndCheck();
            WriteData(data[0], data[1], "0", data[3], data[4], data[5]);
            ReloadData();
        }

        private void Window_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.Delete("log.tmp");
        }

        private void mitLicense_Click(object sender, RoutedEventArgs e)
        {
            LicWindow license = new();
            license.ShowDialog();
        }
    }
}
