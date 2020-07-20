using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CipherCrack
{
    /// <summary>
    /// Encodes the English vocabulary. Words are grouped by their signature (see Helpers class) to make it easier
    /// to get the candidate words per ciphered sequence.
    /// </summary>
    public class Vocabulary
    {
        private Dictionary<string, HashSet<string>> vocabularyBySignature;

        private Vocabulary() { }

        public static Vocabulary FromRaw(IEnumerable<string> rawContents)
        {
            Vocabulary instance = new Vocabulary();

            instance.vocabularyBySignature = new Dictionary<string, HashSet<string>>();

            foreach (string word in rawContents)
            {
                string wordSignature = Helpers.GetWordSignature(word);

                HashSet<string> wordListBySignature = instance.vocabularyBySignature.GetValueOrDefault(wordSignature, null);
                if (wordListBySignature == null)
                {
                    wordListBySignature = new HashSet<string> { word };
                    instance.vocabularyBySignature[wordSignature] = wordListBySignature;
                }
                else
                {
                    wordListBySignature.Add(word);
                }
            }

            return instance;
        }

        public HashSet<string> this[string s]
        {
            get { return vocabularyBySignature[s]; }
        }
    }
}
