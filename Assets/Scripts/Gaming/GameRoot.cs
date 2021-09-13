using System.Collections.Generic;
using System.Text;
using Bots;
using Core;
using Entities;
using Photon.Pun;
using TMPro;
using UI;
using UnityEngine;

namespace Gaming
{
    public class GameRoot : MonoBehaviour
    {
        [SerializeField] private GameController gameController = null;
        [SerializeField] private GameWithBotController gameWithBotController = null;
        [SerializeField] private LettersInputController lettersInputController = null;
        [SerializeField] private PlayersHealthController playersHealthController = null;
        [SerializeField] private GameObject canvas = null;
        [SerializeField] private string myWordColorHex = "00FF00FF";
        [SerializeField] private string allyWordColorHex = "FF0000FF";
        [SerializeField] private TMP_Text wordsScreenText = null;
        [SerializeField] private MessageBox messageBox = null;
        [SerializeField] private LobbyMenu lobbyMenu = null;

        private readonly List<UsedWord> _emptyList = new List<UsedWord>();
        
        public void Launch(string firstLetters, DamageController damageController)
        {
            playersHealthController.ResetPlayers(damageController);
            UpdateLetters(firstLetters);
            UpdateWordsScreen(_emptyList);
            canvas.SetActive(true);
        }

        public void OnExitButtonClicked()
        {
            messageBox.Show(
                "UI_Wanna_Leave_Game_Question",
                "UI_No",
                "UI_Yes",
                () => {},
                Stop);
        }

        public void Stop()
        {
            PhotonNetwork.LeaveRoom();
            lobbyMenu.ShowMainMenu();
            canvas.SetActive(false);

            if (gameController.GameType == GameType.Bot)
            {
                gameWithBotController.EndWithBot();
            }

            gameController.GameType = GameType.None;
        }
        
        public void UpdateWordsScreen(List<UsedWord> words)
        {
            StringBuilder builder = new StringBuilder(words.Count);
            foreach (UsedWord word in words)
            {
                builder.AppendLine($"<color=#{(word.isMine ? myWordColorHex : allyWordColorHex)}>{word.word}</color>");
            }

            wordsScreenText.text = builder.ToString();
        }

        public void UpdateLetters(string letters)
        {
            lettersInputController.SetNewLetters(letters);
        }
    }
}