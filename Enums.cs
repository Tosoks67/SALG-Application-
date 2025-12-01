namespace SALG__Application_;

/// <summary>
/// DoS ranks
/// </summary>
public enum DoS_Rank
{
    None,
    DoS_Cadet,
    DoS_Private,
    DoS_Lance_Corporal,
    DoS_Corporal,
    DoS_Sergeant,
    DoS_Staff_Sergeant,
    DoS_Lieutenant,
    DoS_Major,
    DoS_Colonel,
    DoS_Assistant_Director_of_Security
}

/// <summary>
/// TRT ranks
/// </summary>
public enum TRT_Rank
{
    None,
    TRT_Cadet,
    TRT_Operative,
    TRT_Sergeant,
    TRT_First_Sergeant,
    TRT_Captain
}

/// <summary>
/// MTF Alpha-1 ranks
/// </summary>
public enum Alpha1_Rank
{
    None,
    Al1_Operator,
    Al1_Tactician,
    Al1_Field_Sergeant,
    Al1_First_Sergeant,
    Al1_Lieutenant,
    Al1_Field_Commander,
    Al1_Assistant_Director_of_Task_Forces
}

/// <summary>
/// MTF Epsilon-11 ranks
/// </summary>
public enum Epsilon11_Rank
{
    None,
    E11_Trainee,
    E11_Probationary_Operative,
    E11_Operative,
    E11_Agent,
    E11_Field_Sergeant,
    E11_First_Sergeant,
    E11_Lieutenant,
    E11_Field_Captain
}

/// <summary>
/// MTF Nu-7 ranks
/// </summary>
public enum Nu7_Rank
{
    None,
    Nu7_Recruit,
    Nu7_Trainee,
    Nu7_Probationary_Operative,
    Nu7_Private,
    Nu7_Specialist,
    Nu7_Field_Sergeant,
    Nu7_First_Sergeant,
    Nu7_Lieutenant,
    Nu7_Field_Captain
}

/// <summary>
/// Self-explanatory
/// </summary>
public enum Department
{
    DoS,
    TRT,
    Alpha1,
    Epsilon11,
    Nu7
}

/// <summary>
/// Contains all the colors for all the departments used for the gradient background in MainWindow
/// <br/>(Light and Dark mode)
/// </summary>
public enum DepartmentColors
{
    None = 0,
    DoS = 0x545454,
    TRT = 0x545454,
    Alpha1 = 0x800000,
    Epsilon11 = 0x7f3200,
    Nu7 = 0x7caa00,
    DoS_dark = 0xffffff,
    TRT_dark = 0xffffff,
    Alpha1_dark = 0x800000,
    Epsilon11_dark = 0x803300,
    Nu7_dark = 0x3c5400
}

/// <summary>
/// Static class containing default values
/// <br/>(Default log format, Notes Message)
/// </summary>
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