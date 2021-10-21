using System;
using Ads;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AdsCountSettingsLine : MonoBehaviour
    {
        [SerializeField] private TMP_Text valueText = null;
        [SerializeField] private AdsController adsController = null;
        [SerializeField] private Button minusButton = null;
        [SerializeField] private Button plusButton = null;

        
        public void OnMinusButtonClicked()
        {
            ProcessButtonClicked(-1);
        }

        public void OnPlusButtonClicked()
        {
            ProcessButtonClicked(1);
        }

        private void ProcessButtonClicked(int delta)
        {
            adsController.UpdateCurrentGamesCount(delta);
            UpdateText();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            plusButton.interactable = !adsController.IsMaxAdsCount;
            minusButton.interactable = !adsController.IsMinAdsCount;
        }
        
        private void UpdateText()
        {
            valueText.text = adsController.CurrentGamesCount.ToString();
        }

        #region MonoBehaviourCallbacks

        private void OnEnable()
        {
            UpdateText();
            UpdateButtons();
        }

        #endregion
    }
}