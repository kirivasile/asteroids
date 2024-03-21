using System.Collections.Generic;
using UnityEngine;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;

namespace Asteroids.Game {
    // TODO KV: Make more abstract to use different enemies
    public class EnemyController {
        readonly PooledObjectsOnTimer<EnemyView, Enemy, Unit> _enemyPool;
        readonly IPlayerPositionSubscription _playerPosition;
        readonly IGameEventSubscriber _gameEventDispatcher;

        public EnemyController(
            GameConfigSO gameConfig, GameEventDispatcher eventDispatcher, ScreenBoundsChecker screenBoundsChecker, IPlayerPositionSubscription playerPosition
        ) {
            _enemyPool = new (
                poolInitialSize: gameConfig.PoolInitialSize,
                spawnPeriod: gameConfig.EnemySpawnPeriod,
                createView: () => Object.Instantiate(gameConfig.EnemyPrefab),
                createInit: createInit,
                getView: init => init.view,
                getPosition: _ => screenBoundsChecker.RandomPositionInsideBounds,
                data: Unit._
            );
            _playerPosition = playerPosition;
            _gameEventDispatcher = eventDispatcher;

            Enemy createInit(EnemyView view, Unit _) {
                return new (view, eventDispatcher, gameConfig.EnemySpeed, gameConfig);
            }
        }

        public void OnUpdate(float deltaTime) {
            _enemyPool.OnUpdate();

            foreach (var enemy in _enemyPool.ActiveObjects) {
                enemy.Move(_playerPosition.PlayerPosition, deltaTime);
            }
        }

        public void Enable() {
            _gameEventDispatcher.EnemyDestroyed += DisableEnemy;

            _enemyPool.Enable();
        }

        public void Disable() {
            _gameEventDispatcher.EnemyDestroyed -= DisableEnemy;

            _enemyPool.Disable();
        }

        void DisableEnemy(Enemy enemy) {
            _enemyPool.Release(enemy);
        }
    }
}