using System.IO;
using System.Reflection;

namespace SALG__Application_;

public partial class LicWindow
{
    public LicWindow()
    {
        InitializeComponent();
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("SALG__Application_.LICENSE.txt");
        if (stream is null) return;
        using StreamReader reader = new(stream);
        txtLicense.Text = reader.ReadToEnd();
    }
}