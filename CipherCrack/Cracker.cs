using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CipherCrack
{
    public class Cracker
    {
        CipherPhrase cipherPhrase;
        Dictionary<int, HashSet<char>> cipherMapping;

        public Cracker()
        {
            this.GetCipherPhrase();
            this.InitializeCipherMapping();
        }

        private void GetCipherPhrase()
        {
            IEnumerable<string> words = File.ReadAllLines("EnglishWords.txt").Where(s => !s.EndsWith("'s"));
            var vocabulary = Vocabulary.FromRaw(words);

            // From gloomhaven. The text is an array of strings, each of which encodes ciphered characters as numbers, separated by spaces
            string[] cipherWords = new string[]
            {
                // Message 1
                "1 2 3", "4 5 3 6 1 7 5", "2 6 8", "9 6 10 3", "6", "5 3 11 12 3 8 1", "13 7 5",
                "7 12 5", "8 14 7 15 16 8", "1 2 3", "1 17 3 16 13 1 2", "16 3 1 1 3 5", "2 7 16 10 8",
                "1 2 3", "18 3 19",
                // Message 2
                // Sentence 1
                "17 3", "4 6 16 16", "13 5 7 9", "1 2 3", "10 12 8 1", "13 5 7 9", "1 2 3", "6 20 3 10",
                "21 7 22 3 8", "7 13", "1 2 7 8 3", "19 7 12", "2 6 23 3", "18 15 16 16 3 10",
                // Sentence 2
                "8 14 3 6 18", "7 12 5", "22 6 9 3", "15 22 1 7", "2 15 8", "17 3 21", "6 22 10", "17 3", "17 15 16 16", "21 3", "13 5 3 3",
                // Sentence 3
                "4 16 12 3 8", "19 7 12", "9 12 8 1", "13 15 22 10", "16 3 1 1 3 5 8", "1 7", "7 12 5", "22 6 9 3",
                // Sentence 4
                "2 3 5 3", "15 8", "7 12 5", "13 15 5 8 1"
            };
            this.cipherPhrase = new CipherPhrase(cipherWords, vocabulary);
        }

        /// <summary>
        /// Initializes the cyphertext to plaintext mapping. Initially, one ciphered character can map to any plaintext character.
        /// </summary>
        private void InitializeCipherMapping()
        {
            var alphabet = new List<char>(26);
            for (char c = 'a'; c <= 'z'; ++c)
            {
                alphabet.Add(c);
            }

            this.cipherMapping = new Dictionary<int, HashSet<char>>();
            foreach (int cipherChar in this.cipherPhrase.GetDistinctChars())
            {
                this.cipherMapping[cipherChar] = new HashSet<char>(alphabet);
            }
        }

        public void Crack()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // Check if any words have 0 candidates
                this.cipherPhrase.CheckIfWordsHaveCandidates();

                // Prune cipher mapping according to the candidate words per cipherword
                bool prunedCipherMapping = this.PruneCipherMapping();

                // Prune the list of words, according to the possible character mappings
                bool prunedWordList = this.PruneWordList();

                // If a mapping has been defined, ensure the other cipher characters don't map to the same plaintext one
                bool prunedWordListForUniqueness = this.PruneMappingForCharacterUniqueness();

                // Keep going while a prune happened
                keepGoing = prunedCipherMapping || prunedWordList || prunedWordListForUniqueness;
            }

            Console.WriteLine(this.cipherPhrase);
        }

        private bool PruneWordList()
        {
            bool changed = false;

            foreach (CipherWord word in this.cipherPhrase.CipherWords)
            {

                for (int i = 0; i < word.CipherChars.Count; ++i)
                {
                    HashSet<char> acceptableChars = this.cipherMapping[word.CipherChars[i]];
                    List<string> candidatesToRemove = new List<string>();
                    foreach (string candidate in word.WordCandidates)
                    {
                        if(!acceptableChars.Contains(candidate[i]))
                        {
                            candidatesToRemove.Add(candidate);
                            changed = true;
                        }
                    }

                    word.WordCandidates.ExceptWith(candidatesToRemove);
                }
            }

            return changed;
        }

        private bool PruneCipherMapping()
        {
            bool changed = false;

            foreach (CipherWord word in this.cipherPhrase.CipherWords)
            {
                
                for (int i = 0; i < word.CipherChars.Count; ++i)
                {
                    var charsToPrune = new HashSet<char>(this.cipherMapping[word.CipherChars[i]]);
                    foreach (string candidate in word.WordCandidates)
                    {
                        charsToPrune.Remove(candidate[i]);
                    }
                    if (charsToPrune.Count > 0)
                    {
                        changed = true;
                    }
                    this.cipherMapping[word.CipherChars[i]].ExceptWith(charsToPrune);
                }
            }

            return changed;
        }

        private bool PruneMappingForCharacterUniqueness()
        {
            bool shouldKeepPruning = true;
            bool changed = false;
            while (shouldKeepPruning)
            {
                shouldKeepPruning = false;
                IEnumerable<KeyValuePair<int, HashSet<char>>> definedMappings = this.cipherMapping.Where((kvp) => kvp.Value.Count == 1);

                foreach (var mapping in this.cipherMapping)
                {
                    foreach (var definedMapping in definedMappings)
                    {
                        if (definedMapping.Key != mapping.Key && mapping.Value.Contains(definedMapping.Value.Single()))
                        {
                            mapping.Value.Remove(definedMapping.Value.Single());
                            if(mapping.Value.Count == 1)
                            {
                                shouldKeepPruning = true;
                            }
                            changed = true;
                        }
                    }
                }
            }
            return changed;
        }
    }
}
