using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class FlyingDamageTextController : MonoBehaviour
    {
        [SerializeField] private FlyingText[] textObjects = { };

        private Queue<FlyingText> _flyingTextPool = new Queue<FlyingText>();

        public void ShowDamage(int damage)
        {
            FlyingText text = _flyingTextPool.Dequeue();
            _flyingTextPool.Enqueue(text);
            text.Launch(damage.ToString());
        }
        
        #region MonoBehaviourCallbacks

        private void OnEnable()
        {
            _flyingTextPool.Clear();
            foreach (FlyingText text in textObjects)
            {
                text.gameObject.SetActive(false);
                _flyingTextPool.Enqueue(text);
            }
        }

        #endregion
    }
}