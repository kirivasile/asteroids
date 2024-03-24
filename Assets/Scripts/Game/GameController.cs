using System;
using Asteroids.Configs;
using Asteroids.Utils;
using UnityEngine;

namespace Asteroids.Game {
    // Class containing the logic for the game.
    public class GameController {
        // See class definition for the info.
        readonly ScreenBoundsChecker _screenBoundsChecker;
        // Class responsible for counting the score.
        readonly ScoreCounter _scoreCounter;
        // See struct definition for info.
        readonly InGameControllers _inGameControllers;
        // Controller that disposes all of the disposables after the game session ends.
        // It prevents from forgetting unsubscribing on some event or not releasing the pool, and etc.
        readonly IDisposableController _gameDisposableController;

        // Main update loop of the game.
        event Action<float> _gameUpdate;

        public IUIPlayerDataModel PlayerDataModel => _inGameControllers.PlayerDataModel;

        public GameController(
            GameConfigSO gameConfig, Camera mainCamera, GameEventDispatcher eventDispatcher, IDisposableTracker noDisposableTracker,
            ScoreCounter scoreCounter
        ) {
            _gameDisposableController = new DisposableController();
            _scoreCounter = scoreCounter;

            _screenBoundsChecker = new ScreenBoundsChecker(
                mainCamera, 
                screenBoundsThreshold: gameConfig.ScreenBoundsConfig.ScreenBoundsThreshold, 
                teleportThresholdForScreenBounds: gameConfig.ScreenBoundsConfig.TeleportPositionForScreenBounds
            );

            // Collision mask for the player. It and its weapons can only interact with the asteroids or enemies.
            var playerCollisionMask = 
                1 << gameConfig.AsteroidConfig.AsteroidPrefab.gameObject.layer 
                | 1 << gameConfig.EnemyConfig.EnemyPrefab.gameObject.layer;

            var shootingConfigs = new[] { gameConfig.ShootingConfig };

            var playerController = new PlayerController(
                gameConfig.PlayerMovementConfig, shootingConfigs, gameConfig.LaserConfig, 
                eventDispatcher, _screenBoundsChecker, playerCollisionMask
            );

            var enemyControllers = new [] {
                new EnemyController(gameConfig.EnemyConfig, eventDispatcher, _screenBoundsChecker, playerController)
            };

            _inGameControllers = new InGameControllers(
                playerController,
                new AsteroidController(gameConfig.AsteroidConfig, eventDispatcher, _screenBoundsChecker),
                enemyControllers
            );

            eventDispatcher.Subscribe(noDisposableTracker, SimpleGameEvent.GameStarted, StartGame);
            eventDispatcher.Subscribe(noDisposableTracker, SimpleGameEvent.GameFinished, FinishGame);
        }

        public void OnUpdate(float deltaTime) => _gameUpdate?.Invoke(deltaTime);

        void StartGame() {
            _gameUpdate += OnGameUpdate;
            _gameDisposableController.Track(() => _gameUpdate -= OnGameUpdate);

            _scoreCounter.Reset();
            _inGameControllers.Enable(_gameDisposableController);
        }

        void FinishGame() {
            _gameDisposableController.Dispose();
        }

        void OnGameUpdate(float deltaTime) => _inGameControllers.OnUpdate(deltaTime);

        // Struct that collects all controllers that run during the game.
        struct InGameControllers {
            // All of the logic assosiated with the player.
            readonly PlayerController _playerController;
            // Logic of the asteroids and their chunks (mini-asteroids)
            readonly AsteroidController _asteroidController;
            // Logic of the player's enemies.
            // List of enemy controllers to make it possible to quickly introduce new enemies.
            readonly EnemyController[] _enemyControllers;

            public IUIPlayerDataModel PlayerDataModel => _playerController;

            public InGameControllers(
                PlayerController playerController, AsteroidController asteroidController, EnemyController[] enemyControllers
            ) {
                _playerController = playerController;
                _asteroidController = asteroidController;
                _enemyControllers = enemyControllers;
            }

            public void OnUpdate(float deltaTime) {
                _playerController.OnUpdate(deltaTime);
                _asteroidController.OnUpdate(deltaTime);

                foreach (var controller in _enemyControllers) controller.OnUpdate(deltaTime);
            }

            public void Enable(IDisposableTracker gameTracker) {
                _playerController.Enable(gameTracker);
                _asteroidController.Enable(gameTracker);

                foreach (var controller in _enemyControllers) controller.Enable(gameTracker);
            }
        }
    }
}
