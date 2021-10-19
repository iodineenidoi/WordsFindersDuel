using UnityEngine;
using UnityEngine.Advertisements;

namespace Ads
{
    public class AdsController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] private string androidGameId = "";
        [SerializeField] private string androidAdUnitId = "";
        [SerializeField] private bool testMode = true;
        [SerializeField] private bool enablePerPlacementMode = true;

        public bool AdLoaded { get; private set; } = false;
        public bool IsReady => Advertisement.IsReady(androidAdUnitId);
        public bool IsGameToShow => true;
        
        public void ShowAd()
        {
            Advertisement.Show(androidAdUnitId, this);
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


        #region MonoBehaviourCallbacks

        private void Awake()
        {
            Debug.Log("I started awake in ads controller");
            InitializeAds();
        }

        #endregion
    }
}