namespace SALG__Application_
{
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

    public enum RankShort
    {
        PVT = Rank.Private,
        LCPL = Rank.Lance_Corporal,
        CPL = Rank.Corporal,
        SGT = Rank.Sergeant,
        SSGT = Rank.Staff_Sergeant,
        LT = Rank.Lieutenant,
        MAJ = Rank.Major,
        COL = Rank.Colonel,
        ADoS = Rank.Assistant_Director_of_Security
    }

    public enum TRTRank
    {
        None,
        Cadet,
        Operative,
        Sergeant,
        First_Sergeant,
        Captain
    }

    public enum TRTRankShort
    {
        SGT = TRTRank.Sergeant,
        FSGT = TRTRank.First_Sergeant,
        CPT = TRTRank.Captain
    }
}
