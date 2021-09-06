using System;
using System.Collections.Generic;

namespace Localization
{
    [Serializable]
    public class KeyTranslations
    {
        public string key;
        public List<LocalizationLanguage> languages;
        public List<string> values;
    }
}