using UnityEngine;

using Asteroids.Configs;
using Asteroids.Game;
using Asteroids.Utils;
using System;

namespace Asteroids.Game {
    public interface IPlayerPositionSubscription {
        Vector3 PlayerPosition { get; }
    }

    public class PlayerController : IPlayerPositionSubscription {
        readonly GameConfigSO _gameConfig;
        readonly Player _player;

        readonly PlayerMovementController _playerMovement;
        readonly ProjectileShootingController _mainShooting; // Make more abstract to shoot different projectiles
        readonly LaserShootingController _laserShooting;
        readonly PlayerCollisionDetector _collisionDetector;

        Vector3 IPlayerPositionSubscription.PlayerPosition => _player.Position;

        public IPlayerUIData PlayerData => new PlayerUIData(
            playerPosition: _player.Position,
            playerRotation: _player.Rotation,
            playerSpeed: _playerMovement.CurrentSpeed,
            laserCount: _laserShooting.LaserCount,
            laserRechargeTimer: _laserShooting.LaserRechargeTimer
        );

        public PlayerController(GameConfigSO gameConfig, IGameEventEmitter gameEventDispatcher, ScreenBoundsChecker screenBoundsChecker) {
            _gameConfig = gameConfig;

            var view = UnityEngine.Object.Instantiate(_gameConfig.PlayerPrefab, gameConfig.PlayerStartPosition, Quaternion.identity);
            view.gameObject.SetActive(false);

            var player = new Player(view, gameConfig, screenBoundsChecker);
            _player = player;

            _playerMovement = new PlayerMovementController(player, gameConfig);
            _mainShooting = new ProjectileShootingController(_gameConfig, player);
            _laserShooting = new LaserShootingController(_gameConfig, player);
            _collisionDetector = new PlayerCollisionDetector(player, _playerMovement, gameEventDispatcher, _gameConfig);
        }

        public void OnUpdate(float deltaTime) {
            _playerMovement.OnUpdate(deltaTime);
            _mainShooting.OnUpdate(deltaTime);
            _laserShooting.OnUpdate();
            _collisionDetector.OnUpdate(deltaTime);
        }

        public void Enable() {
            _player.SetActive(true);
            _player.ResetPosition();

            _playerMovement.Enable();
            _mainShooting.Enable();
            _laserShooting.Enable();
        }

        // TODO KV: maybe add trackers?
        public void Disable() {
            _player.SetActive(false);
            _playerMovement.Disable();
            _mainShooting.Disable();
            _laserShooting.Disable();
        }
    }
}