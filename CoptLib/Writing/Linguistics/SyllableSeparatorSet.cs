namespace CoptLib.Writing.Linguistics;

public readonly struct SyllableSeparatorSet
{
    public static SyllableSeparatorSet IPA = new("ˈ", "ˌ", ".");
    public static SyllableSeparatorSet Empty = new("", "", "");

    public SyllableSeparatorSet(string syllableSeparator)
    {
        PrimaryStressed = SecondaryStressed = Unstressed = syllableSeparator;
    }

    public SyllableSeparatorSet(string primary, string secondary, string unstressed)
    {
        PrimaryStressed = primary;
        SecondaryStressed = secondary;
        Unstressed = unstressed;
    }

    public string PrimaryStressed { init; get; }
    public string SecondaryStressed { init; get; }
    public string Unstressed { init; get; }
}
