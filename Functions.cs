using System;
using System.IO;
using System.Windows.Navigation;
using static SALG__Application_.FilePaths;
using static SALG__Application_.Defaults;

namespace SALG__Application_
{
    internal static class Functions
    {
        /// <summary>
        /// Checks ranks. Returns true if at least one rank is set, false if both are None (also sets a default rank in that case).
        /// </summary>
        public static bool CheckRanks(UserData data, out UserData udOut, bool TRT = false)
        {
            udOut = data;
            if (data.Rank != Rank.None || data.TRTRank != TRTRank.None) return true;
            udOut.RankString = TRT ? TRTRank.TRT_Cadet.ToString() : Rank.Cadet.ToString();
            return false;
        }

        /// <summary>
        /// Logs a message to log.txt in the application folder (includes a timestamp at the beginning)
        /// Will be included as a debugging option in future releases
        /// </summary>
        public static void Log(string message)
        {
            string path = Path.Combine(AppFolder, "log.txt");
            string msg = "\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + message;
            File.WriteAllText(path, File.ReadAllText(path) + msg);
        }

        /// <summary>
        /// Generate a log string based on the provided data
        /// See Format.xaml for an explanation regarding the custom format system
        /// </summary>
        public static string GenerateLog(UserData data, string sTime, string eTime, int tOnSite, string notes = "", string format = DefaultFormat)
        {
            string rankString =
                data.Rank != Rank.None && data.Rank != Rank.Assistant_Director_of_Security ? "Security " + data.Rank.ToString().Replace('_', ' ') :
                data.TRTRank != TRTRank.None ? "Tactical Response " + data.TRTRank.ToString().Substring(4).Replace('_', ' ') :
                data.RankString.Replace('_', ' ');
            List<string> formatList = new(format.Split('\n'));
            for (int i = formatList.Count - 1; i >= 0; i--)
            {
                if (formatList[i].Length < 4) continue;
                if ((formatList[i][..4] == "$(q)"))
                {
                    if (!data.ShowQuotaDone)
                    {
                        formatList.RemoveAt(i);
                        continue;
                    }
                    else
                        formatList[i] = formatList[i][4..];
                }
                if (formatList[i][..4] == "$(n)")
                {
                    if (string.IsNullOrWhiteSpace(notes))
                    {
                        formatList.RemoveAt(i);
                        continue;
                    }
                    else
                        formatList[i] = formatList[i][4..];
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
    }
}
