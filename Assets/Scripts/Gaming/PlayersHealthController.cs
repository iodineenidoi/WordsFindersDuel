﻿using System.Collections;
using Core;
using Networking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gaming
{
    public class PlayersHealthController : MonoBehaviour
    {
        [SerializeField] private int framesCountBeforeChangeHealth = 0;
        [SerializeField] private Color myHealthBarColor = Color.black;
        [SerializeField] private Color enemyHealthBarColor = Color.black;
        [SerializeField] private Image firstPlayerHealthValueImage = null;
        [SerializeField] private Image secondPlayerHealthValueImage = null;
        [SerializeField] private TMP_Text firstPlayerHealthValueText = null;
        [SerializeField] private TMP_Text secondPlayerHealthValueText = null;

        public void ResetPlayers(DamageController damageController)
        {
            damageController.OnPlayersHealthChanged += UpdatePlayersHealthDelayed;

            if (PhotonNetwork.NickName == damageController.FirstGamePlayer.Name)
            {
                firstPlayerHealthValueImage.color = myHealthBarColor;
                secondPlayerHealthValueImage.color = enemyHealthBarColor;
            }
            else
            {
                firstPlayerHealthValueImage.color = enemyHealthBarColor;
                secondPlayerHealthValueImage.color = myHealthBarColor;
            }

            UpdatePlayersHealth(damageController.FirstGamePlayer.Health, damageController.SecondGamePlayer.Health);
        }

        private void UpdatePlayersHealthDelayed(int firstPlayerHealth, int secondPlayerHealth)
        {
            StartCoroutine(UpdatePlayersHealthDelayed(
                firstPlayerHealth,
                secondPlayerHealth, 
                framesCountBeforeChangeHealth));
        }
        
        private IEnumerator UpdatePlayersHealthDelayed(int firstPlayerHealth, int secondPlayerHealth, int framesDelay)
        {
            while (framesDelay-- > 0)
            {
                yield return null;
            }

            UpdatePlayersHealth(firstPlayerHealth, secondPlayerHealth);
        }

        private void UpdatePlayersHealth(int firstPlayerHealth, int secondPlayerHealth)
        {
            firstPlayerHealthValueImage.fillAmount = firstPlayerHealth / 100f;
            secondPlayerHealthValueImage.fillAmount = secondPlayerHealth / 100f;

            firstPlayerHealthValueText.text = firstPlayerHealth.ToString();
            secondPlayerHealthValueText.text = secondPlayerHealth.ToString();
        }
    }
}