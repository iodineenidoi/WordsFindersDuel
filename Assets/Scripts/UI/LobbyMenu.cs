using UnityEngine;

namespace UI
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu = null;
        [SerializeField] private GameObject findLobbyMenu = null;

        public void HideAll()
        {
            findLobbyMenu.SetActive(false);
            mainMenu.SetActive(false);
        }
        
        public void ShowMainMenu()
        {
            findLobbyMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        
        public void ShowFindLobbyMenu()
        {
            mainMenu.SetActive(false);
            findLobbyMenu.SetActive(true);
        }
    }
}