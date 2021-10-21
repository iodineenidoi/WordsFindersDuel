using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localization;
using UnityEngine;

namespace Core
{
    public class AnagramsController : MonoBehaviour
    {
        [SerializeField] private WordsCollection[] anagramsCollections = { };
        
        public string CurrentLetters { get; private set; }
        public List<string> AnagramWords { get; private set; }

        public WordsCollection CurrentWordsCollection => anagramsCollections
            .First(x => x.Language == Localizer.Language);

        public IEnumerator UpdateAnagramWords(string letters, Action callback)
        {
            CurrentLetters = letters;
            yield return StartCoroutine(UpdateAnagramWords(letters, CurrentWordsCollection.AllWords));
            Debug.Log($"Letters: {CurrentLetters}. Words count: {AnagramWords.Count}");
            
            callback?.Invoke();
        }

        private IEnumerator UpdateAnagramWords(string letters, List<string> allWords)
        {
            List<string> results = new List<string>();

            for (int i = 0; i < allWords.Count; i++)
            {
                if (IsAnagram(letters, allWords[i]))
                {
                    results.Add(allWords[i]);
                }
                
                if (i % 500 == 0)
                {
                    yield return null;
                }
            }

            AnagramWords = results;
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
    }
}