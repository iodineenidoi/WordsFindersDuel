using UnityEngine;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform settingsPanel = null;
        
        private bool _isOpen = false;
        
        public void OnSettingsButtonClicked()
        {
            _isOpen = !_isOpen;
            settingsPanel.gameObject.SetActive(_isOpen);
        }
    }
}