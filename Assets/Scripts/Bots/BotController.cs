using System;
using Core.Extensions;
using Networking;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bots
{
    public class BotController
    {
        private GameWithBotController _gameWithBotController;
        private AnagramsController _anagramsController;
        private float _delayBetween;
        private float _timeLeft;

        public BotController(
            GameWithBotController gameWithBotController,
            AnagramsController anagramsController,
            float delayBetween)
        {
            _gameWithBotController = gameWithBotController;
            _anagramsController = anagramsController;
            _delayBetween = delayBetween;
        }

        public void Update()
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft > 0f) return;

            _timeLeft += _delayBetween;

            string wordToAdd = _anagramsController.AnagramWords.GetRandom();
            bool useWord = Math.Pow(1f / wordToAdd.Length, 2) > Random.value;
            if (!useWord) return;

            _gameWithBotController.TryWord(wordToAdd, GameWithBotController.BotName);
        }
    }
}