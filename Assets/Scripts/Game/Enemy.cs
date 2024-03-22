using UnityEngine;
using Asteroids.Views;

namespace Asteroids.Game {
    // Class containg the logic of the enemies of the player.
    public class Enemy {
        public readonly EnemyView view;
        
        readonly float _speed;

        public Enemy(EnemyView view, IGameEventEmitter eventDispatcher, float speed) {
            this.view = view;
            _speed = speed;

            view.onCollisionWithPlayerWeapon += () => {
                eventDispatcher.PushEnemyDestroyed(this);
                eventDispatcher.PushPlayerScored(ScoreType.Enemy);
            };
        }

        public void Move(Vector3 playerPosition, float deltaTime) {
            // Don't need to screen bounds check here.
            view.transform.position = Vector3.MoveTowards(view.transform.position, playerPosition, _speed * deltaTime);
        }
    }
}