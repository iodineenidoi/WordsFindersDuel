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

        public void UpdateAnagramWords(string letters)
        {
            CurrentLetters = letters;
            AnagramWords = CurrentWordsCollection.GetAnagramWords(letters);
        }
    }
}