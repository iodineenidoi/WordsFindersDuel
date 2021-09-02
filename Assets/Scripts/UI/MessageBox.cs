using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MessageBox : MonoBehaviour
    {
        [SerializeField] private GameObject messageBoxRoot = null;
        [SerializeField] private TMP_Text messageText = null;
        [SerializeField] private TMP_Text okButtonText = null;
        [SerializeField] private TMP_Text greenButtonText = null;
        [SerializeField] private TMP_Text redButtonText = null;
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
            
            messageText.text = messageTextValue;
            
            okButtonText.text = buttonTextValue;
            greenButtonText.text = string.Empty;
            redButtonText.text = string.Empty;
            
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
            
            messageText.text = messageTextValue;
            
            okButtonText.text = string.Empty;
            greenButtonText.text = greenButtonTextValue;
            redButtonText.text = redButtonTextValue;
            
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

            messageText.text = string.Empty;
            
            okButtonText.text = string.Empty;
            greenButtonText.text = string.Empty;
            redButtonText.text = string.Empty;
            
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