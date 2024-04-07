using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using OwlCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoptLib.Writing.Linguistics;

public class PhoneticWord
{
    public PhoneticWord(IList<PhoneticEquivalent> equivalents, IEnumerable<int> syllableBreaks)
    {
        Equivalents = equivalents;
        SyllableBreaks = new(syllableBreaks);
    }

    public IList<PhoneticEquivalent> Equivalents { get; }

    public SortedSet<int> SyllableBreaks { get; set; }

    public SortedSet<int> PrimarySyllables { get; set; } = [];

    public SortedSet<int> SecondarySyllables { get; set; } = [];

    public int Length => Equivalents.Count;

    public static PhoneticWord Parse(params string[] syllables)
    {
        List<PhoneticEquivalent> equivalents = new();
        List<int> syllableBreaks = new(syllables.Length);
        int currentSyllableBreak = 0;

        foreach (string syllable in syllables)
        {
            var syllableEquivalents = PhoneticEquivalent.Parse(syllable);
            equivalents.AddRange(syllableEquivalents);

            syllableBreaks.Add(currentSyllableBreak += syllableEquivalents.Length);
        }

        // No need to specify that the last syllable ends
        // at the end of the word.
        syllableBreaks.RemoveAt(syllableBreaks.Count - 1);

        return new(equivalents, syllableBreaks);
    }

    public void CopyTo(PhoneticWord destinationWord, int offset)
    {
        Guard.IsGreaterThanOrEqualTo(offset, 0);
        Guard.IsNotNull(destinationWord);

        // Copy the word itself, making the destination list bigger if necessary
        for (int i = 0; i < Equivalents.Count; i++)
            destinationWord.Equivalents.ReplaceOrAdd(i + offset, Equivalents[i]);

            // Copy the syllable breaks, adjusting the indexes for the offset
        foreach (var syllableBreak in SyllableBreaks)
            destinationWord.SyllableBreaks.Add(syllableBreak + offset);
    }

    public PhoneticWord Substring(int startIndex) => Substring(startIndex, Length - startIndex);

    public PhoneticWord Substring(int startIndex, int length)
    {
        var subEquivalents = Equivalents
            .Slice(startIndex, length)
            .ToArray();

        var subSyllableBreaks = SyllableBreaks
            .Where(b => b > startIndex)
            .Select(b => b - startIndex)
            .ToArray();

        return new PhoneticWord(subEquivalents, subSyllableBreaks);
    }

    public bool IsPrimaryStress(int index) => PrimarySyllables.Contains(index);
    public bool IsSecondaryStress(int index) => SecondarySyllables.Contains(index);

    public string GetSeparatorForSyllable(int index, SyllableSeparatorSet syllableSeparators)
    {
        if (IsPrimaryStress(index))
            return syllableSeparators.PrimaryStressed;
        else if (IsSecondaryStress(index))
            return syllableSeparators.SecondaryStressed;
        else
            return syllableSeparators.Unstressed;
    }

    public override string ToString()
    {
        string ogStr = ToString(false);
        string ipaStr = ToIpaString();
        return $"({ogStr}, {ipaStr})";
    }

    /// <summary>
    /// Returns a string that represents either the original word or
    /// its IPA transcription, using the original casing.
    /// </summary>
    /// <param name="useIpa">
    /// Whether to use the IPA transcription instead of the original text.
    /// </param>
    /// <param name="syllableSeparators">
    /// The <see cref="string"/> to separate syllables with,
    /// or <see langword="null"/> to not separate syllables.
    /// </param>
    public string ToString(bool useIpa, SyllableSeparatorSet? syllableSeparators = null)
    {
        bool insertSyllableBreaks = syllableSeparators is not null;
        StringBuilder sb = new(Length + (insertSyllableBreaks ? SyllableBreaks.Count : 0));

        for (int i = 0; i < Length; i++)
        {
            if (insertSyllableBreaks && SyllableBreaks.Contains(i))
            {
                var sep = GetSeparatorForSyllable(i, syllableSeparators!.Value);
                sb.Append(sep);
            }

            var pe = Equivalents[i];
            sb.Append(useIpa ? pe.GetIpa() : pe.GetSource());
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns a string that represents the IPA transcription of
    /// the source word, using the original casing.
    /// </summary>
    public string ToIpaString() => ToString(true, SyllableSeparatorSet.IPA);
}
