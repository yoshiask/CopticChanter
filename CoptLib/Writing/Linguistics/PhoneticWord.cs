using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using OwlCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoptLib.Writing.Linguistics;

public class PhoneticWord
{
    public const char DEFAULT_SYLLABLE_SEPARATOR = 'ˌ';

    public PhoneticWord(IList<PhoneticEquivalent> equivalents, IEnumerable<int> syllableBreaks)
    {
        Equivalents = equivalents;
        SyllableBreaks = new(syllableBreaks);
    }

    public IList<PhoneticEquivalent> Equivalents { get; }

    public SortedSet<int> SyllableBreaks { get; }

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
        {
            destinationWord.Equivalents.ReplaceOrAdd(i + offset, Equivalents[i]);
        }

        // Copy the syllable breaks, adjusting the indexes for the offset
        int syllableIndex = 0;
        foreach (int syllableBreak in SyllableBreaks)
        {
            destinationWord.SyllableBreaks.Add(syllableBreak + offset);
            syllableIndex++;
        }
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

    public override string ToString()
    {
        string ogStr = ToString(false);
        string ipaStr = ToString(true, DEFAULT_SYLLABLE_SEPARATOR);
        return $"({ogStr}, {ipaStr})";
    }

    /// <summary>
    /// Returns a string that represents either the original word or
    /// its IPA transcription, using the original casing.
    /// </summary>
    /// <param name="useIpa">
    /// Whether to use the IPA transcription instead of the original text.
    /// </param>
    /// <param name="insertSyllableMarkers">
    /// Whether to include syllable markers in the final string.
    /// </param>
    public string ToString(bool useIpa, char? syllableMarkerChar = null)
    {
        Func<PhoneticEquivalent, object> getLetter = useIpa
            ? (PhoneticEquivalent pe) => pe.GetIpa()
            : (PhoneticEquivalent pe) => pe.GetSource();

        StringBuilder sb = new(Length + (syllableMarkerChar != null ? SyllableBreaks.Count : 0));

        for (int i = 0; i < Length; i++)
        {
            if (syllableMarkerChar != null && SyllableBreaks.Contains(i))
                sb.Append(syllableMarkerChar);

            var pe = Equivalents[i];
            if (useIpa)
                sb.Append(pe.GetIpa());
            else
                sb.Append(pe.GetSource());
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns a string that represents the IPA transcription of
    /// the source word, using the original casing.
    /// </summary>
    public string ToIpaString() => ToString(true, DEFAULT_SYLLABLE_SEPARATOR);
}
