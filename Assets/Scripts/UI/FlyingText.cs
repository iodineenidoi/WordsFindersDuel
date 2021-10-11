﻿using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class FlyingText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text = null;
        [SerializeField] private Vector2 startSize = Vector2.zero;
        [SerializeField] private Vector2 endSize = Vector2.zero;
        [SerializeField] private Vector2 startPos = Vector2.zero;
        [SerializeField] private float speed = 0.5f;

        private float _interpolater = 0f;
        
        public void Launch(string textValue)
        {
            if (text.gameObject.activeSelf)
            {
                OnEnable();
            }
            else
            {
                gameObject.SetActive(true);
            }

            text.text = textValue;
        }

        #region MonoBehaviourCallbacks

        private void OnEnable()
        {
            _interpolater = 0f;
            text.rectTransform.anchoredPosition = startPos;
            text.rectTransform.sizeDelta = startSize;
        }

        private void Update()
        {
            text.rectTransform.sizeDelta = Vector2.Lerp(startSize, endSize, _interpolater);
            _interpolater += speed * Time.deltaTime;

            text.rectTransform.anchoredPosition += Vector2.up * 0.5f;

            if (_interpolater >= 1f)
            {
                gameObject.SetActive(false);
            }
        }

        #endregion
    }
}