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
        [SerializeField] private Color myHealthBarColor = Color.black;
        [SerializeField] private Color enemyHealthBarColor = Color.black;
        [SerializeField] private Image firstPlayerHealthValueImage = null;
        [SerializeField] private Image secondPlayerHealthValueImage = null;
        [SerializeField] private TMP_Text firstPlayerHealthValueText = null;
        [SerializeField] private TMP_Text secondPlayerHealthValueText = null;

        public void ResetPlayers(DamageController damageController)
        {
            damageController.OnPlayersHealthChanged += UpdatePlayersHealth;

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

        private void UpdatePlayersHealth(int firstPlayerHealth, int secondPlayerHealth)
        {
            firstPlayerHealthValueImage.fillAmount = firstPlayerHealth / 100f;
            secondPlayerHealthValueImage.fillAmount = secondPlayerHealth / 100f;

            firstPlayerHealthValueText.text = firstPlayerHealth.ToString();
            secondPlayerHealthValueText.text = secondPlayerHealth.ToString();
        }
    }
}