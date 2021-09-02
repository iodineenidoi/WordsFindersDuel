using Core.Extensions;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FindLobbyMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomNameInputField = null;
        [SerializeField] private Button joinRoomByNameButton = null;
        [SerializeField] private NetworkController networkController = null;
        
        public void JoinRoomByName()
        {
            networkController.FindRoomByName(roomNameInputField.text);
        }

        public void ValidateInputField(string value)
        {
            joinRoomByNameButton.interactable = !value.IsNullOrWhiteSpace();
        }
    }
}