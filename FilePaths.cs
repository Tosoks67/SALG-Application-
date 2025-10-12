using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SALG__Application_
{
    /// <summary>
    /// self-explanatory static class containing file paths used in the application
    /// </summary>
    public static class FilePaths
    {
        public static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SALG");
        public static readonly string BackupFolder = Path.Combine(AppFolder, "backup");
        public static readonly string DataPath = Path.Combine(AppFolder, "data.json");
        public static readonly string PrefsPath = Path.Combine(AppFolder, "prefs.json");
        public static readonly string FormatPath = Path.Combine(AppFolder, "format.txt");
        public static readonly string NotesPath = Path.Combine(AppFolder, "notes.txt");
    }
}
