using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Photon.Pun;
using UnityEngine;

namespace Localization
{
    public class Localizer : MonoBehaviour
    {
        private const string SAVED_LOCALIZATION_KEY = "SAVED_LOCALIZATION_KEY";

        [SerializeField] private TranslationsCollection[] _translationsCollections = { };
        
        private int _currentLanguageIndex = -1;
        private List<LocalizationLanguage> _languages = new List<LocalizationLanguage>
            {LocalizationLanguage.English, LocalizationLanguage.Russian};

        private static Dictionary<string, KeyTranslations> _translations = new Dictionary<string, KeyTranslations>();

        public static LocalizationLanguage Language { get; private set; }
        public static event Action OnLanguageChanged;
        public static event Action OnTranslationsLoaded;

        public static string GetTranslation(string key)
        {
            return _translations.ContainsKey(key) 
                ? _translations[key].values[(int) Language] 
                : key;
        }

        public void SetNextLanguage()
        {
            _currentLanguageIndex += 1;
            if (_currentLanguageIndex >= _languages.Count)
            {
                _currentLanguageIndex = 0;
            }

            SetLanguage(_languages[_currentLanguageIndex]);
        }

        private void SetLanguage(LocalizationLanguage language, bool saveLanguage = true)
        {
            Language = language;
            
            OnLanguageChanged?.Invoke();
            _currentLanguageIndex = (int) Language;
            Debug.Log($"Current language is {Language}");
            
            if (saveLanguage)
            {
                PlayerPrefs.SetInt(SAVED_LOCALIZATION_KEY, _currentLanguageIndex);
                PlayerPrefs.Save();
            }
        }
        
        private IEnumerator SetLanguageAfterPhotonConnected()
        {
            while (PhotonNetwork.CloudRegion.IsNullOrEmpty())
            {
                yield return null;
            }

            switch (PhotonNetwork.CloudRegion)
            {
                case "ru":
                    _currentLanguageIndex = (int) LocalizationLanguage.Russian;
                    break;
                    
                default:
                    _currentLanguageIndex = (int) LocalizationLanguage.English;
                    break;
            }
            
            SetLanguage(_languages[_currentLanguageIndex]);
        }

        private KeyTranslations[] GetKeyTranslations()
        {
            return _translationsCollections
                .SelectMany(x => x.TranslationsList)
                .ToArray();
        }

        private void UpdateTranslationsDict()
        {
            _translations.Clear();
            foreach (KeyTranslations translation in GetKeyTranslations())
            {
                _translations.Add(translation.key, translation);
            }
            OnTranslationsLoaded?.Invoke();
        }

        private void SetLanguageOnGameLoaded()
        {
            if (PlayerPrefs.HasKey(SAVED_LOCALIZATION_KEY))
            {
                _currentLanguageIndex = PlayerPrefs.GetInt(SAVED_LOCALIZATION_KEY);
                SetLanguage(_languages[_currentLanguageIndex], false);
                OnTranslationsLoaded?.Invoke();
            }
            else
            {
                StartCoroutine(SetLanguageAfterPhotonConnected());
            }
        }
        
        #region MonoBehaviourCallbacks

        private void Start()
        {
            UpdateTranslationsDict();
            SetLanguageOnGameLoaded();
        }

        #endregion
    }
}