namespace SALG__Application_;

public class Prefs
{
    public bool DarkMode { get; set; }
    public Department Department { get; set; } = Department.DoS;
    public Prefix DoSPrefix { get; } = new() { Text = "Security", Enabled = true };
    public Prefix TrtPrefix { get; } = new() { Text = "Tactical Response", Enabled = true };
    public Prefix Alpha1Prefix { get; } = new() { Text = "Alpha-1", Enabled = true };
    public Prefix Epsilon11Prefix { get; } = new() { Text = "Epsilon-11", Enabled = true };
    public Prefix Nu7Prefix { get; } = new() { Text = "Nu-7", Enabled = true };

    public override string ToString() =>
        $"Dark Mode: {DarkMode}" +
        $"\nDepartment: {Department}" +
        $"\n\nDoS prefix: \n{DoSPrefix}" +
        $"\n\nTRT prefix: \n{TrtPrefix}" +
        $"\n\nAlpha-1 prefix: \n{Alpha1Prefix}" +
        $"\n\nEpsilon-11 prefix: \n{Epsilon11Prefix}" +
        $"\n\nNu-7 prefix: \n{Nu7Prefix}";
}