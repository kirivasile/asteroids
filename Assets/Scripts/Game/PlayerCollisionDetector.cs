using System;
using Asteroids.Configs;
using Asteroids.Views;
using UnityEngine;

namespace Asteroids.Game {
    public class PlayerCollisionDetector {
        readonly IMovablePlayer _player;
        readonly IPlayerSpeedSubscription _playerSpeedSubscription;
        readonly IGameEventEmitter _gameEventDispatcher;
        readonly LayerMask _collisionMask;

        public PlayerCollisionDetector(IMovablePlayer player, IPlayerSpeedSubscription playerSpeedSubscription, IGameEventEmitter gameEventDispatcher, GameConfigSO gameConfig) {
            _player = player;
            _playerSpeedSubscription = playerSpeedSubscription;
            _collisionMask = 1 << gameConfig.AsteroidPrefab.gameObject.layer | 1 << gameConfig.EnemyPrefab.gameObject.layer;
            _gameEventDispatcher = gameEventDispatcher;
        }

        public void OnUpdate(float deltaTime) {
            var hit = Physics2D.CircleCast(
                origin: _player.Position, _player.CollisionRadius, 
                direction: _player.ForwardVector.value, 
                distance: _playerSpeedSubscription.CurrentSpeed * deltaTime, 
                layerMask: _collisionMask
            );
            if (hit && (hit.collider.CompareTag(AsteroidView.TAG) || hit.collider.CompareTag(EnemyView.TAG))) {
                _gameEventDispatcher.Push(SimpleGameEvent.GameFinished);
            }
        }
    }
}