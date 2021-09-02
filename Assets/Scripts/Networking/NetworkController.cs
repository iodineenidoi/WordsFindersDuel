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
        public bool GameStarted { get; private set; }

        public void SendWord(string word)
        {
            photonView.RPC(
                nameof(RpsTryWord), 
                RpcTarget.MasterClient,
                word, PhotonNetwork.NickName);
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

        private void HandleEnemyLeftGame()
        {
            messageBox.Show(
                "Противник покинул игру!\nВы победили!",
                "Ура!",
                () =>
                {
                    gameRoot.Stop();
                    _damageController = null;
                });
        }

        public void HandlePlayerIsDead(string playerName)
        {
            if (PhotonNetwork.NickName == playerName)
            {
                messageBox.Show(
                    "Противник повержен!\nПоздравляю, вы победили!",
                    "Ура!",
                    () =>
                    {
                        gameRoot.Stop();
                        _damageController = null;
                    });
            }
            else
            {
                messageBox.Show(
                    "Вы проиграли.",
                    "Ок",
                    () =>
                    {
                        gameRoot.Stop();
                        _damageController = null;
                    });
            }
        }
        
        #region MonoBehaviourCallbacks

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PhotonNetwork.NickName = NameGenerator.Get();
            PhotonNetwork.ConnectUsingSettings();
        }

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
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

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined to room. Player count: {PhotonNetwork.CurrentRoom.PlayerCount}");

            StartCoroutine(TryStartGame());
        }

        public override void OnLeftRoom()
        {
            GameStarted = false;
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("I created room");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Player joined my room. Player name: {newPlayer.NickName}. Player count: {PhotonNetwork.CurrentRoom.PlayerCount}");

            StartCoroutine(TryStartGame());
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined to lobby");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Return code: {returnCode}. Message: {message}");

            PhotonNetwork.CreateRoom(null, new RoomOptions());
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Return code: {returnCode}. Message: {message}");
            
            if (returnCode == 32758)
            {
                messageBox.Show(
                    "Комнаты с таким именем не существует",
                    "Продолжить",
                    () =>
                    {
                        waitingScreen.Hide();
                        lobbyMenu.ShowFindLobbyMenu();
                    });
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"Disconnected due to: {cause}");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player {otherPlayer.NickName} left the game. You are the winner!");

            if (GameStarted)
            {
                HandleEnemyLeftGame();
            }
        }

        #endregion

        private IEnumerator TryStartGame()
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