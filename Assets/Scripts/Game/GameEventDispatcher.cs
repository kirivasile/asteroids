using System;
using System.Collections.Generic;
using Asteroids.Views;

namespace Asteroids.Game {
    // Game events without any data.
    public enum SimpleGameEvent { GameStarted = 0, GameFinished = 1 }

    public enum ScoreType { Asteroid = 0, MiniAsteroid = 1, Enemy = 2}

    // Interface providing the methods only to subscribe for the events. Can't push them.
    public interface IGameEventSubscriber {
        void Subscribe(SimpleGameEvent evt, Action callback);
        void UnSubscribe(SimpleGameEvent evt, Action callback);

        event Action<Asteroid, AsteroidView.PlayerWeaponType> AsteroidDestroyed;
        event Action<AsteroidMini> MiniAsteroidDestroyed;
        event Action<Enemy> EnemyDestroyed;
        event Action<ScoreType> PlayerScored;
    }

    // Iterface for pushing the events, can't subscribe on them.
    public interface IGameEventEmitter {
        void Push(SimpleGameEvent evt);

        void PushAsteroidDestroyed(Asteroid asteroid, AsteroidView.PlayerWeaponType weaponType);
        void PushAsteroidMiniDestroyed(AsteroidMini asteroid);
        void PushEnemyDestroyed(Enemy enemy);
        void PushPlayerScored(ScoreType type);
    }

    // Simple event dispatcher that hepls to emit and subscribe for the game events.
    public class GameEventDispatcher : IGameEventSubscriber, IGameEventEmitter {

        readonly Dictionary<SimpleGameEvent, Action> _simpleEvents;

        event Action<Asteroid, AsteroidView.PlayerWeaponType> _asteroidDestroyed;
        event Action<AsteroidMini> _miniAsteroidDestroyed;
        event Action<Enemy> _enemyDestroyed;

        event Action<ScoreType> _playerScored;

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

        public event Action<ScoreType> PlayerScored {
            add => _playerScored += value;
            remove => _playerScored -= value;
        }

        public void Push(SimpleGameEvent evt) => _simpleEvents[evt]?.Invoke();

        public void PushAsteroidDestroyed(Asteroid asteroid, AsteroidView.PlayerWeaponType weaponType) =>
            _asteroidDestroyed?.Invoke(asteroid, weaponType);

        public void PushAsteroidMiniDestroyed(AsteroidMini asteroid) => _miniAsteroidDestroyed?.Invoke(asteroid);

        public void PushEnemyDestroyed(Enemy enemy) => _enemyDestroyed?.Invoke(enemy);

        public void PushPlayerScored(ScoreType type) => _playerScored?.Invoke(type);

        public void Subscribe(SimpleGameEvent evt, Action callback) =>
            _simpleEvents[evt] += callback;

        public void UnSubscribe(SimpleGameEvent evt, Action callback) =>
            _simpleEvents[evt] -= callback;
    }
}