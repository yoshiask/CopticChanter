using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using OwlCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Writing.Linguistics;

public class PhoneticWord
{
    public PhoneticWord(IList<PhoneticEquivalent> equivalents, IList<int> syllableBreaks)
    {
        Equivalents = equivalents;
        SyllableBreaks = syllableBreaks;
    }

    public IList<PhoneticEquivalent> Equivalents { get; }

    public IList<int> SyllableBreaks { get; }

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

            syllableBreaks.Add(currentSyllableBreak += syllable.Length);
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
            destinationWord.SyllableBreaks.ReplaceOrAdd(syllableIndex, syllableBreak + offset);
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
        string ogStr = string.Join(null, Equivalents.Select(pe => pe.GetSource()));

        string ipaStr = string.Join(null, Equivalents.Select(pe => pe.GetIpa()));
        foreach (int syllableBreak in SyllableBreaks)
            ipaStr = ipaStr.Insert(syllableBreak, "'");

        return $"({ogStr}, {ipaStr})";
    }
}
