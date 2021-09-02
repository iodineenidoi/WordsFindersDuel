using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gaming
{
    public class LetterButton : MonoBehaviour
    {
        [SerializeField] private LettersInputController lettersInputController = null;
        [SerializeField] private TMP_Text buttonLetterText = null;
        [SerializeField] private Button buttonLetter = null;

        private char _currentLetter = '=';
        
        public void SetButtonLetter(char letter)
        {
            _currentLetter = letter;
            buttonLetterText.text = letter.ToString();
        }

        public void OnClick()
        {
            if (!buttonLetter.interactable) 
                return;
            
            buttonLetter.interactable = false;
            lettersInputController.AddLetter(_currentLetter);
        }

        public void ResetButton()
        {
            buttonLetter.interactable = true;
        }
    }
}