using System;
using System.Collections;
using Localization;
using UI;
using UnityEngine;

namespace Core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private LobbyMenu lobbyMenu = null;

        private bool _translationsLoaded = false;
        private bool _languageSet = false;
        
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private IEnumerator StartGameAfterLocalizationDone()
        {
            while (!_translationsLoaded || !_languageSet)
            {
                yield return null;
            }
            
            lobbyMenu.ShowMainMenu();
        }

        private void OnTranslationsLoadedHandler()
        {
            _translationsLoaded = true;
        }

        private void OnLanguageChangedHandler()
        {
            _languageSet = true;
        }

        #region MonoBehaviourCallbacks

        private void Awake()
        {
            StartCoroutine(StartGameAfterLocalizationDone());
        }

        private void OnEnable()
        {
            Localizer.OnTranslationsLoaded += OnTranslationsLoadedHandler;
            Localizer.OnLanguageChanged += OnLanguageChangedHandler;
        }

        private void OnDisable()
        {
            Localizer.OnTranslationsLoaded -= OnTranslationsLoadedHandler;
            Localizer.OnLanguageChanged -= OnLanguageChangedHandler;
        }

        #endregion
    }
}