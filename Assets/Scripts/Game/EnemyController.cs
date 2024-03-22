using UnityEngine;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;

namespace Asteroids.Game {
    // Class responsible for the player's enemies logic. One controller for each type of enemy.
    public class EnemyController {
        readonly PooledObjectsOnTimer<EnemyView, Enemy, Unit> _enemyPool;
        readonly IPlayerPositionSubscription _playerPosition;
        readonly IGameEventSubscriber _gameEventDispatcher;

        public EnemyController(
            IEnemyConfig config, GameEventDispatcher eventDispatcher, ScreenBoundsChecker screenBoundsChecker, IPlayerPositionSubscription playerPosition
        ) {
            _enemyPool = new (
                poolInitialSize: config.EnemyPoolInitialSize,
                spawnPeriod: config.EnemySpawnPeriod,
                createView: () => Object.Instantiate(config.EnemyPrefab),
                createInit: createInit,
                getView: init => init.view,
                getPosition: _ => screenBoundsChecker.RandomPositionInsideBounds,
                data: Unit._
            );
            _playerPosition = playerPosition;
            _gameEventDispatcher = eventDispatcher;

            Enemy createInit(EnemyView view, Unit _) {
                return new (view, eventDispatcher, config.EnemySpeed);
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