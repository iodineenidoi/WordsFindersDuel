using System.Linq;
using Entities;
using UnityEngine;

namespace Gaming
{
    public class PlayersAnimationsController : MonoBehaviour
    {
        [SerializeField] private PlayerAnimatorSettings[] playersAnimatorSettings = { };
        [SerializeField] private Animator[] playerAnimators = { };

        public void SetTrigger(PlayerAnimationType animationType, bool isMe)
        {
            int animatorIndex = isMe ? 0 : 1;
            playerAnimators[animatorIndex].SetTrigger(animationType.ToString());
        }
        
        public void StartPlayersAnimators()
        {
            PlayerAnimatorSettings[] settings = playersAnimatorSettings.OrderBy(_ => Random.value).ToArray();
            for (int i = 0; i < playerAnimators.Length; i++)
            {
                playerAnimators[i].runtimeAnimatorController = settings[i].animator;
                playerAnimators[i].GetComponentInChildren<SpriteRenderer>().sprite = settings[i].sprite;
                playerAnimators[i].StartPlayback();
            }
        }
    }
}