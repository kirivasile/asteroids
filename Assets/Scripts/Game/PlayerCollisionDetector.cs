using UnityEngine;
using Asteroids.Views;

namespace Asteroids.Game {
    // Class that checks the player's collisions, and sends events if `Physics2D.CircleCast` has found something.
    public class PlayerCollisionDetector {
        readonly IMovablePlayer _player;
        readonly IPlayerSpeedSubscription _playerSpeedSubscription;
        readonly IGameEventEmitter _gameEventDispatcher;
        readonly LayerMask _collisionMask;

        public PlayerCollisionDetector(
            IMovablePlayer player, IPlayerSpeedSubscription playerSpeedSubscription, IGameEventEmitter gameEventDispatcher, LayerMask collisionMask
        ) {
            _player = player;
            _playerSpeedSubscription = playerSpeedSubscription;
            _collisionMask = collisionMask;
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