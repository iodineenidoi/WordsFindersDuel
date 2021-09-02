namespace Entities
{
    public class UsedWord
    {
        public readonly string word;
        public readonly bool isMine;

        public UsedWord(string word, bool isMine)
        {
            this.word = word;
            this.isMine = isMine;
        }
    }
}