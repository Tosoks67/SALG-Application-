using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace SALG__Application_
{
    /// <summary>
    /// Logika interakcji dla klasy License.xaml
    /// </summary>
    public partial class LicWindow : Window
    {
        public LicWindow()
        {
            InitializeComponent();
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream("SALG__Application_.LICENSE.txt");
            if (stream != null)
            {
                using StreamReader reader = new(stream);
                txtLicense.Text = reader.ReadToEnd();
            }
        }
    }
}
