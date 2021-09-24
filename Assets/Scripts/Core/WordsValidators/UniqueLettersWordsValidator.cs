using System.Linq;

namespace Core.WordsValidators
{
    public class UniqueLettersWordsValidator : IWordsValidator 
    {
        public bool ValidateWord(string word)
        {
            return word.ToLowerInvariant().GroupBy(x => x).Count() == word.Length;
        }
    }
}