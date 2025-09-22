using System.IO;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists("misc") || !CheckMisc(ReadMisc())) { File.WriteAllText("misc", "N|N"); }
            if (ReadMisc()[0] == "Y") { mitDarkMode.IsChecked = true; }
            if (!File.Exists("data") || !CheckUp(ReadData())) { Setup setup = new(); setup.ShowDialog(); ReloadData(); } else { ReloadData(); }
            DepartmentChecker();
            DarkModeChecker();
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

            FrameworkElementFactory btnFEF = new(typeof(Border)) { Name = "border" };
            btnFEF.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
            btnFEF.SetBinding(Border.PaddingProperty, new Binding("Padding") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
            btnFEF.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            btnFEF.SetValue(Border.BorderThicknessProperty, new Thickness(0.5));
            FrameworkElementFactory contentPresenter = new(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            btnFEF.AppendChild(contentPresenter);

            Storyboard backgroundToWhite = new() { Children = { new ColorAnimation { To = Color.FromRgb(255, 255, 255), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
            Storyboard.SetTargetProperty(backgroundToWhite.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
            Storyboard foregroundToWhite = new() { Children = { new ColorAnimation { To = Color.FromRgb(255, 255, 255), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
            Storyboard.SetTargetProperty(foregroundToWhite.Children[0], new PropertyPath("(Foreground).(SolidColorBrush.Color)"));
            Storyboard backgroundToGray = new() { Children = { new ColorAnimation { To = Color.FromRgb(120, 120, 120), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
            Storyboard.SetTargetProperty(backgroundToGray.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
            Storyboard backgroundToBlack = new() { Children = { new ColorAnimation { To = Color.FromRgb(0, 0, 0), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
            Storyboard.SetTargetProperty(backgroundToBlack.Children[0], new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
            Storyboard foregroundToBlack = new() { Children = { new ColorAnimation { To = Color.FromRgb(0, 0, 0), Duration = new Duration(TimeSpan.FromSeconds(0.2)) } } };
            Storyboard.SetTargetProperty(foregroundToBlack.Children[0], new PropertyPath("(Foreground).(SolidColorBrush.Color)"));

            Style darkButtonStyle = new(typeof(Button))
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
                            VisualTree = btnFEF
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
            Style lightButtonStyle = new(typeof(Button))
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
                            VisualTree = btnFEF
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

            if (mitDarkMode.IsChecked == true)
            {
                File.WriteAllText("misc", "Y|" + ReadMisc()[1]);
                gstStart.Color = Color.FromRgb(255, 255, 255);
                gstStop.Color = Color.FromRgb(0, 0, 0);
                btnFEF.SetValue(Border.BorderBrushProperty, Brushes.White);
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
                btnFEF.SetValue(Border.BorderBrushProperty, Brushes.Black);
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
                txtCurrentRank.Content = ReadMisc()[1] == "Y" ? rankTRT.ToString().Replace('_', ' ') : rankDoS.ToString().Replace('_', ' ');
                numQuotaDone.Content = data[2] + " / " + data[4] + " minutes";
                numTotalTime.Content = data[3] + " minute(s)";
            }
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
            string[] data = ReadData();
            if (CheckUp(data))
            {
                WriteData(data[0], data[1], data[2], data[3], data[4], sQDone);
            }
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
            
        }

        private void InputNumCheck(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
                return;
            }
            if ((sender == numStartHour || sender == numEndHour) && )
            {

            }
        }

        private void PasteNumCheck(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste && !int.TryParse(Clipboard.GetText(), out _) || Clipboard.GetText().Length > 2)
            {
                e.Handled = true;
            }
        }
    }
}