using UnityEngine;

using Asteroids.Configs;
using Asteroids.Utils;
using System;

namespace Asteroids.Game {
    public interface IPlayerPositionSubscription {
        Vector3 PlayerPosition { get; }
    }

    public interface IUIPlayerDataModel {
        // Simple analogue to the reactive values.
        event Action<IPlayerUIData> Observe;
    }

    // Controller responsible for all the systems connected to the player.
    public class PlayerController : IPlayerPositionSubscription, IUIPlayerDataModel {
        readonly Player _player;

        readonly PlayerMovementController _playerMovement;
        // List of projectile controllers to make it possible to quickly introduce new weapons for the player
        readonly ProjectileShootingController[] _projectileShootingControllers;
        readonly LaserShootingController _laserShooting;
        readonly PlayerCollisionDetector _collisionDetector;

        Vector3 IPlayerPositionSubscription.PlayerPosition => _player.Position;

        event Action<IPlayerUIData> _playerDataModelObserver;

        event Action<IPlayerUIData> IUIPlayerDataModel.Observe {
            add => _playerDataModelObserver += value;
            remove => _playerDataModelObserver -= value;
        }

        public PlayerController(
            IPlayerMovementConfig playerMovementConfig, IShootingConfig[] shootingConfigs, ILaserConfig laserConfig, 
            IGameEventEmitter gameEventDispatcher, ScreenBoundsChecker screenBoundsChecker, LayerMask collisionLayerMask
        ) {
            var view = UnityEngine.Object.Instantiate(
                playerMovementConfig.PlayerPrefab, playerMovementConfig.PlayerStartPosition, Quaternion.identity
            );
            view.gameObject.SetActive(false);

            var player = new Player(view, playerMovementConfig: playerMovementConfig, laserConfig: laserConfig, screenBoundsChecker);
            _player = player;

            _playerMovement = new PlayerMovementController(player, playerMovementConfig);
            _projectileShootingControllers = shootingConfigs.Map(config => new ProjectileShootingController(player, config, collisionLayerMask));
            _laserShooting = new LaserShootingController(player, laserConfig, collisionLayerMask);
            _collisionDetector = new PlayerCollisionDetector(player, _playerMovement, gameEventDispatcher, collisionLayerMask);
        }

        public void OnUpdate(float deltaTime) {
            _playerMovement.OnUpdate(deltaTime);
            _laserShooting.OnUpdate();
            _collisionDetector.OnUpdate(deltaTime);

            foreach (var controller in _projectileShootingControllers) controller.OnUpdate(deltaTime);

            NotifyPlayerDataModelObservers();

            void NotifyPlayerDataModelObservers() {
                var currentData = new PlayerUIData(
                    playerPosition: _player.Position,
                    playerRotation: _player.Rotation,
                    playerSpeed: _playerMovement.CurrentSpeed,
                    laserCount: _laserShooting.LaserCount,
                    laserRechargeTimer: _laserShooting.LaserRechargeTimer
                );

                _playerDataModelObserver?.Invoke(currentData);
            }
        }

        public void Enable() {
            _player.SetActive(true);
            _player.ResetPosition();
            _playerMovement.Enable();
            _laserShooting.Enable();

            foreach (var controller in _projectileShootingControllers) controller.Enable();
        }

        public void Disable() {
            _player.SetActive(false);
            _playerMovement.Disable();
            _laserShooting.Disable();

            foreach (var controller in _projectileShootingControllers) controller.Disable();
        }

        
    }
}