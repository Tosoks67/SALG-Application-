using System;

namespace SALG__Application_;

/// <summary>
/// Class representing user data (username, rank, quota done, total time, etc.)
/// </summary>
public class UserData
{
    public string Username { get; set; } = "John Doe";
    public string RankString { get; set; } = "";
    public DoS_Rank DoS => TryParseRankOrDefault<DoS_Rank>();
    public TRT_Rank Trt => TryParseRankOrDefault<TRT_Rank>();
    public Alpha1_Rank Alpha1 => TryParseRankOrDefault<Alpha1_Rank>();
    public Epsilon11_Rank Epsilon11 => TryParseRankOrDefault<Epsilon11_Rank>();
    public Nu7_Rank Nu7 => TryParseRankOrDefault<Nu7_Rank>();
    public int QuotaDone { get; set; }
    public int TotalTime { get; set; }
    public int CurrentQuota { get; set; }
    public bool ShowQuotaDone { get; set; } = true;

    private T TryParseRankOrDefault<T>() where T : struct, Enum => Enum.TryParse(RankString.Replace(' ', '_'), out T r) ? r : default;

    public override string ToString() =>
        $"Username: {Username}" +
        $"\nRank String: {RankString}" +
        $"\nDoS: {DoS}" +
        $"\nTRT: {Trt}" +
        $"\nAlpha-1: {Alpha1}" +
        $"\nEpsilon-11: {Epsilon11}" +
        $"\nNu-7: {Nu7}" +
        $"\nQuota Done: {QuotaDone}" +
        $"\nTotal Time: {TotalTime}" +
        $"\nCurrent Quota: {CurrentQuota}" +
        $"\nShow Quota Done: {ShowQuotaDone}";
}