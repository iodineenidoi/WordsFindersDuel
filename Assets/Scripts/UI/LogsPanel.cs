using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LogsPanel : MonoBehaviour
    {
        [SerializeField] private Scrollbar verticalScroll = null;

        private void OnEnable()
        {
            verticalScroll.value = 0;
        }
    }
}