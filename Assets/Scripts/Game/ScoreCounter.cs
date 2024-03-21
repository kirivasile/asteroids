using System;
using Asteroids.Configs;

namespace Asteroids.Game {
    public interface IScoreCounter {
        int Score { get; }
    }

    public class ScoreCounter : IScoreCounter {
        readonly IScoreConfig _scoreConfig;

        int _score;

        int IScoreCounter.Score => _score;

        public ScoreCounter(IGameEventSubscriber gameEventDispatcher, IScoreConfig scoreConfig) {
            _scoreConfig = scoreConfig;

            gameEventDispatcher.PlayerScored += OnPlayerScored;

            Reset();
        }

        public void Reset() {
            _score = 0;
        }

        void OnPlayerScored(ScoreType type) {
            var score = type switch {
                ScoreType.Asteroid => _scoreConfig.AsteroidScore,
                ScoreType.MiniAsteroid => _scoreConfig.MiniAsteroidScore,
                ScoreType.Enemy => _scoreConfig.EnemyScore,
                // Better handle this with `ExhaustiveMatching` module, however I can't use it.
                _ => throw new ArgumentOutOfRangeException()
            };
            _score += score;
        } 
    }
}