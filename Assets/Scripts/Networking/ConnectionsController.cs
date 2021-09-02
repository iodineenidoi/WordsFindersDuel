﻿using Gaming;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;

namespace Networking
{
    [RequireComponent(typeof(PhotonView))]
    public class ConnectionsController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private NetworkController networkController = null;
        [SerializeField] private MessageBox messageBox = null;
        [SerializeField] private WaitingScreen waitingScreen = null;
        [SerializeField] private LobbyMenu lobbyMenu = null;
        [SerializeField] private GameRoot gameRoot = null;
        
        #region PhotonCallbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Return code: {returnCode}. Message: {message}");

            if (returnCode == 32760)
            {
                PhotonNetwork.CreateRoom(null, new RoomOptions());
            }
        }
        
        public override void OnCreatedRoom()
        {
            Debug.Log("I created room");
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined to room. Player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
            networkController.TryStartGame();
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Player joined my room. Player name: {newPlayer.NickName}. Player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
            networkController.TryStartGame();
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
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player {otherPlayer.NickName} left the game. You are the winner!");

            if (networkController.GameStarted)
            {
                HandleEnemyLeftGame();
            }
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"Disconnected due to: {cause}");
        }

        #endregion
        
        private void HandleEnemyLeftGame()
        {
            messageBox.Show(
                "Противник покинул игру!\nВы победили!",
                "Ура!",
                () =>
                {
                    gameRoot.Stop();
                });
        }
    }
}