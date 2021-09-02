using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingCircles : MonoBehaviour
    {
        [SerializeField] private Image firstCircle = null;
        [SerializeField] private Image secondCircle = null;
        [SerializeField, Range(0.01f, 1f)] private float animationSpeed = 0.05f;
        [SerializeField] private bool useDeltaTime = false;

        private bool _processFirstCircle = false;
        private Coroutine _loadingCoroutine = null;
        
        public void StartLoading()
        {
            gameObject.SetActive(true);
        }

        public void StopLoading()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator UpdateLoadingProcess()
        {
            firstCircle.fillAmount = 0f;
            secondCircle.fillAmount = 0f;
            _processFirstCircle = true;
            
            while (true)
            {
                if (_processFirstCircle)
                {
                    UpdateFirstCircle();
                }
                else
                {
                    UpdateSecondCircle();
                }

                yield return null;
            }
        }

        private void UpdateFirstCircle()
        {
            firstCircle.fillAmount += GetDelta();

            if (Mathf.Approximately(firstCircle.fillAmount, 1f))
            {
                _processFirstCircle = false;
                firstCircle.fillAmount = 0f;
                secondCircle.fillAmount = 1f;
            }
        }

        private void UpdateSecondCircle()
        {
            secondCircle.fillAmount -= GetDelta();

            if (Mathf.Approximately(secondCircle.fillAmount, 0f))
            {
                _processFirstCircle = true;
            }
        }

        private float GetDelta()
        {
            return useDeltaTime ? animationSpeed * Time.deltaTime : animationSpeed;
        }

        #region MonoBehaviourCallbacks

        private void OnEnable()
        {
            if (_loadingCoroutine != null)
            {
                StopCoroutine(_loadingCoroutine);
                _loadingCoroutine = null;
            }

            _loadingCoroutine = StartCoroutine(UpdateLoadingProcess());
        }

        private void OnDisable()
        {
            if (_loadingCoroutine != null)
            {
                StopCoroutine(_loadingCoroutine);
                _loadingCoroutine = null;
            }
        }

        #endregion
    }
}