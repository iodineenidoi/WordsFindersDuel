﻿using System;
using Ads;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AdsCountSettingsLine : MonoBehaviour
    {
        [SerializeField] private AdsController adsController = null;
        [SerializeField] private TextLocalizer valueText = null;
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
            valueText.Localize($"UI_Ads_Count_{adsController.CurrentGamesCount}");
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