namespace SALG;

public class Prefix
{
    public string Text { get; set; } = "";
    public bool Enabled { get; set; } = true;

    public override string ToString() =>
        $"Text: {Text}" +
        $"\nEnabled: {Enabled}";
}