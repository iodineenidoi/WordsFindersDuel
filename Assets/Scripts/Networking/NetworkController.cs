using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entities;
using Gaming;
using Helpers;
using Localization;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace Networking
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        private const byte MaxPlayersCountInRoom = 2;
        private const string LanguagePropKey = "C0";

        [SerializeField] private GameController gameController = null;
        [SerializeField] private MessageBox messageBox = null;
        [SerializeField] private WaitingScreen waitingScreen = null;
        [SerializeField] private LobbyMenu lobbyMenu = null;
        [SerializeField] private GameRoot gameRoot = null;
        [SerializeField] private AnagramsController anagramsController = null;
        [SerializeField] private LettersGenerator lettersGenerator = null;
        [SerializeField] private Localizer localizer = null;

        private List<UsedWord> _usedWords = new List<UsedWord>();
        private string _currentLetters = null;
        private float _delayToUpdateLetters = -1f;
        private DamageController _damageController = null;
        
        private TypedLobby _typedLobby = new TypedLobby("customSqlLobby", LobbyType.SqlLobby);

        private string MyRoomName => $"{Localizer.LanguageCode}:{PhotonNetwork.NickName}";

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

        public void FindRandomRoom()
        {
            string sqlFilter = $"{LanguagePropKey} = '{Localizer.LanguageCode}'";
            bool joinResult = PhotonNetwork
                .JoinRandomRoom(null, MaxPlayersCountInRoom, MatchmakingMode.FillRoom, _typedLobby, sqlFilter);
            
            if (joinResult)
            {
                waitingScreen.Show(HandleCancelCreateRoom, showPlayWithBotButton: true);
            }
        }

        public void CreateRandomRoom()
        {
            PhotonNetwork.CreateRoom(MyRoomName, GetRoomOptions(), _typedLobby);
        }

        public void CreateRoom()
        {
            RoomOptions roomOptions = GetRoomOptions();
            roomOptions.IsVisible = false;

            if (PhotonNetwork.CreateRoom(MyRoomName, roomOptions))
            {
                waitingScreen.Show(HandleCancelCreateRoom, MyRoomName);
            }
        }

        public void FindRoomByName(string roomName)
        {
            if (!roomName.StartsWith(Localizer.LanguageCode))
            {
                messageBox.Show(
                    "UI_Need_Change_Language",
                    "UI_Yes",
                    "UI_No",
                    () =>
                    {
                        LocalizationLanguage languageFromCode = Localizer.GetLanguageFromCode(roomName.Substring(0, 2));
                        localizer.SetLanguage(languageFromCode);
                        FindRoomByName(roomName);
                    },
                    () =>
                    {
                        lobbyMenu.ShowFindLobbyMenu();
                    });

                return;
            }
                
            if (PhotonNetwork.JoinRoom(roomName))
            {
                waitingScreen.Show(HandleCancelCreateRoom);
            }
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
            
            if (PhotonNetwork.NickName == playerName)
            {
                messageBox.Show(
                    "UI_You_Lose",
                    "UI_Ok",
                    gameRoot.Stop);
            }
            else
            {
                messageBox.Show(
                    "UI_You_Won",
                    "UI_Great",
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
            PhotonNetwork.NickName = NameGenerator.Get(4, 7);
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
                        lettersGenerator.UpdateRandomLetters(_currentLetters));
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
                
                string generatedLetters = lettersGenerator
                    .GetUniqueRandomLetters(GameController.LettersToGenerateForAnagrams);
                
                photonView.RPC(
                    nameof(RpcStartGame),
                    RpcTarget.All,
                    generatedLetters);
            }
        }

        private void UpdateAnagramAndWords()
        {
            anagramsController.UpdateAnagramWords(_currentLetters);
            _delayToUpdateLetters = Random.Range(7f, 13f);
        }
        
        private RoomOptions GetRoomOptions()
        {
            RoomOptions result = new RoomOptions();
            result.CustomRoomProperties = new Hashtable {{LanguagePropKey, Localizer.LanguageCode}};
            result.CustomRoomPropertiesForLobby = new[] {LanguagePropKey};
            result.MaxPlayers = MaxPlayersCountInRoom;
            result.IsVisible = true;
            return result;
        }
    }
}