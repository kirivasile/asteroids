using System;
using Asteroids.Configs;
using Asteroids.Views;

namespace Asteroids.Game {
    public interface IScoreCounter {
        int Score { get; }
    }

    public class ScoreCounter : IScoreCounter {
        readonly GameEventDispatcher _gameEventDispatcher;

        int _score;

        int IScoreCounter.Score => _score;

        public ScoreCounter(GameEventDispatcher gameEventDispatcher) {
            _gameEventDispatcher = gameEventDispatcher;

            gameEventDispatcher.PlayerScored += OnPlayerScored;

            Reset();
        }

        public void Reset() {
            _score = 0;
            _gameEventDispatcher.PushScoreChanged(0);
        }

        void OnPlayerScored(int score) {
            _score += score;
            _gameEventDispatcher.PushScoreChanged(_score);
        } 
    }
}