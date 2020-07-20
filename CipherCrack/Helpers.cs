using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherCrack
{
    public static class Helpers
    {
        /// <summary>
        /// Get the word signature for the purposes of decoding the ciphered version.
        /// The word signature captures how many characters the word has, as well as the character
        /// repetition pattern.
        /// </summary>
        /// <example>
        /// The word "test" would be encoded as "4[(0, 3)]", since it is composed by 4 total characters, and the ones
        /// in the first and third positions are repeated.
        /// </example>
        internal static string GetWordSignature(string word)
        {
            var charIndexes = new Dictionary<char, List<int>>();

            for (int i = 0; i < word.Length; ++i)
            {
                char c = word[i];
                if (charIndexes.ContainsKey(c))
                {
                    charIndexes[c].Add(i);
                }
                else
                {
                    charIndexes[c] = new List<int> { i };
                }
            }

            IEnumerable<string> indexWithSameChars = charIndexes.Values.Where(l => l.Count > 1).Select(l => IndexListToString(l)).OrderBy(s => s);
            return $"{word.Length}[{string.Join(',', indexWithSameChars)}]";
        }

        internal static string IndexListToString(List<int> indexList)
        {
            return $"({string.Join(',', indexList)})";
        }
    }
}
