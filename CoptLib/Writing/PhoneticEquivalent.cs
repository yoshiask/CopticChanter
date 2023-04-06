namespace CoptLib.Writing
{
    /// <summary>
    /// Represents a character and its pronunciation.
    /// </summary>
    /// <remarks>
    /// Consumers determine whether the pronunciation is accurate.
    /// </remarks>
    public struct PhoneticEquivalent
    {
        public PhoneticEquivalent(char source, string ipa)
        {
            Source = char.ToLower(source);
            IsUpper = char.IsUpper(source);
            Ipa = ipa;
        }

        public PhoneticEquivalent(char source, string ipa, bool isUpper)
        {
            Source = source;
            Ipa = ipa;
            IsUpper = isUpper;
        }

        /// <summary>
        /// The original character.
        /// </summary>
        public char Source { get; set; }

        /// <summary>
        /// The pronunciation represented with IPA.
        /// </summary>
        public string Ipa { get; set; }

        /// <summary>
        /// Whether the source letter should be uppercase.
        /// </summary>
        public bool IsUpper { get; set; }

        /// <summary>
        /// Gets the <see cref="Source"/> with correct casing.
        /// </summary>
        public char GetSource() => !IsUpper ? Source : char.ToUpper(Source);

        /// <summary>
        /// Gets the <see cref="Ipa"/> transcription with correct casing.
        /// </summary>
        public string GetIpa()
        {
            if (!IsUpper || Ipa.Length == 0)
            {
                return Ipa;
            }
            else
            {
                if (Ipa.Length == 1)
                    return Ipa.ToUpper();
                else
                    return char.ToUpper(Ipa[0]) + Ipa[1..];
            }
        }

        public override string ToString() => $"('{GetSource()}', \"{GetIpa()}\")";

        /// <summary>
        /// Parses a special format string into a set of <see cref="PhoneticEquivalent"/>s.
        /// Pairs are split by semicolors, and the Source and IPA string by commas.
        /// <para>Example:<br/><c>"ⲙ,m;ⲁ,ä;ⲣ,ɾ;ⲓ,ⲓ;ⲁ,ä"</c></para>
        /// </summary>
        /// <param name="value">The string to parse.</param>
        public static PhoneticEquivalent[] Parse(string value)
        {
            var pairs = value.Split(';');
            var word = new PhoneticEquivalent[pairs.Length];

            for (int i = 0; i < word.Length; i++)
            {
                var data = pairs[i].Split(',');
                word[i] = new(data[0][0], data[1]);
            }

            return word;
        }

        /// <summary>
        /// Creates a copy of the current object.
        /// </summary>
        /// <remarks>
        /// Since <see cref="IPA"/> is a reference type, its reference
        /// is copied rather than its value. Changes made to that property
        /// will affect all copies of this object.
        /// </remarks>
        public PhoneticEquivalent Clone() => new(Source, Ipa, IsUpper);
    }
}
