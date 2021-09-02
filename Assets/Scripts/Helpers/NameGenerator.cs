using System;
using System.Linq;

namespace Helpers
{
    public static class NameGenerator
    {
        public static string Get(int minLength = 5, int maxLength = 11)
        {
            string guidLetters = GetStringFromGuid(Guid.NewGuid());
            while (guidLetters.Length < minLength)
            {
                guidLetters += GetStringFromGuid(Guid.NewGuid());
            }

            if (guidLetters.Length > maxLength)
            {
                guidLetters = guidLetters.Substring(0, maxLength);
            }

            return guidLetters.ToLowerInvariant();
        }

        private static string GetStringFromGuid(Guid guid)
        {
            return GetCustomStringFromGuid(guid, char.IsLetterOrDigit);
        }

        private static string GetCustomStringFromGuid(Guid guid, Predicate<char> charsSelector)
        {
            return new string(guid.ToString().Where(x => charsSelector(x)).ToArray());
        }
    }
}