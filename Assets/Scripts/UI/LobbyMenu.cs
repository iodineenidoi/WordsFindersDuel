﻿using UnityEngine;

namespace UI
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject gameName = null;
        [SerializeField] private GameObject mainMenu = null;
        [SerializeField] private GameObject findLobbyMenu = null;
        [SerializeField] private GameObject settingsMenu = null;

        public void HideAll()
        {
            findLobbyMenu.SetActive(false);
            mainMenu.SetActive(false);
            settingsMenu.SetActive(false);
            gameName.SetActive(false);
        }
        
        public void ShowMainMenu()
        {
            findLobbyMenu.SetActive(false);
            mainMenu.SetActive(true);
            settingsMenu.SetActive(true);
            gameName.SetActive(true);
        }
        
        public void ShowFindLobbyMenu()
        {
            mainMenu.SetActive(false);
            findLobbyMenu.SetActive(true);
            settingsMenu.SetActive(true);
            gameName.SetActive(true);
        }
    }
}