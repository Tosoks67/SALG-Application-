namespace SALG__Application_;

public class Prefix
{
    public string Text { get; set; } = "";
    public bool Enabled { get; set; } = true;

    public override string ToString() =>
        $"Text: {Text}" +
        $"\nEnabled: {Enabled}";
}