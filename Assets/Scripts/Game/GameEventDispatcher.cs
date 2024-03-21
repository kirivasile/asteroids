using System;
using System.Collections.Generic;
using Asteroids.Views;

namespace Asteroids.Game {
    // TODO KV: docme
    public enum SimpleGameEvent { GameStarted = 0, GameFinished = 1 }

    public interface IGameEventSubscriber {
        void Subscribe(SimpleGameEvent evt, Action callback);
        void UnSubscribe(SimpleGameEvent evt, Action callback);

        event Action<Asteroid, AsteroidView.PlayerWeaponType> AsteroidDestroyed;
        event Action<AsteroidMini> MiniAsteroidDestroyed;
        event Action<Enemy> EnemyDestroyed;
        event Action<int> PlayerScored;
        event Action<int> ScoreChanged;
    }

    public interface IGameEventEmitter {
        void Push(SimpleGameEvent evt);

        void PushAsteroidDestroyed(Asteroid asteroid, AsteroidView.PlayerWeaponType weaponType);
        void PushAsteroidMiniDestroyed(AsteroidMini asteroid);
        void PushEnemyDestroyed(Enemy enemy);
        void PushPlayerScored(int score);
        void PushScoreChanged(int score);

        // void ClearAllSubscriptions();
    }

    // TODO KV: Split to push events and subscriber
    public class GameEventDispatcher : IGameEventSubscriber, IGameEventEmitter {

        readonly Dictionary<SimpleGameEvent, Action> _simpleEvents;

        // Maybe remove these events and return to callbacks?
        event Action<Asteroid, AsteroidView.PlayerWeaponType> _asteroidDestroyed;
        event Action<AsteroidMini> _miniAsteroidDestroyed;
        event Action<Enemy> _enemyDestroyed;

        event Action<int> _playerScored;
        // TODO KV: remove
        event Action<int> _scoreChanged;

        public GameEventDispatcher() {
            _simpleEvents = new();

            foreach (var evt in Enum.GetValues(typeof(SimpleGameEvent))) {
                _simpleEvents.Add((SimpleGameEvent) evt, null);
            }
        }

        public event Action<AsteroidMini> MiniAsteroidDestroyed {
            add => _miniAsteroidDestroyed += value;
            remove => _miniAsteroidDestroyed -= value;
        }

        public event Action<Enemy> EnemyDestroyed {
            add => _enemyDestroyed += value;
            remove => _enemyDestroyed -= value;
        }

        public event Action<Asteroid, AsteroidView.PlayerWeaponType> AsteroidDestroyed {
            add => _asteroidDestroyed += value;
            remove => _asteroidDestroyed -= value;
        }

        public event Action<int> PlayerScored {
            add => _playerScored += value;
            remove => _playerScored -= value;
        }

        public event Action<int> ScoreChanged {
            add => _scoreChanged += value;
            remove => _scoreChanged -= value;
        }

        public void Push(SimpleGameEvent evt) => _simpleEvents[evt]?.Invoke();

        public void PushAsteroidDestroyed(Asteroid asteroid, AsteroidView.PlayerWeaponType weaponType) =>
            _asteroidDestroyed?.Invoke(asteroid, weaponType);

        public void PushAsteroidMiniDestroyed(AsteroidMini asteroid) => _miniAsteroidDestroyed?.Invoke(asteroid);

        public void PushEnemyDestroyed(Enemy enemy) => _enemyDestroyed?.Invoke(enemy);

        public void PushScoreChanged(int score) => _scoreChanged?.Invoke(score);

        public void PushPlayerScored(int score) => _playerScored?.Invoke(score);

        // TODO KV: check usages
        // public void ClearAllSubscriptions() {
        //     _simpleEvents.Clear();
        //     foreach (var evt in Enum.GetValues(typeof(SimpleGameEvent))) {
        //         _simpleEvents.Add((SimpleGameEvent)evt, null);
        //     }

        //     _asteroidDestroyed = null;
        //     _miniAsteroidDestroyed = null;
        //     _enemyDestroyed = null;
        //     _scoreChanged = null;
        //     _playerScored = null;
        // }

        public void Subscribe(SimpleGameEvent evt, Action callback) =>
            _simpleEvents[evt] += callback;

        public void UnSubscribe(SimpleGameEvent evt, Action callback) =>
            _simpleEvents[evt] -= callback;
    }
}