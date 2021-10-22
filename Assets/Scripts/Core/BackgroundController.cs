using UnityEngine;

namespace Core
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyBackground = null;

        public void SetLobbyBackground()
        {
            lobbyBackground.SetActive(true);
        }

        public void SetBattleBackground()
        {
            lobbyBackground.SetActive(false);
        }
    }
}