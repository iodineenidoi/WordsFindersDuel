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
        [SerializeField] private RectTransform flyingStars = null;
        [SerializeField] private TextChanger textChanger = null;
        [SerializeField] private Button cancelButton = null;
        [SerializeField] private Button playWithBotButton = null;

        private Action _cancelButtonPressedCallback = null;

        public void Show(Action callback = null, string roomName = null, bool showPlayWithBotButton = false)
        {
            // loadingCircles.StartLoading();
            flyingStars.gameObject.SetActive(true);
            textChanger.StartChanger();

            if (callback != null)
            {
                _cancelButtonPressedCallback = callback;
                cancelButton.gameObject.SetActive(true);
            }
            else
            {
                _cancelButtonPressedCallback = null;
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

            playWithBotButton.gameObject.SetActive(showPlayWithBotButton);
        }

        public void Hide()
        {
            // loadingCircles.StopLoading();
            textChanger.StopChanger();
            flyingStars.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            _cancelButtonPressedCallback = null;
            roomNameRoot.SetActive(false);
            roomNameText.text = string.Empty;
            playWithBotButton.gameObject.SetActive(false);
        }

        public void OnCancelButtonPressed()
        {
            try
            {
                _cancelButtonPressedCallback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}