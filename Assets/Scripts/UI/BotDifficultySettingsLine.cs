using Bots;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BotDifficultySettingsLine : MonoBehaviour
    {
        [SerializeField] private GameWithBotController gameWithBotController = null;
        [SerializeField] private TextLocalizer valueText = null;
        [SerializeField] private Button minusButton = null;
        [SerializeField] private Button plusButton = null;

        
        public void OnMinusButtonClicked()
        {
            ProcessButtonClicked(-1);
        }

        public void OnPlusButtonClicked()
        {
            ProcessButtonClicked(1);
        }

        private void ProcessButtonClicked(int delta)
        {
            gameWithBotController.BotDifficulty += delta;
            UpdateText();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            plusButton.interactable = gameWithBotController.BotDifficulty != BotDifficulty.Hard;
            minusButton.interactable = gameWithBotController.BotDifficulty != BotDifficulty.Easy;
        }
        
        private void UpdateText()
        {
            valueText.Localize($"UI_Bot_{gameWithBotController.BotDifficulty}");
        }

        #region MonoBehaviourCallbacks

        private void OnEnable()
        {
            UpdateText();
            UpdateButtons();
        }

        #endregion
    }
}