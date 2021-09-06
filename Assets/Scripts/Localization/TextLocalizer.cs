using System;
using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextLocalizer : MonoBehaviour
    {
        [SerializeField] private string localizationKey = "";
        
        private object[] _localizationArgs = null;
        private TMP_Text _text = null;

        public void Localize(string key, params object[] args)
        {
            localizationKey = key;
            _localizationArgs = args ?? new object[0];
            _text.text = string.Format(Localizer.GetTranslation(key), _localizationArgs);
        }

        private void OnLanguageChanged()
        {
            Localize(localizationKey, _localizationArgs);
        }

        #region MonoBehaviourCallbacks

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            OnLanguageChanged();
            Localizer.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            Localizer.OnLanguageChanged -= OnLanguageChanged;
        }

        #endregion
    }
}