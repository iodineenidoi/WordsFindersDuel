using System;
using Entities;

namespace Core
{
    public class DamageController
    {
        private GamePlayer _first;
        private GamePlayer _second;

        public event Action<string> OnPlayerIsDead;
        public event Action<int, int> OnPlayersHealthChanged;

        public GamePlayer FirstGamePlayer => _first;
        public GamePlayer SecondGamePlayer => _second;
        
        public DamageController(string firstPlayerName, string secondPlayerName)
        {
            _first = new GamePlayer(firstPlayerName);
            _second = new GamePlayer(secondPlayerName);
        }

        public void ChangePlayerHealth(string name, int delta, bool changeAnotherPlayerHealth = false)
        {
            if (_first.IsDead || _second.IsDead) 
                return;
            
            GamePlayer player;

            if (_first.Name == name) player = changeAnotherPlayerHealth ? _second : _first;
            else if (_second.Name == name) player = changeAnotherPlayerHealth ? _first : _second;
            else throw new ArgumentException($"There's no such a player with name: {name}");

            player.ChangeHealth(delta);
            OnPlayersHealthChanged?.Invoke(_first.Health, _second.Health);
            
            if (player.IsDead)
            {
                OnPlayerIsDead?.Invoke(player.Name);
            }
        }
    }
}