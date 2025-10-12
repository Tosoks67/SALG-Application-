using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SALG__Application_.Functions;

namespace SALG__Application_
{
    /// <summary>
    /// Class representing user data (username, rank, quota done, total time, etc.)
    /// </summary>
    public class UserData
    {
        private string _rankString = "None";
        public string Username { get; set; } = "John Doe";
        public string RankString
        {
            get => _rankString;
            set
            {
                _rankString = value;
                UpdateRanks();
            }
        }
        public Rank Rank { get; private set; } = Rank.None;
        public TRTRank TRTRank { get; private set; } = TRTRank.None;
        public int QuotaDone { get; set; } = 0;
        public int TotalTime { get; set; } = 0;
        public int CurrentQuota { get; set; } = 0;
        public bool ShowQuotaDone { get; set; } = true;

        /// <summary>
        /// Is fired each time RankString is set, updates the Rank and TRTRank properties accordingly.
        /// </summary>
        private void UpdateRanks()
        {
            if (!Enum.TryParse(RankString.Replace(' ', '_'), out Rank r)) r = default;
            if (!Enum.TryParse(RankString.Replace(' ', '_'), out TRTRank tR)) tR = default;
            Rank = r;
            TRTRank = tR;
        }
    }
}
