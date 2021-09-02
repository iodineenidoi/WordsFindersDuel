using System;
using Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WaitingScreen : MonoBehaviour
    {
        [SerializeField] private GameObject roomNameRoot = null;
        [SerializeField] private TMP_Text roomNameText = null;
        [SerializeField] private LoadingCircles loadingCircles = null;
        [SerializeField] private TextChanger textChanger = null;
        [SerializeField] private Button cancelButton = null;

        private Action cancelButtonPressedCallback = null;

        public void Show(Action callback = null, string roomName = null)
        {
            loadingCircles.StartLoading();
            textChanger.StartChanger();

            if (callback != null)
            {
                cancelButtonPressedCallback = callback;
                cancelButton.gameObject.SetActive(true);
            }
            else
            {
                cancelButtonPressedCallback = null;
                cancelButton.gameObject.SetActive(false);
            }

            if (roomName.IsNullOrWhiteSpace())
            {
                roomNameRoot.SetActive(false);
                roomNameText.text = string.Empty;
            }
            else
            {
                roomNameRoot.SetActive(true);
                roomNameText.text = roomName;
            }
        }

        public void Hide()
        {
            loadingCircles.StopLoading();
            textChanger.StopChanger();
            cancelButton.gameObject.SetActive(false);
            cancelButtonPressedCallback = null;
            roomNameRoot.SetActive(false);
            roomNameText.text = string.Empty;
        }

        public void OnCancelButtonPressed()
        {
            try
            {
                cancelButtonPressedCallback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}