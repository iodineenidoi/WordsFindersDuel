using System;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TextChanger : MonoBehaviour
    {
        [SerializeField] private TextLocalizer tmpText = null;
        [SerializeField] private float interval = 1f;
        [SerializeField] private string[] textsList = new string[0];

        private int _currentTextIndex = -1;
        private float _timeLeftToChangeText = -1f;
        private bool _isInProcess = false;

        public void StartChanger(bool startFromFirst = false)
        {
            if (_isInProcess) return;

            EnableText(true);

            if (startFromFirst ||
                _currentTextIndex < 0 ||
                _currentTextIndex >= textsList.Length)
            {
                _currentTextIndex = 0;
            }

            _timeLeftToChangeText = interval;
            SetText(textsList[_currentTextIndex]);

            _isInProcess = true;
        }

        public void StopChanger()
        {
            if (!_isInProcess) return;

            EnableText(false);
            _timeLeftToChangeText = -1f;

            _isInProcess = false;
        }

        #region MonoBehaviourCallbacks

        private void Update()
        {
            if (!_isInProcess) return;

            _timeLeftToChangeText -= Time.deltaTime;
            if (_timeLeftToChangeText > 0f) return;

            _timeLeftToChangeText += interval;
            _currentTextIndex += 1;
            if (_currentTextIndex >= textsList.Length)
                _currentTextIndex = 0;

            SetText(textsList[_currentTextIndex]);
        }

        #endregion

        private void SetText(string text)
        {
            tmpText.Localize(text);
        }

        private void EnableText(bool enable)
        {
            tmpText.gameObject.SetActive(enable);
        }
    }
}