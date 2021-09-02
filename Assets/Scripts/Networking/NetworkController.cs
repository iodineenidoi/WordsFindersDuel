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
        private const int LettersToGenerateForAnagrams = 15;

        [SerializeField] private MessageBox messageBox = null;
        [SerializeField] private WaitingScreen waitingScreen = null;
        [SerializeField] private LobbyMenu lobbyMenu = null;
        [SerializeField] private GameRoot gameRoot = null;
        [SerializeField] private AnagramsLoader anagramsLoader = null;

        private List<UsedWord> _usedWords = new List<UsedWord>();
        private string _currentLetters = null;
        private float _delayToUpdateLetters = -1f;
        private DamageController _damageController = null;

        public DamageController DamageController => _damageController;
        public bool GameStarted { get; set; }

        public void SendWord(string word)
        {
            photonView.RPC(
                nameof(RpsTryWord), 
                RpcTarget.MasterClient,
                word, PhotonNetwork.NickName);
        }

        public void TryStartGame()
        {
            StartCoroutine(StartGame());
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
                waitingScreen.Show(HandleCancelCreateRoom);
            }
        }

        public void FindRoomByName(string roomName)
        {
            if (PhotonNetwork.JoinRoom(roomName))
            {
                waitingScreen.Show(HandleCancelCreateRoom);
            }
        }

        private void HandleCancelCreateRoom()
        {
            PhotonNetwork.LeaveRoom();
            lobbyMenu.ShowMainMenu();
        }

        public void HandlePlayerIsDead(string playerName)
        {
            if (PhotonNetwork.NickName == playerName)
            {
                messageBox.Show(
                    "Противник повержен!\nПоздравляю, вы победили!",
                    "Ура!",
                    gameRoot.Stop);
            }
            else
            {
                messageBox.Show(
                    "Вы проиграли.",
                    "Ок",
                    gameRoot.Stop);
            }
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
                    StartCoroutine(LoadNewWords(() =>
                    {
                        photonView.RPC(
                            nameof(UpdateLetters),
                            RpcTarget.All,
                            _currentLetters);
                    }));
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
            
            waitingScreen.Hide();
            lobbyMenu.HideAll();
            gameRoot.Launch(firstLetters);
            _currentLetters = firstLetters;
            GameStarted = true;
        }

        [PunRPC]
        private void RpsTryWord(string word, string userName)
        {
            if (PhotonNetwork.IsMasterClient && 
                anagramsLoader.LoadedWords.Contains(word) &&
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
            gameRoot.UpdateLetters(_currentLetters);
        }
        
        #endregion
        
        #region PhotonCallbacks

        public override void OnLeftRoom()
        {
            GameStarted = false;
        }

        #endregion

        private IEnumerator StartGame()
        {
            _usedWords.Clear();

            if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersCountInRoom && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                
                Debug.Log("I'm the master now and we gonna start the game");
                yield return StartCoroutine(LoadNewWords());

                photonView.RPC(
                    nameof(RpcStartGame),
                    RpcTarget.All,
                    _currentLetters);
            }
        }
        
        private IEnumerator LoadNewWords(Action callback = null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                string generatedLetters = LettersGenerator.GetUniqueRandomLetters(LettersToGenerateForAnagrams);
                while (generatedLetters == anagramsLoader.Letters)
                {
                    generatedLetters = LettersGenerator.GetUniqueRandomLetters(LettersToGenerateForAnagrams);
                }
                
                anagramsLoader.LoadAnagrams(generatedLetters);
                yield return new WaitUntil(() => anagramsLoader.IsReady);
                
                _currentLetters = generatedLetters;
                _delayToUpdateLetters = Random.Range(7f, 13f);
                
                callback?.Invoke();
            }
        }
    }
}