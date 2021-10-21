using System.Collections.Generic;
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
        public List<string> AllWords => allWords;
        

#if UNITY_EDITOR

        public void ReplaceAllWords(List<string> newAllWords)
        {
            allWords = newAllWords;
        }

#endif
    }
}