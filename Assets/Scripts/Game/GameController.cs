using System;
using System.Collections;
using System.Collections.Generic;
using Asteroids.Configs;
using Asteroids.Game;
using Asteroids.Utils;
using UnityEngine;

namespace Asteroids.Game {
    public interface IUIPlayerDataModel {
        Option<IPlayerUIData> PlayerData { get; }
    }

    // TODO KV: maybe add logger as well?
    public class GameController : IUIPlayerDataModel {
        readonly ScreenBoundsChecker _screenBoundsChecker;
        readonly ScoreCounter _scoreCounter;
        readonly InGameControllers _inGameControllers;

        event Action<float> _gameUpdate;

        // TODO KV: Don't like mutable flag
        bool _isGameRunning;
        
        public IScoreCounter ScoreCounter => _scoreCounter;

        // Option<IPlayerUIData> IUIPlayerDataModel.PlayerData => _maybeInGameControllers.Map(_ => _.playerController.PlayerData);
        // TODO KV: docme
        // TODO KV: change for BoolExts.ToOption
        Option<IPlayerUIData> IUIPlayerDataModel.PlayerData {
            get {
                if (_isGameRunning) return Some._(_inGameControllers.playerController.PlayerData);
                else return None._;
            }
        }

        public GameController(GameConfigSO gameConfig, Camera mainCamera, GameEventDispatcher eventDispatcher) {
            _screenBoundsChecker = new ScreenBoundsChecker(
                mainCamera, 
                screenBoundsThreshold: gameConfig.ScreenBoundsThreshold, 
                teleportThresholdForScreenBounds: gameConfig.TeleportPositionForScreenBounds
            );
            _scoreCounter = new ScoreCounter(eventDispatcher, gameConfig);

            var playerCollisionMask = 1 << gameConfig.AsteroidPrefab.gameObject.layer | 1 << gameConfig.EnemyPrefab.gameObject.layer;

            var playerController = new PlayerController(gameConfig, eventDispatcher, _screenBoundsChecker, playerCollisionMask);
            _inGameControllers = new InGameControllers(
                playerController,
                new AsteroidController(gameConfig, eventDispatcher, _screenBoundsChecker),
                new EnemyController(gameConfig, eventDispatcher, _screenBoundsChecker, playerController)
            );

            eventDispatcher.Subscribe(SimpleGameEvent.GameStarted, StartGame);
            eventDispatcher.Subscribe(SimpleGameEvent.GameFinished, FinishGame);
        }

        public void OnUpdate(float deltaTime) => _gameUpdate?.Invoke(deltaTime);

        void StartGame() {
            _gameUpdate += _inGameControllers.OnUpdate;

            _isGameRunning = true;

            _scoreCounter.Reset();
            _inGameControllers.Enable();
        }

        void FinishGame() {
            _gameUpdate -= _inGameControllers.OnUpdate;

            _isGameRunning = false;

            _inGameControllers.Disable();
        }

        class InGameControllers {
            public readonly PlayerController playerController;
            readonly AsteroidController _asteroidController;
            readonly EnemyController _enemyController;

            public InGameControllers(
                PlayerController playerController, AsteroidController asteroidController, EnemyController enemyController
            ) {
                this.playerController = playerController;
                _asteroidController = asteroidController;
                _enemyController = enemyController;
            }

            public void OnUpdate(float deltaTime) {
                playerController.OnUpdate(deltaTime);
                _asteroidController.OnUpdate(deltaTime);
                _enemyController.OnUpdate(deltaTime);
            }

            public void Enable() {
                playerController.Enable();
                _asteroidController.Enable();
                _enemyController.Enable();
            }

            public void Disable() {
                playerController.Disable();
                _asteroidController.Disable();
                _enemyController.Disable();
            }
        }
    }
}
