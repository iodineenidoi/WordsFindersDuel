using System.Collections.Generic;
using System.Linq;
using Localization;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Words Collection")]
    public class WordsCollection : ScriptableObject
    {
        [SerializeField] private LocalizationLanguage language = LocalizationLanguage.English;
        [SerializeField] private string letters = "";
        [SerializeField] private List<string> allWords = new List<string>();

        public LocalizationLanguage Language => language;
        public string Letters => letters;

        public List<string> GetAnagramWords(string anagram)
        {
            anagram = new string(anagram.Where(ch => letters.Contains(ch)).ToArray());
            return allWords.Where(word => IsAnagram(anagram, word)).ToList();
        }

        private bool IsAnagram(string anagram, string word)
        {
            Dictionary<char, int> lettersCounter = new Dictionary<char, int>(anagram.Length + word.Length);
            for (int i = 0; i < anagram.Length; i++)
            {
                if (!lettersCounter.ContainsKey(anagram[i]))
                    lettersCounter[anagram[i]] = 0;

                lettersCounter[anagram[i]]++;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (!lettersCounter.ContainsKey(word[i]))
                    return false;

                lettersCounter[word[i]]--;
            }

            return lettersCounter.All(x => x.Value >= 0);
        }

#if UNITY_EDITOR

        public void ReplaceAllWords(List<string> newAllWords)
        {
            allWords = newAllWords;
        }

#endif
    }
}