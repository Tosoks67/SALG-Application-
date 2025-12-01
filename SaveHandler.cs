using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using static SALG__Application_.Defaults;
using static SALG__Application_.FilePaths;

namespace SALG__Application_;

public static class SaveHandler
{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Load user data. Handles migration from the legacy pipe-delimited "data" file,
    /// <br/>backs up corrupt JSON files and returns a default object if nothing exists.
    /// </summary>
    public static UserData LoadUserData()
    {
        Directory.CreateDirectory(AppFolder);
        Directory.CreateDirectory(BackupFolder);

        if (File.Exists(DataPath))
        {
            try
            {
                var json = File.ReadAllText(DataPath);
                var data = JsonSerializer.Deserialize<UserData>(json, JsonOptions);
                if (data != null) return data;

                BackupCorruptFile(DataPath, "deserialized-null");
                MessageBox.Show("Data file corrupted.\nBackup saved in %appdata%\\SALG\\backup", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (JsonException je)
            {
                BackupCorruptFile(DataPath, "json-error-" + SanitizeForFilename(je.Message));
                MessageBox.Show("Data file deserialization fail.\nBackup saved in %appdata%\\SALG\\backup", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                BackupCorruptFile(DataPath, "io-error-" + SanitizeForFilename(ex.Message));
                MessageBox.Show("Data file deserialization fail.\nBackup saved in %appdata%\\SALG\\backup", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        if (File.Exists("data"))
        {
            try
            {
                var legacy = File.ReadAllText("data");
                var migrated = MigrateLegacyData(legacy);
                if (migrated != null)
                {
                    SaveUserData(migrated);
                    BackupLegacyFile("data");
                    return migrated;
                }
            }
            catch
            {
                BackupLegacyFile("data");
            }
        }

        return new UserData();
    }

    /// <summary>
    /// Save user data atomically (write to temp file then move).
    /// </summary>
    public static void SaveUserData(UserData data, string? path = null)
    {
        path ??= DataPath;
        Directory.CreateDirectory(AppFolder);

        var temp = DataPath + ".tmp";
        var json = JsonSerializer.Serialize(data, JsonOptions);

        File.WriteAllText(temp, json);
        if (File.Exists(DataPath))
            File.Replace(temp, DataPath, DataPath + ".bak");
        else
            File.Move(temp, DataPath);
    }

    public static Prefs LoadPreferences()
    {
        Directory.CreateDirectory(AppFolder);

        if (File.Exists(PrefsPath))
        {
            try
            {
                var json = File.ReadAllText(PrefsPath);
                var prefs = JsonSerializer.Deserialize<Prefs>(json, JsonOptions);
                if (prefs != null) return prefs;

                BackupCorruptFile(PrefsPath, "deserialized-null");
                MessageBox.Show("Preference file corrupted.\nBackup saved in %appdata%\\SALG\\backup", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (JsonException je)
            {
                BackupCorruptFile(PrefsPath, "json-error-" + SanitizeForFilename(je.Message));
                MessageBox.Show("Preference file deserialization fail.\nBackup saved in %appdata%\\SALG\\backup", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                BackupCorruptFile(PrefsPath, "io-error-" + SanitizeForFilename(ex.Message));
                MessageBox.Show("Preference file deserialization fail.\nBackup saved in %appdata%\\SALG\\backup", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        if (File.Exists("misc"))
        {
            try
            {
                var legacy = File.ReadAllText("misc");
                var migrated = MigrateLegacyMisc(legacy);
                if (migrated != null)
                {
                    SavePreferences(migrated);
                    BackupLegacyFile("misc");
                    return migrated;
                }
            }
            catch
            {
                BackupLegacyFile("misc");
            }
        }

        return new Prefs();
    }

    public static string LoadNotes()
    {
        Directory.CreateDirectory(AppFolder);

        if (File.Exists(NotesPath))
        {
            var notes = File.ReadAllLines(NotesPath);
            return notes.Length >= 1 ? notes[0] : "";
        }

        if (File.Exists("notes"))
        {
            try
            {
                var legacy = File.ReadAllLines("notes");
                var current = legacy.Length >= 1 ? legacy[0] : "";
                File.WriteAllText(NotesPath, current + NotesMessage);
                BackupLegacyFile("notes");
                return current;
            }
            catch
            {
                BackupLegacyFile("notes");
            }
        }
                

        return "";
    }

    public static void SavePreferences(Prefs prefs)
    {
        Directory.CreateDirectory(AppFolder);

        var temp = PrefsPath + ".tmp";
        var json = JsonSerializer.Serialize(prefs, JsonOptions);

        File.WriteAllText(temp, json);
        if (File.Exists(PrefsPath))
            File.Replace(temp, PrefsPath, PrefsPath + ".bak");
        else
            File.Move(temp, PrefsPath);
    }

    private static void BackupCorruptFile(string path, string reason)
    {
        try
        {
            Directory.CreateDirectory(BackupFolder);
            var stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var name = Path.GetFileName(path);
            var safeReason = SanitizeForFilename(reason);
            var dest = Path.Combine(BackupFolder, $"{name}.corrupt.{safeReason}.{stamp}.bak");
            File.Move(path, dest);
        }
        catch
        {
            // Don't throw from backup -— best effort only.
        }
    }

    private static void BackupLegacyFile(string path)
    {
        try
        {
            Directory.CreateDirectory(BackupFolder);
            var stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var name = Path.GetFileName(path);
            var dest = Path.Combine(BackupFolder, $"{name}.migrated.{stamp}.bak");
            File.Move(path, dest);
        }
        catch
        {
            // best effort
        }
    }

    private static string SanitizeForFilename(string s)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            s = s.Replace(c, '_');
        if (s.Length > 50) s = s[..50];
        return s;
    }

    /// <summary>
    /// Parses the old pipe-delimited format and returns a UserData object if valid,
    /// <br/>otherwise null.
    /// <br/>Syntax: username|rank|qDone|tTime|cQuota|show qDone (Y/N)
    /// </summary>
    private static UserData? MigrateLegacyData(string legacyContent)
    {
        if (string.IsNullOrWhiteSpace(legacyContent))
            return null;

        var parts = legacyContent.Split('|');

        if (parts.Length != 6)
            return null;

        try
        {
            UserData ud = new()
            {
                Username = parts[0].Trim(),
                RankString = parts[1].Trim(),
            };

            if (int.TryParse(parts[2].Trim(), out var qDone))
                ud.QuotaDone = qDone;
            else
                ud.QuotaDone = 0;

            if (int.TryParse(parts[3].Trim(), out var tTime))
                ud.TotalTime = tTime;
            else
                ud.TotalTime = 0;

            if (int.TryParse(parts[4].Trim(), out var cQuota))
                ud.CurrentQuota = cQuota;
            else
                ud.CurrentQuota = 0;

            ud.ShowQuotaDone = parts[5].Trim().ToUpper() == "Y";

            return ud;
        }
        catch
        {
            return null;
        }
    }

    private static Prefs? MigrateLegacyMisc(string legacyContent)
    {
        if (string.IsNullOrWhiteSpace(legacyContent))
            return null;

        var parts = legacyContent.Split('|');

        if (parts.Length != 2)
            return null;

        try
        {
            Prefs prefs = new()
            {
                DarkMode = parts[0].Trim().ToUpper() == "Y",
                Department = parts[1].Trim().ToUpper() == "Y" ? Department.TRT : Department.DoS
            };

            return prefs;
        }
        catch
        {
            return null;
        }
    }
}