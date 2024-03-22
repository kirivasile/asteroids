using System;
using Asteroids.Configs;
using Asteroids.Utils;
using UnityEngine;

namespace Asteroids.Game {
    // Class containing the logic for the game.
    public class GameController {
        readonly ScreenBoundsChecker _screenBoundsChecker;
        readonly ScoreCounter _scoreCounter;
        readonly InGameControllers _inGameControllers;

        event Action<float> _gameUpdate;
        
        public IScoreCounter ScoreCounter => _scoreCounter;

        public IUIPlayerDataModel PlayerDataModel => _inGameControllers.PlayerDataModel;

        public GameController(GameConfigSO gameConfig, Camera mainCamera, GameEventDispatcher eventDispatcher) {
            _screenBoundsChecker = new ScreenBoundsChecker(
                mainCamera, 
                screenBoundsThreshold: gameConfig.ScreenBoundsConfig.ScreenBoundsThreshold, 
                teleportThresholdForScreenBounds: gameConfig.ScreenBoundsConfig.TeleportPositionForScreenBounds
            );
            _scoreCounter = new ScoreCounter(eventDispatcher, gameConfig.ScoreConfig);

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

            eventDispatcher.Subscribe(SimpleGameEvent.GameStarted, StartGame);
            eventDispatcher.Subscribe(SimpleGameEvent.GameFinished, FinishGame);
        }

        public void OnUpdate(float deltaTime) => _gameUpdate?.Invoke(deltaTime);

        void StartGame() {
            _gameUpdate += OnGameUpdate;

            _scoreCounter.Reset();
            _inGameControllers.Enable();
        }

        void FinishGame() {
            _gameUpdate -= OnGameUpdate;

            _inGameControllers.Disable();
        }

        void OnGameUpdate(float deltaTime) => _inGameControllers.OnUpdate(deltaTime);

        // Struct that collects all controllers that run during the game.
        struct InGameControllers {
            readonly PlayerController _playerController;
            readonly AsteroidController _asteroidController;
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

            public void Enable() {
                _playerController.Enable();
                _asteroidController.Enable();

                foreach (var controller in _enemyControllers) controller.Enable();
            }

            public void Disable() {
                _playerController.Disable();
                _asteroidController.Disable();
                foreach (var controller in _enemyControllers) controller.Disable();
            }
        }
    }
}
