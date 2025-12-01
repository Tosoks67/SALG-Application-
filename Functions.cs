using System;
using System.Collections.Generic;
using System.IO;
using static SALG__Application_.Defaults;
using static SALG__Application_.FilePaths;

namespace SALG__Application_;

internal static class Functions
{
    /// <summary>
    /// Checks ranks. Returns true if at least one rank is set, false if both are None (also sets a default rank in that case).
    /// </summary>
    public static bool CheckRanks(UserData data, out UserData udOut, Department dep = Department.DoS)
    {
        udOut = data;
        if (dep == Department.DoS && data.DoS != DoS_Rank.None ||
            dep == Department.TRT && data.Trt != TRT_Rank.None ||
            dep == Department.Alpha1 && data.Alpha1 != Alpha1_Rank.None ||
            dep == Department.Epsilon11 && data.Epsilon11 != Epsilon11_Rank.None ||
            dep == Department.Nu7 && data.Nu7 != Nu7_Rank.None)
            return true;

        udOut.RankString = dep switch
        {
            Department.DoS => nameof(DoS_Rank.DoS_Cadet),
            Department.TRT => nameof(TRT_Rank.TRT_Cadet),
            Department.Alpha1 => nameof(Alpha1_Rank.Al1_Operator),
            Department.Epsilon11 => nameof(Epsilon11_Rank.E11_Trainee),
            Department.Nu7 => nameof(Nu7_Rank.Nu7_Recruit),
            _ => "None"
        };

        return false;
    }

    /// <summary>
    /// Logs a message to log.txt in the application folder (includes a timestamp at the beginning)
    /// Will be included as a debugging option in future releases
    /// </summary>
    public static void Log(string message)
    {
        var path = Path.Combine(AppFolder, "log.txt");
        var msg = "\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + message;
        var currentText = File.Exists(path) ? File.ReadAllText(path) : "";
        File.WriteAllText(path, currentText + msg);
    }

    /// <summary>
    /// Generate a log string based on the provided data
    /// See Format.xaml for an explanation regarding the custom format system
    /// </summary>
    public static string GenerateLog(Prefs prefs, UserData data, string sTime, string eTime, int tOnSite, string notes = "", string format = DefaultFormat)
    {
        var rankString = data.RankString.PrepRankString().LogPrefix(prefs);
        List<string> formatList = [.. format.Split('\n')];
        for (var i = formatList.Count - 1; i >= 0; i--)
        {
            if (formatList[i].Length < 4) continue;
            switch (formatList[i][..4])
            {
                case "$(q)" when !data.ShowQuotaDone:
                case "$(n)" when string.IsNullOrWhiteSpace(notes):
                    formatList.RemoveAt(i);
                    break;
                case "$(q)" or "$(n)":
                    formatList[i] = formatList[i][4..];
                    break;
                default:
                    continue;
            }
        }
        format = string.Join('\n', formatList);
        return format
            .Replace("$(username)", data.Username)
            .Replace("$(rank)", rankString)
            .Replace("$(start_time)", sTime)
            .Replace("$(end_time)", eTime)
            .Replace("$(time_on_site)", tOnSite.ToString())
            .Replace("$(total_time)", (data.TotalTime + tOnSite).ToString())
            .Replace("$(quota_done)", (data.QuotaDone + tOnSite).ToString())
            .Replace("$(current_quota)", data.CurrentQuota.ToString())
            .Replace("$(notes)", notes);
    }

    /// <summary>
    /// Extension function for strings , removes the first 4 characters and replaces underscores with spaces.
    /// </summary>
    public static string PrepRankString(this string input)
    {
        return input.Replace('_',' ')[4..];
    }

    public static string StringToRankName(this string input, Department dep)
    {
        var depString = dep switch
        {
            Department.DoS => "DoS ",
            Department.TRT => "TRT ",
            Department.Alpha1 => "Al1 ",
            Department.Epsilon11 => "E11 ",
            Department.Nu7 => "Nu7 ",
            _ => ""
        };
        return (depString + input).Replace(' ','_');
    }

    private static string LogPrefix(this string input, Prefs prefs)
    {
        var prefix = prefs.Department switch
        {
            Department.DoS => prefs.DoSPrefix.Enabled ? prefs.DoSPrefix.Text + " " : "",
            Department.TRT => prefs.TrtPrefix.Enabled ? prefs.TrtPrefix.Text + " " : "",
            Department.Alpha1 => prefs.Alpha1Prefix.Enabled ? prefs.Alpha1Prefix.Text + " " : "",
            Department.Epsilon11 => prefs.Epsilon11Prefix.Enabled ? prefs.Epsilon11Prefix.Text + " " : "",
            Department.Nu7 => prefs.Nu7Prefix.Enabled ? prefs.Nu7Prefix.Text + " " : "",
            _ => ""
        };

        switch (prefs.Department)
        {
            case Department.DoS:
                Enum.TryParse(input, out DoS_Rank dR);
                if (dR == DoS_Rank.DoS_Assistant_Director_of_Security)
                    return input;
                else
                    return prefix + input;
            case Department.TRT:
                return prefix + input;
            case Department.Alpha1:
                Enum.TryParse(input, out Alpha1_Rank aR);
                if (aR == Alpha1_Rank.Al1_Assistant_Director_of_Task_Forces)
                    return input;
                else
                    return prefix + input;
            case Department.Epsilon11:
                return prefix + input;
            case Department.Nu7:
                return prefix + input;
            default:
                return prefix + input;
        }
    }
}