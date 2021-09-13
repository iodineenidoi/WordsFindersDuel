using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Gaming;
using Localization;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class AnagramsController : MonoBehaviour
    {
        [SerializeField] private AnagramsCollection[] anagramsCollections = { };
        
        public string CurrentLetters { get; private set; }
        public List<string> AnagramWords { get; private set; }

        public void UpdateAnagramWords(string letters)
        {
            AnagramsCollection anagramsCollection = anagramsCollections
                .First(x => x.Language == Localizer.Language);

            CurrentLetters = letters;
            AnagramWords = anagramsCollection.GetAnagramWords(letters);
        }
    }
}