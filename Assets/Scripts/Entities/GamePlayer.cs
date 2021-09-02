using UnityEngine;

namespace Entities
{
    public class GamePlayer
    {
        public string Name { get; private set; }
        public int Health { get; private set; }

        public bool IsDead => Health == 0;
        
        public GamePlayer(string name)
        {
            Name = name;
            Health = 100;
        }

        public void ChangeHealth(int delta)
        {
            Health = Mathf.Clamp(Health - delta, 0, 100);
        }
    }
}