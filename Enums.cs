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
        TRT_Cadet,
        TRT_Operative,
        TRT_Sergeant,
        TRT_First_Sergeant,
        TRT_Captain
    }

    public enum TRTRankShort
    {
        SGT = TRTRank.TRT_Sergeant,
        FSGT = TRTRank.TRT_First_Sergeant,
        CPT = TRTRank.TRT_Captain
    }
}
