namespace CoptLib.Writing
{
    /// <summary>
    /// Represents a character and its pronunciation.
    /// </summary>
    /// <remarks>
    /// Consumers determine whether the pronunciation is accurate.
    /// </remarks>
    public sealed class PhoneticEquivalent
    {
        public PhoneticEquivalent(char source, string ipa)
        {
            Source = source;
            Ipa = ipa;
        }

        /// <summary>
        /// The original character.
        /// </summary>
        public char Source { get; set; }

        /// <summary>
        /// The pronunciation represented with IPA.
        /// </summary>
        public string Ipa { get; set; }

        public override string ToString() => $"('{Source}', \"{Ipa}\")";
    }
}
