using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ads;
using Bots;
using Core;
using Entities;
using Localization;
using Photon.Pun;
using TMPro;
using UI;
using UnityEngine;

namespace Gaming
{
    public class GameRoot : MonoBehaviour
    {
        [SerializeField] private int framesToSkipBeforeRunDeathAnimation = 60;
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
        [SerializeField] private PlayersAnimationsController playersAnimationsController = null;
        [SerializeField] private AdsController adsController = null;
        [SerializeField] private BackgroundController backgroundController = null;

        private readonly List<UsedWord> _emptyList = new List<UsedWord>();
        private int _firstPlayerHealth = -1;
        private int _secondPlayerHealth = -1;

        public void Launch(string firstLetters, DamageController damageController)
        {
            SetupDamageController(damageController);
            SetupAnimations();
            UpdateLetters(firstLetters);
            UpdateWordsScreen(_emptyList);
            canvas.SetActive(true);
            backgroundController.SetBattleBackground();
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
            backgroundController.SetLobbyBackground();
            
            if (gameController.GameType == GameType.Bot)
            {
                gameWithBotController.EndWithBot();
            }

            gameController.GameType = GameType.None;

            if (adsController.AdLoaded && adsController.IsReady)
            {
                adsController.ShowAd();
            }
        }
        
        public void UpdateWordsScreen(List<UsedWord> words)
        {
            StringBuilder builder = new StringBuilder(words.Count);
            foreach (UsedWord word in words)
            {
                string localizationKey = word.isMine
                    ? "UI_You_Used_Spell"
                    : "UI_Enemy_Used_Spell";

                string newLine = string.Format(Localizer.GetTranslation(localizationKey), word.word, word.word.Length);
                builder.AppendLine(newLine);
            }

            wordsScreenText.text = builder.ToString();
        }

        public void UpdateLetters(string letters)
        {
            lettersInputController.SetNewLetters(letters);
        }

        private void SetupAnimations()
        {
            playersAnimationsController.StartPlayersAnimators();
        }

        private void SetupDamageController(DamageController damageController)
        {
            _firstPlayerHealth = damageController.FirstGamePlayer.Health;
            _secondPlayerHealth = damageController.SecondGamePlayer.Health;
            playersHealthController.ResetPlayers(damageController);
            damageController.OnPlayersHealthChanged += HandlePlayersHealthChanged;
            damageController.OnPlayerIsDead += HandlePlayerIsDead;
        }

        private void HandlePlayerIsDead(string playerName)
        {
            StartCoroutine(HandlePlayerIsDeadDelayed(playerName, framesToSkipBeforeRunDeathAnimation));
        }

        private IEnumerator HandlePlayerIsDeadDelayed(string playerName, int framesDelay)
        {
            while (framesDelay-- > 0)
            {
                yield return null;
            }
            
            playersAnimationsController.SetTrigger(PlayerAnimationType.Die, playerName == PhotonNetwork.NickName);
        }

        private void HandlePlayersHealthChanged(int firstPlayerHealth, int secondPlayerHealth)
        {
            if (_firstPlayerHealth != firstPlayerHealth)
            {
                playersAnimationsController.SetTrigger(PlayerAnimationType.Attack, false);
            }
            
            if (_secondPlayerHealth != secondPlayerHealth)
            {
                playersAnimationsController.SetTrigger(PlayerAnimationType.Attack, true);
            }

            _firstPlayerHealth = firstPlayerHealth;
            _secondPlayerHealth = secondPlayerHealth;
        }
    }
}