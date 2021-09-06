using System;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MessageBox : MonoBehaviour
    {
        [SerializeField] private GameObject messageBoxRoot = null;
        [SerializeField] private TextLocalizer messageText = null;
        [SerializeField] private TextLocalizer okButtonText = null;
        [SerializeField] private TextLocalizer greenButtonText = null;
        [SerializeField] private TextLocalizer redButtonText = null;
        [SerializeField] private GameObject okButton = null;
        [SerializeField] private GameObject greenButton = null;
        [SerializeField] private GameObject redButton = null;

        private Action _okButtonCallback = null;
        private Action _greenButtonCallback = null;
        private Action _redButtonCallback = null;

        public void OnOkButtonClicked()
        {
            _okButtonCallback?.Invoke();
        }

        public void OnGreenButtonClicked()
        {
            _greenButtonCallback?.Invoke();
        }

        public void OnRedButtonClicked()
        {
            _redButtonCallback?.Invoke();
        }
        
        public void Show(string messageTextValue, string buttonTextValue, Action buttonCallback)
        {
            if (messageBoxRoot.activeSelf)
                return;
            
            messageText.Localize(messageTextValue);
            
            okButtonText.Localize(buttonTextValue);
            greenButtonText.Localize(string.Empty);
            redButtonText.Localize(string.Empty);
            
            _okButtonCallback = buttonCallback;
            _greenButtonCallback = null;
            _redButtonCallback = null;
            
            okButton.SetActive(true);
            greenButton.SetActive(false);
            redButton.SetActive(false);
            
            messageBoxRoot.SetActive(true);
        }
        
        public void Show(string messageTextValue, string greenButtonTextValue, string redButtonTextValue, Action greenButtonCallback, Action redButtonCallback)
        {
            if (messageBoxRoot.activeSelf)
                return;
            
            messageText.Localize(messageTextValue);
            
            okButtonText.Localize(string.Empty);
            greenButtonText.Localize(greenButtonTextValue);
            redButtonText.Localize(redButtonTextValue);
            
            _okButtonCallback = null;
            _greenButtonCallback = greenButtonCallback;
            _redButtonCallback = redButtonCallback;
            
            okButton.SetActive(false);
            greenButton.SetActive(true);
            redButton.SetActive(true);
            
            messageBoxRoot.SetActive(true);
        }

        public void Hide()
        {
            if (!messageBoxRoot.activeSelf)
                return;

            messageText.Localize(string.Empty);
            
            okButtonText.Localize(string.Empty);
            greenButtonText.Localize(string.Empty);
            redButtonText.Localize(string.Empty);
            
            _okButtonCallback = null;
            _greenButtonCallback = null;
            _redButtonCallback = null;
            
            okButton.SetActive(false);
            greenButton.SetActive(false);
            redButton.SetActive(false);
            
            messageBoxRoot.SetActive(false);
        }
    }
}