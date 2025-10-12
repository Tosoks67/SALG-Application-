namespace SALG__Application_
{
    /// <summary>
    /// DoS ranks
    /// </summary>
    public enum Rank
    {
        None,
        Cadet,
        Private,
        Lance_Corporal,
        Corporal,
        Sergeant,
        Staff_Sergeant,
        Lieutenant,
        Major,
        Colonel,
        Assistant_Director_of_Security
    }

    /// <summary>
    /// TRT ranks
    /// </summary>
    public enum TRTRank
    {
        None,
        TRT_Cadet,
        TRT_Operative,
        TRT_Sergeant,
        TRT_First_Sergeant,
        TRT_Captain
    }

    public static class Defaults
    {
        public const string DefaultFormat =
            "**Username: **$(username)" +
            "\r\n**Rank: **$(rank)" +
            "\r\n**Start Time: **$(start_time)" +
            "\r\n**End time: **$(end_time)" +
            "\r\n**Total time on-site: **$(time_on_site) minutes" +
            "\r\n**Total time: **$(total_time) minutes" +
            "\r\n$(q)-# Quota: $(quota_done) / $(current_quota)" +
            "\r\n__**Evidence: **__" +
            "\r\n$(n)**Note(s): **$(notes)";
        public const string NotesMessage = "\n\nDo know that only the first line is loaded.";
    }
}
