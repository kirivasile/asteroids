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
        readonly IPlayerConfig _config;
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

        public PlayerController(
            IPlayerConfig config, IGameEventEmitter gameEventDispatcher, ScreenBoundsChecker screenBoundsChecker, LayerMask collisionLayerMask
        ) {
            _config = config;

            var view = UnityEngine.Object.Instantiate(_config.PlayerPrefab, config.PlayerStartPosition, Quaternion.identity);
            view.gameObject.SetActive(false);

            var player = new Player(view, playerMovementConfig: config, laserConfig: config, screenBoundsChecker);
            _player = player;

            _playerMovement = new PlayerMovementController(player, config);
            _mainShooting = new ProjectileShootingController(player, shootingConfig: config, collisionLayerMask);
            _laserShooting = new LaserShootingController(player, config, collisionLayerMask);
            _collisionDetector = new PlayerCollisionDetector(player, _playerMovement, gameEventDispatcher, config, collisionLayerMask);
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