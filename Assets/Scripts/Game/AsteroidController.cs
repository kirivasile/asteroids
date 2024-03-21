using System.Collections.Generic;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;
using UnityEngine;

namespace Asteroids.Game {
    using AsteroidViewPool = PooledObjectsOnTimer<AsteroidView, Asteroid, Unit>;
    using AsteroidMiniViewPool = PooledObjectsOnEvent<AsteroidView, AsteroidMini, AsteroidMiniData>;

    public class AsteroidController {
        readonly GameConfigSO _gameConfig;
        readonly AsteroidViewPool _asteroidPool;
        readonly AsteroidMiniViewPool _miniAsteroidPool;
        readonly IGameEventSubscriber _eventDispatcher;

        public AsteroidController(GameConfigSO gameConfig, GameEventDispatcher eventDispatcher, ScreenBoundsChecker screenBoundsChecker) {
            _gameConfig = gameConfig;
            _eventDispatcher = eventDispatcher;
            _asteroidPool = new(
                poolInitialSize: gameConfig.PoolInitialSize,
                spawnPeriod: gameConfig.AsteroidSpawnPeriod,
                createView: () => Object.Instantiate(gameConfig.AsteroidPrefab),
                createInit: createInit,
                getView: init => init.view,
                getPosition: _ => screenBoundsChecker.RandomPositionInsideBounds,
                data: Unit._
            );
            _miniAsteroidPool = new(
                poolInitialSize: gameConfig.PoolInitialSize,
                createView: () => Object.Instantiate(gameConfig.AsteroidPrefab),
                createInit: createInitForAsteroidMini,
                getPosition: data => data.position,
                getView: init => init.view
            );

            Asteroid createInit(AsteroidView view, Unit _) {
                // var randomPos = new Vector3(
                //     Random.Range(screenBoundsChecker.leftBottomFieldWorldPos.x, screenBoundsChecker.rightTopFieldWorldPos.x),
                //     Random.Range(screenBoundsChecker.leftBottomFieldWorldPos.y, screenBoundsChecker.rightTopFieldWorldPos.y),
                //     0f
                // );
                var movementVector = Random.insideUnitCircle.normalized * gameConfig.AsteroidSpeed;

                return new Asteroid(
                    view, movementVector, gameConfig.AsteroidScale, eventDispatcher, 
                    screenBoundsChecker, gameConfig
                );
            }

            AsteroidMini createInitForAsteroidMini(AsteroidView view, AsteroidMiniData data) {
                return new AsteroidMini(
                    view, data.movementVector, gameConfig.AsteroidMiniScale, eventDispatcher, 
                    screenBoundsChecker, gameConfig
                );
            }
        }

        public void OnUpdate(float deltaTime) {
            _asteroidPool.OnUpdate();

            foreach (var asteroid in _asteroidPool.ActiveObjects) {
                asteroid.Move(deltaTime: deltaTime);
            }
            foreach (var miniAsteroid in _miniAsteroidPool.ActiveObjects) {
                miniAsteroid.Move(deltaTime: deltaTime);
            }
        }

        void DisableAsteroid(Asteroid asteroid, AsteroidView.PlayerWeaponType weaponType) {
            _asteroidPool.Release(asteroid);

            // We can split asteroids only using projectiles
            if (weaponType == AsteroidView.PlayerWeaponType.Projectile) {
                for (var i = 0; i < _gameConfig.AsteroidMinisPerAsteroid; ++i) {
                    _miniAsteroidPool.SpawnObject(new(
                        position: asteroid.Position,
                        movementVector: Random.insideUnitCircle.normalized * _gameConfig.AsteroidMiniSpeed
                    ));
                }
            }
        }

        void DisableAsteroidMini(AsteroidMini asteroid) {
            _miniAsteroidPool.Release(asteroid);
        }

        public void Enable() {
            _asteroidPool.Enable();

            _eventDispatcher.AsteroidDestroyed += DisableAsteroid;
            _eventDispatcher.MiniAsteroidDestroyed += DisableAsteroidMini;
        }

        public void Disable() {
            _eventDispatcher.AsteroidDestroyed -= DisableAsteroid;
            _eventDispatcher.MiniAsteroidDestroyed -= DisableAsteroidMini;

            _asteroidPool.Disable();
            _miniAsteroidPool.Disable();
        }
    }

    struct AsteroidMiniData {
        public readonly Vector3 position;
        public readonly Vector3 movementVector;

        public AsteroidMiniData(Vector3 position, Vector3 movementVector) {
            this.position = position;
            this.movementVector = movementVector;
        }
    }
}