using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherCrack
{
    public class CipherPhrase
    {
        public List<CipherWord> CipherWords { get; }

        public CipherPhrase(string[] cipherWords, Vocabulary vocabulary)
        {
            this.CipherWords = cipherWords.Select(cw => new CipherWord(cw, vocabulary)).ToList();
        }

        public HashSet<int> GetDistinctChars()
        {
            var hashSet = new HashSet<int>();
            foreach (CipherWord word in this.CipherWords)
            {
                hashSet.UnionWith(word.CipherChars);
            }
            return hashSet;
        }

        public void CheckIfWordsHaveCandidates()
        {
            foreach (CipherWord word in this.CipherWords)
            {
                if (word.WordCandidates.Count == 0)
                {
                    throw new Exception($"Cipherword {string.Join(' ', word.CipherChars)} has no word candidates");
                }
            }
        }

        public override string ToString()
        {
            return string.Join(' ', this.CipherWords);
        }
    }

    /// <summary>
    /// Defineses a ciphered word, which is a sequence of ciphered characters.
    /// </summary>
    public class CipherWord
    {
        public List<int> CipherChars { get; }

        // Note: it would probably be more efficient to use some sort of prefix tree structure to hold
        // the candidates. However, the way it is, perf is surprisingly good, so this simpler implementation will do.
        public  HashSet<string> WordCandidates { get; }

        public CipherWord(string word, Vocabulary vocabulary)
        {
            this.CipherChars = word.Split(" ").Select(c => int.Parse(c)).ToList();
            this.WordCandidates = new HashSet<string>(vocabulary[this.GetWordSignature()]);
        }

        public string GetWordSignature()
        {
            // The helper method works on characters, and in this class we encode ciphered characters as numbers
            // So encode these numbers to single chars to get the word signature
            var cipherAsReadableChar = new string(CipherChars.Select(i => (char)('a' + i)).ToArray());

            return Helpers.GetWordSignature(cipherAsReadableChar);
        }

        public override string ToString()
        {
            if(this.WordCandidates.Count > 1)
            {
                return "{" + string.Join(", ", this.WordCandidates) + "}";
            }
            else
            {
                return this.WordCandidates.Single();
            }
        }

    }


}
