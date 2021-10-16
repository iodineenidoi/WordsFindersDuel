using System.Collections;
using Localization;
using Networking;
using UI;
using UnityEngine;

namespace Core
{
    public class GameController : MonoBehaviour
    {
        public const int LettersToGenerateForAnagrams = 15;
        public const int FramesToAnimateDeath = 180;

        [SerializeField] private LobbyMenu lobbyMenu = null;
        [SerializeField] private ConnectionsController connectionsController = null;

        private bool _translationsLoaded = false;
        private bool _languageSet = false;
        
        public GameType GameType { get; set; }

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
            while (!_translationsLoaded || !_languageSet || !connectionsController.ConnectedToMaster)
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