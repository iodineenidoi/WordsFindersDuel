using System;
using System.Text;
using Localization;
using Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Gaming
{
    public class LettersInputController : MonoBehaviour
    {
        [SerializeField] private NetworkController networkController = null;
        [SerializeField] private Button enterButton = null;
        [SerializeField] private TextLocalizer enterButtonText = null;
        [SerializeField] private LetterButton[] letterButtons = new LetterButton[0];

        private StringBuilder _stringBuilder = new StringBuilder();

        public void SendWord()
        {
            networkController.SendWord(_stringBuilder.ToString());
            ResetInput();
        }
        
        public void SetNewLetters(string letters)
        {
            if (letters.Length != letterButtons.Length)
                throw new ArgumentException("Letters count is not equal to buttons count!");
            
            for (int i = 0; i < letterButtons.Length; i++)
            {
                letterButtons[i].SetButtonLetter(letters[i]);
            }

            ResetInput();
        }
        
        public void AddLetter(char letter)
        {
            _stringBuilder.Append(letter);
            UpdateEnterButton();
        }

        public void ResetInput()
        {
            _stringBuilder.Clear();
            for (int i = 0; i < letterButtons.Length; i++)
            {
                letterButtons[i].ResetButton();
            }

            UpdateEnterButton();
        }

        public void UpdateEnterButton()
        {
            if (_stringBuilder.Length == 0)
            {
                enterButton.interactable = false;
                enterButtonText.Localize("UI_Enter");
            }
            else
            {
                enterButton.interactable = true;
                enterButtonText.Localize(_stringBuilder.ToString());
            }
        }
    }
}