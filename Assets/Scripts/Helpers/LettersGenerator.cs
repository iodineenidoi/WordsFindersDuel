using System.Linq;
using UnityEngine;

namespace Helpers
{
    public static class LettersGenerator
    {
        private const string RuLetters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

        public static string GetUniqueRandomLetters(int length)
        {
            return new string(RuLetters.OrderBy(_ => Random.Range(0f, 1f)).ToArray())
                .Substring(0, Mathf.Min(RuLetters.Length, length));
        }

        public static string UpdateRandomLetters(string prevLetters)
        {
            string letters = null;
            do
            {
                letters = GetUniqueRandomLetters(prevLetters.Length);
            } 
            while (HasSameLetters(prevLetters, letters));

            return letters;
        }

        private static bool HasSameLetters(string first, string second)
        {
            return string.Concat(first.OrderBy(c => c)) == string.Concat(second.OrderBy(c => c));
        }
    }
}