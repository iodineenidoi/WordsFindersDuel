namespace Core.WordsValidators
{
    public class LettersLengthRangedWordsValidator : IWordsValidator
    {
        private int _minLen;
        private int _maxLen;

        public LettersLengthRangedWordsValidator(int minLen, int maxLen)
        {
            _minLen = minLen;
            _maxLen = maxLen;
        }

        public bool ValidateWord(string word)
        {
            return word.Length >= _minLen && word.Length <= _maxLen;
        }
    }
}