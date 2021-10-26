using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Extensions;
using Entities;
using Gaming;
using Helpers;
using Photon.Pun;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bots
{
    public class GameWithBotController : MonoBehaviour
    {
        private const string BotDifficultyPlayerPrefsKey = "BOT_DIFFICULTY_KEY";
        public const string BotName = "bot";

        [SerializeField] private GameController gameController = null;
        [SerializeField] private AnagramsController anagramsController = null;
        [SerializeField] private GameRoot gameRoot = null;
        [SerializeField] private MessageBox messageBox = null;
        [SerializeField] private LettersGenerator lettersGenerator = null;

        private bool _isInGame = false;
        private string _currentLetters = "";
        private float _delayToUpdateLetters = -1f;
        private DamageController _damageController = null;
        private List<UsedWord> _usedWords = new List<UsedWord>();
        private BotController _bot = null;
        private BotDifficulty _botDifficulty = BotDifficulty.Medium;
        
        public BotDifficulty BotDifficulty
        {
            get => _botDifficulty;
            set
            {
                if (value == _botDifficulty)
                    return;

                _botDifficulty = value;
                Debug.Log($"Bot difficulty set: {_botDifficulty}");

                SaveSettings();
            }
        }
        
        public void PlayWithBot()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            _usedWords.Clear();
            _bot = new BotController(this, anagramsController, _botDifficulty, 1f);
            _damageController = new DamageController(PhotonNetwork.NickName, BotName);
            _damageController.OnPlayerIsDead += HandlePlayerIsDead;

            UpdateAnagramAndWords(() =>
            {
                gameController.GameType = GameType.Bot;
            
                gameRoot.Launch(_currentLetters, _damageController);
                _isInGame = true;
            });
        }

        public void EndWithBot()
        {
            _damageController = null;
            _bot = null;
            _isInGame = false;
        }

        public void TryWord(string word, string playerName)
        {
            if (_usedWords.Any(x => x.word == word) ||
                !anagramsController.AnagramWords.Contains(word))
                return;

            UsedWord newUsedWord = new UsedWord(word, playerName == PhotonNetwork.NickName);
            _usedWords.Add(newUsedWord);
            gameRoot.UpdateWordsScreen(_usedWords);
            _damageController.ChangePlayerHealth(playerName, word.Length, true);
        }

        private void UpdateAnagramAndWords(Action callback)
        {
            _currentLetters = _currentLetters.IsNullOrWhiteSpace() 
                ? lettersGenerator.GetUniqueRandomLetters(GameController.LettersToGenerateForAnagrams) 
                : lettersGenerator.UpdateRandomLetters(_currentLetters);

            StartCoroutine(anagramsController.UpdateAnagramWords(_currentLetters, () =>
            {
                _delayToUpdateLetters = Random.Range(7f, 13f);
                callback?.Invoke();
            }));
        }

        private void HandlePlayerIsDead(string playerName)
        {
            StartCoroutine(HandlePlayerIsDeadDelayed(playerName));
        }

        private IEnumerator HandlePlayerIsDeadDelayed(string playerName)
        {
            for (int i = 0; i < GameController.FramesToAnimateDeath; i++)
            {
                yield return null;
            }
            
            _isInGame = false;
            if (PhotonNetwork.NickName == playerName)
            {
                messageBox.Show(
                    "UI_You_Lose",
                    "UI_Ok",
                    PlayerIsDeadMessageBoxCallback);
            }
            else
            {
                messageBox.Show(
                    "UI_You_Won",
                    "UI_Great",
                    PlayerIsDeadMessageBoxCallback);
            }
        }

        private void PlayerIsDeadMessageBoxCallback()
        {
            EndWithBot();
            gameRoot.Stop();
        }

        #region PlayerPrefsMethods

        private void LoadSettings()
        {
            _botDifficulty = PlayerPrefs.HasKey(BotDifficultyPlayerPrefsKey)
                ? (BotDifficulty) PlayerPrefs.GetInt(BotDifficultyPlayerPrefsKey)
                : BotDifficulty.Medium;
            
            SaveSettings();
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt(BotDifficultyPlayerPrefsKey, (int) _botDifficulty);
            PlayerPrefs.Save();
        }

        #endregion

        #region MonoBehaviourCallbacks

        private void Awake()
        {
            LoadSettings();
        }

        private void Update()
        {
            if (!_isInGame) return;

            _bot.Update();
            
            _delayToUpdateLetters -= Time.deltaTime;
            if (_delayToUpdateLetters <= 0f)
            {
                _delayToUpdateLetters = float.MaxValue;
                UpdateAnagramAndWords(() =>
                {
                    gameRoot.UpdateLetters(anagramsController.CurrentLetters);
                });
            }
        }

        #endregion
    }
}