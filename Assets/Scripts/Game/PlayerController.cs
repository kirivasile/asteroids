using UnityEngine;

using Asteroids.Configs;
using Asteroids.Utils;
using System;

namespace Asteroids.Game {
    public interface IPlayerPositionSubscription {
        Vector3 PlayerPosition { get; }
    }

    // Model interface for the player UI.
    public interface IUIPlayerDataModel {
        void SubscribeOnPlayerUIData(IDisposableTracker tracker, Action<IPlayerUIData> handler);
    }

    // Controller responsible for all the systems connected to the player.
    public class PlayerController : IPlayerPositionSubscription, IUIPlayerDataModel {
        // Player object representation
        readonly Player _player;

        // Movement logic
        readonly PlayerMovementController _playerMovement;
        // Projectile shooting logic
        // List of projectile controllers to make it possible to quickly introduce new weapons for the player
        readonly ProjectileShootingController[] _projectileShootingControllers;
        // Laser shooting logic
        readonly LaserShootingController _laserShooting;
        // Player collision detection
        readonly PlayerCollisionDetector _collisionDetector;

        Vector3 IPlayerPositionSubscription.PlayerPosition => _player.Position;

        // Used for implementation of `IUIPlayerDataModel` 
        event Action<IPlayerUIData> _playerDataModelObservable;

        void IUIPlayerDataModel.SubscribeOnPlayerUIData(IDisposableTracker tracker, Action<IPlayerUIData> handler) {
            _playerDataModelObservable += handler;
            tracker.Track(() => _playerDataModelObservable -= handler);
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

                _playerDataModelObservable?.Invoke(currentData);
            }
        }

        public void Enable(IDisposableTracker gameTracker) {
            _player.Enable(gameTracker);
            _player.Reset();
            _playerMovement.Enable(gameTracker);
            _laserShooting.Enable(gameTracker);

            foreach (var controller in _projectileShootingControllers) controller.Enable(gameTracker);
        }

        
    }
}