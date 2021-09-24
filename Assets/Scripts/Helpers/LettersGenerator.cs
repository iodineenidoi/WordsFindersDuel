using System.Linq;
using Core;
using UnityEngine;

namespace Helpers
{
    public class LettersGenerator : MonoBehaviour
    {
        [SerializeField] private AnagramsController anagramsController = null;
        
        public string GetUniqueRandomLetters(int length)
        {
            string letters = anagramsController.CurrentWordsCollection.Letters;
            
            return new string(letters.OrderBy(_ => Random.Range(0f, 1f)).ToArray())
                .Substring(0, Mathf.Min(letters.Length, length));
        }

        public string UpdateRandomLetters(string prevLetters)
        {
            string letters;
            do
            {
                letters = GetUniqueRandomLetters(prevLetters.Length);
            } 
            while (HasSameLetters(prevLetters, letters));

            return letters;
        }

        private bool HasSameLetters(string first, string second)
        {
            return string.Concat(first.OrderBy(c => c)) == string.Concat(second.OrderBy(c => c));
        }
    }
}