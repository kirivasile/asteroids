using System;
using System.Collections;
using System.Collections.Generic;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;
using UnityEngine;

namespace Asteroids.Game {
    public class Enemy {
        public readonly EnemyView view;
        readonly float _speed;

        public Enemy(EnemyView view, IGameEventEmitter eventDispatcher, float speed, GameConfigSO gameConfig) {
            this.view = view;
            _speed = speed;

            view.onCollisionWithPlayerWeapon += () => {
                eventDispatcher.PushEnemyDestroyed(this);
                eventDispatcher.PushPlayerScored(gameConfig.EnemyScore);
            };
        }

        public void Move(Vector3 playerPosition, float deltaTime) {
            // Don't need to screen bounds check here.
            view.transform.position = Vector3.MoveTowards(view.transform.position, playerPosition, _speed * deltaTime);
        }
    }
}