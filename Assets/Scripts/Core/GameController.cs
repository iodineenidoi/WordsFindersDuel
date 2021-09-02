using UI;
using UnityEngine;

namespace Core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private LobbyMenu lobbyMenu = null;
        [SerializeField] private WaitingScreen waitingScreen = null;

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #region MonoBehaviourCallbacks

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            lobbyMenu.ShowMainMenu();
        }

        #endregion
    }
}