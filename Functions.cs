using System;
using System.IO;
using System.Windows.Navigation;

namespace SALG__Application_
{
    internal static class Functions
    {
        public static bool RankStringToEnum(string value, out Rank result, Rank fallback = Rank.None)
        {
            if (Enum.TryParse(value, true, out Rank directRank))
            {
                result = directRank;
                return true;
            }

            if (Enum.TryParse(value, true, out RankShort shortRank))
            {
                result = (Rank)(int)shortRank;
                return true;
            }

            result = fallback;
            return false;
        }

        public static bool TRTStringToEnum(string value, out TRTRank result, TRTRank fallback = TRTRank.None)
        {
            if (Enum.TryParse(value, true, out TRTRank directRank))
            {
                result = directRank;
                return true;
            }

            if (Enum.TryParse(value, true, out TRTRankShort shortRank))
            {
                result = (TRTRank)(int)shortRank;
                return true;
            }

            result = fallback;
            return false;
        }

        public static bool CheckUp(string[] data)
        {
            /* syntax:
                    data[0] = Username
                    data[1] = Rank
                    data[2] = Quota Done
                    data[3] = Total Time
                    data[4] = Quota
                    data[5] = Show Quota Done */
            RankStringToEnum(data[1].Replace(' ', '_'), out Rank rank);
            TRTStringToEnum(data[1].Replace(' ', '_'), out TRTRank TRTRank);
            if (data.Length != 6 || (rank == Rank.None && TRTRank == TRTRank.None) || !int.TryParse(data[2], out int _) || !int.TryParse(data[3], out int _) || !int.TryParse(data[4], out int _) || (data[5].ToUpper() != "Y" && data[5].ToUpper() != "N"))
            {
                File.Delete("data");
                return false;
            }
            return true;
        }

        public static bool CheckMisc(string[] misc)
        {
            /* syntax:
                    misc[0] = Dark Mode
                    misc[1] = TRT */
            if (misc.Length != 2 || misc[0].ToUpper() != "Y" && misc[0].ToUpper() != "N" || misc[1].ToUpper() != "Y" && misc[1].ToUpper() != "N")
            {
                File.Delete("misc");
                return false;
            }
            return true;
        }

        public static string[] ReadData()
        {
            if (!File.Exists("data"))
            {
                return ["", "", "", "", "", ""];
            }
            string full = File.ReadAllText("data");
            string[] data = full.Split('|');
            return data;
        }

        public static string[] ReadMisc()
        {
            if (!File.Exists("misc"))
            {
                return ["", ""];
            }
            string full = File.ReadAllText("misc");
            string[] data = full.Split('|');
            return data;
        }

        public static void WriteData(string username, string currentRank, string quotaDone, string totalTimeServed, string requiredQuota, string showQuotaDone)
        {
            username = username == "" ? "John Doe" : username;
            currentRank = currentRank == "" ? "Cadet" : currentRank;
            quotaDone = quotaDone == "" ? "0" : quotaDone;
            totalTimeServed = totalTimeServed == "" ? "0" : totalTimeServed;
            requiredQuota = requiredQuota == "" ? "120" : requiredQuota;
            showQuotaDone = showQuotaDone == "" ? "Y" : showQuotaDone;
            File.WriteAllText("data", username + "|" + currentRank + "|" + quotaDone + "|" + totalTimeServed + "|" + requiredQuota + "|" + showQuotaDone);
        }

        public static string[] GetAndCheck()
        {
            if (!CheckUp(ReadData()))
            {
                Setup setup = new();
                setup.ShowDialog();
            }
            return ReadData();
        }
    }
}
