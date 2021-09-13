using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entities;
using Gaming;
using Helpers;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Networking
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        private const byte MaxPlayersCountInRoom = 2;

        [SerializeField] private GameController gameController = null;
        [SerializeField] private MessageBox messageBox = null;
        [SerializeField] private WaitingScreen waitingScreen = null;
        [SerializeField] private LobbyMenu lobbyMenu = null;
        [SerializeField] private GameRoot gameRoot = null;
        [SerializeField] private AnagramsController anagramsController = null;

        private List<UsedWord> _usedWords = new List<UsedWord>();
        private string _currentLetters = null;
        private float _delayToUpdateLetters = -1f;
        private DamageController _damageController = null;

        public bool GameStarted { get; private set; }

        public void SendWord(string word)
        {
            photonView.RPC(
                nameof(RpsTryWord),
                RpcTarget.MasterClient,
                word, PhotonNetwork.NickName);
        }

        public void TryStartGame()
        {
            StartGame();
        }

        public void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = MaxPlayersCountInRoom,
                IsVisible = false,
            };

            string roomName = NameGenerator.Get();
            if (PhotonNetwork.CreateRoom(roomName, roomOptions))
            {
                waitingScreen.Show(HandleCancelCreateRoom, roomName);
            }
        }

        public void FindRandomRoom()
        {
            if (PhotonNetwork.JoinRandomRoom())
            {
                waitingScreen.Show(HandleCancelCreateRoom, showPlayWithBotButton: true);
            }
        }

        public void FindRoomByName(string roomName)
        {
            if (PhotonNetwork.JoinRoom(roomName))
            {
                waitingScreen.Show(HandleCancelCreateRoom);
            }
        }

        public void HandlePlayerIsDead(string playerName)
        {
            if (PhotonNetwork.NickName == playerName)
            {
                messageBox.Show(
                    "UI_You_Won",
                    "UI_Great",
                    gameRoot.Stop);
            }
            else
            {
                messageBox.Show(
                    "UI_You_Lose",
                    "UI_Ok",
                    gameRoot.Stop);
            }
        }

        private void HandleCancelCreateRoom()
        {
            PhotonNetwork.LeaveRoom();
            lobbyMenu.ShowMainMenu();
        }

        #region MonoBehaviourCallbacks

        private void Awake()
        {
            PhotonNetwork.NickName = NameGenerator.Get();
            PhotonNetwork.ConnectUsingSettings();
        }

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient && GameStarted)
            {
                _delayToUpdateLetters -= Time.deltaTime;
                if (_delayToUpdateLetters <= 0f)
                {
                    _delayToUpdateLetters = float.MaxValue;
                    photonView.RPC(
                        nameof(UpdateLetters),
                        RpcTarget.All,
                        GenerateLetters());
                }
            }
        }

        #endregion

        #region PhotonCommands

        [PunRPC]
        private void RpcStartGame(string firstLetters)
        {
            Debug.Log("We started a game!!!");

            List<Player> players = PhotonNetwork.CurrentRoom.Players.Values.ToList();
            string firstPlayerName = players[0].NickName;
            string secondPlayerName = players[1].NickName;
            _damageController = new DamageController(firstPlayerName, secondPlayerName);
            _damageController.OnPlayerIsDead += HandlePlayerIsDead;

            gameController.GameType = GameType.Network;
            
            _currentLetters = firstLetters;
            UpdateAnagramAndWords();

            waitingScreen.Hide();
            lobbyMenu.HideAll();
            gameRoot.Launch(_currentLetters, _damageController);

            GameStarted = true;
        }

        [PunRPC]
        private void RpsTryWord(string word, string userName)
        {
            if (PhotonNetwork.IsMasterClient &&
                anagramsController.AnagramWords.Contains(word) &&
                !_usedWords.Select(x => x.word).Contains(word))
            {
                photonView.RPC(
                    nameof(AddUsedWord),
                    RpcTarget.All,
                    word, userName);
            }
        }

        [PunRPC]
        private void AddUsedWord(string word, string userName)
        {
            UsedWord newUsedWord = new UsedWord(word, PhotonNetwork.NickName == userName);
            _usedWords.Add(newUsedWord);
            gameRoot.UpdateWordsScreen(_usedWords);

            _damageController.ChangePlayerHealth(userName, word.Length, true);
        }

        [PunRPC]
        private void UpdateLetters(string letters)
        {
            _currentLetters = letters;
            UpdateAnagramAndWords();
            gameRoot.UpdateLetters(_currentLetters);
        }

        #endregion

        #region PhotonCallbacks

        public override void OnLeftRoom()
        {
            GameStarted = false;
        }

        #endregion

        private void StartGame()
        {
            _usedWords.Clear();

            if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersCountInRoom && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;

                Debug.Log("I'm the master now and we gonna start the game");
                
                string generatedLetters = GenerateLetters();
                
                photonView.RPC(
                    nameof(RpcStartGame),
                    RpcTarget.All,
                    generatedLetters);
            }
        }

        private string GenerateLetters()
        {
            string generatedLetters = null;
            do
            {
                generatedLetters =
                    LettersGenerator.GetUniqueRandomLetters(GameController.LettersToGenerateForAnagrams);
            } while (generatedLetters == anagramsController.CurrentLetters);

            return generatedLetters;
        }

        private void UpdateAnagramAndWords()
        {
            anagramsController.UpdateAnagramWords(_currentLetters);
            _delayToUpdateLetters = Random.Range(7f, 13f);
        }
    }
}