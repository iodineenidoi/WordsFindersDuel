using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform settingsPanel = null;
        [SerializeField] private Image buttonImage = null;
        [SerializeField] private Sprite openButtonSprite = null;
        [SerializeField] private Sprite closeButtonSprite = null;
        
        private bool _isOpen = false;
        
        public void OnSettingsButtonClicked()
        {
            _isOpen = !_isOpen;
            settingsPanel.gameObject.SetActive(_isOpen);
            buttonImage.sprite = _isOpen ? closeButtonSprite : openButtonSprite;
        }
    }
}