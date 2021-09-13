using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Localization;
using UnityEngine;

namespace Networking
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Anagrams Collection")]
    public class AnagramsCollection : ScriptableObject
    {
        [SerializeField] private LocalizationLanguage language = LocalizationLanguage.English;
        [SerializeField] private List<string> anagrams = new List<string>();
        [SerializeField] private List<string> allWords = new List<string>();

        public LocalizationLanguage Language => language;
        public IReadOnlyList<string> Anagrams => anagrams.AsReadOnly();
        public IReadOnlyList<string> AllWords => allWords.AsReadOnly();

        public List<string> GetAnagramWords(string anagram)
        {
            return allWords;
        }

        public void ReplaceAnagramsAndAllWord(List<string> newAnagrams, List<string> newAllWords)
        {
            anagrams = newAnagrams;
            allWords = newAllWords;
        }

        public void AddNewAnagramsAndWords(List<string> newAnagrams, List<string> newWords)
        {
            HashSet<string> anagramsSet = new HashSet<string>(newAnagrams);
            foreach (string anagram in anagrams)
            {
                anagramsSet.Add(anagram);
            }
            
            HashSet<string> allWordsSet = new HashSet<string>(newWords);
            foreach (string word in allWords)
            {
                allWordsSet.Add(word);
            }

            anagrams = anagramsSet.ToList();
            allWords = allWordsSet.ToList();
        }
    }
}