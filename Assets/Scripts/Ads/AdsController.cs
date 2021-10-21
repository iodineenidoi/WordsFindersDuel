using UnityEngine;
using UnityEngine.Advertisements;

namespace Ads
{
    public class AdsController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private const string GamesCountForAdPrefsKey = "GAMES_COUNT_FOR_AD_KEY";
        private const string CurrentGamesCountForAdPrefsKey = "CURRENT_GAMES_COUNT_FOR_AD_KEY";
        
        [SerializeField] private string androidGameId = "";
        [SerializeField] private string androidAdUnitId = "";
        [SerializeField] private bool testMode = true;
        [SerializeField] private bool enablePerPlacementMode = true;
        [SerializeField] private int minGamesForAd = 0;
        [SerializeField] private int maxGamesForAd = 3;

        private int _gamesLeftForShowAd = -1;

        public bool AdLoaded { get; private set; } = false;
        public int CurrentGamesCount { get; private set; } = -1;
        public bool IsReady => Advertisement.IsReady(androidAdUnitId);
        public bool IsMinAdsCount => CurrentGamesCount == minGamesForAd;
        public bool IsMaxAdsCount => CurrentGamesCount == maxGamesForAd;

        public void ShowAd()
        {
            if (CurrentGamesCount != 0 && --_gamesLeftForShowAd <= 0)
            {
                Advertisement.Show(androidAdUnitId, this);
                _gamesLeftForShowAd = CurrentGamesCount;
            }
            
            SavePrefs();
        }

        public void UpdateCurrentGamesCount(int delta)
        {
            CurrentGamesCount = Mathf.Min(maxGamesForAd, Mathf.Max(minGamesForAd, CurrentGamesCount + delta));
            _gamesLeftForShowAd = CurrentGamesCount;
            SavePrefs();
        }

        private void InitializeAds()
        {
            Advertisement.Initialize(androidGameId, testMode, enablePerPlacementMode, this);
        }

        private void LoadAd()
        {
            Debug.Log($"Loading Ad: {androidAdUnitId}");
            
            AdLoaded = false;
            Advertisement.Load(androidAdUnitId, this);
        }

        #region AdvertisementCallbacks

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");

            LoadAd();
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"Ad Loaded: {placementId}");

            if (placementId == androidAdUnitId)
            {
                AdLoaded = true;
            }
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log($"{nameof(OnUnityAdsShowComplete)}. State: {showCompletionState}");
            
            if (androidAdUnitId == placementId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Unity Ads Rewarded Ad Complete");
                LoadAd();
            }
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log(nameof(OnUnityAdsFailedToLoad) + $" Error: {error}. Message: {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log(nameof(OnUnityAdsShowFailure) + $" Error: {error}. Message: {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }
        
        #endregion

        #region PlayerPrefsMethods

        private void LoadPrefs()
        {
            CurrentGamesCount = PlayerPrefs.HasKey(GamesCountForAdPrefsKey) 
                ? PlayerPrefs.GetInt(GamesCountForAdPrefsKey) 
                : maxGamesForAd;

            _gamesLeftForShowAd = PlayerPrefs.HasKey(CurrentGamesCountForAdPrefsKey) 
                ? PlayerPrefs.GetInt(CurrentGamesCountForAdPrefsKey) 
                : CurrentGamesCount;
            
            SavePrefs();
        }
        
        private void SavePrefs()
        {
            PlayerPrefs.SetInt(GamesCountForAdPrefsKey, CurrentGamesCount);
            PlayerPrefs.SetInt(CurrentGamesCountForAdPrefsKey, _gamesLeftForShowAd);
            PlayerPrefs.Save();
        }

        #endregion

        #region MonoBehaviourCallbacks

        private void Awake()
        {
            Debug.Log("I started awake in ads controller");
            LoadPrefs();
            InitializeAds();
        }

        #endregion
    }
}