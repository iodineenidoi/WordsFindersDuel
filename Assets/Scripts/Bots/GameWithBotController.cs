﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Extensions;
using Entities;
using Gaming;
using Helpers;
using Networking;
using Photon.Pun;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bots
{
    public class GameWithBotController : MonoBehaviour
    {
        public const string BotName = "bot";

        [SerializeField] private GameController gameController = null;
        [SerializeField] private AnagramsController anagramsController = null;
        [SerializeField] private GameRoot gameRoot = null;
        [SerializeField] private MessageBox messageBox = null;

        private bool _isInGame = false;
        private string _currentLetters = "";
        private float _delayToUpdateLetters = -1f;
        private DamageController _damageController = null;
        private List<UsedWord> _usedWords = new List<UsedWord>();
        private BotController _bot = null;
        
        public void PlayWithBot()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            _usedWords.Clear();
            _bot = new BotController(this, anagramsController, 1f);
            _damageController = new DamageController(PhotonNetwork.NickName, BotName);
            _damageController.OnPlayerIsDead += HandlePlayerIsDead;

            UpdateAnagramAndWords();
            gameController.GameType = GameType.Bot;
            
            gameRoot.Launch(_currentLetters, _damageController);
            _isInGame = true;
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

        private void UpdateAnagramAndWords()
        {
            _currentLetters = _currentLetters.IsNullOrWhiteSpace() 
                ? LettersGenerator.GetUniqueRandomLetters(GameController.LettersToGenerateForAnagrams) 
                : LettersGenerator.UpdateRandomLetters(_currentLetters);

            anagramsController.UpdateAnagramWords(_currentLetters);
            _delayToUpdateLetters = Random.Range(7f, 13f);
        }

        private void HandlePlayerIsDead(string playerName)
        {
            _isInGame = false;
            if (PhotonNetwork.NickName == playerName)
            {
                messageBox.Show(
                    "UI_You_Won",
                    "UI_Great",
                    PlayerIsDeadMessageBoxCallback);
            }
            else
            {
                messageBox.Show(
                    "UI_You_Lose",
                    "UI_Ok",
                    PlayerIsDeadMessageBoxCallback);
            }
        }

        private void PlayerIsDeadMessageBoxCallback()
        {
            EndWithBot();
            gameRoot.Stop();
        }

        #region MonoBehaviourCallbacks

        private void Update()
        {
            if (!_isInGame) return;

            _bot.Update();
            
            _delayToUpdateLetters -= Time.deltaTime;
            if (_delayToUpdateLetters <= 0f)
            {
                UpdateAnagramAndWords();
                gameRoot.UpdateLetters(anagramsController.CurrentLetters);
            }
        }

        #endregion
    }
}