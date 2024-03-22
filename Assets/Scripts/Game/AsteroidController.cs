using UnityEngine;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;

namespace Asteroids.Game {
    using AsteroidViewPool = PooledObjectsOnTimer<AsteroidView, Asteroid, Unit>;
    using AsteroidMiniViewPool = PooledObjectsOnEvent<AsteroidView, AsteroidMini, AsteroidMiniData>;

    public class AsteroidController {
        readonly IAsteroidConfig _asteroidConfig;
        readonly AsteroidViewPool _asteroidPool;
        readonly AsteroidMiniViewPool _miniAsteroidPool;
        readonly IGameEventSubscriber _eventDispatcher;

        public AsteroidController(IAsteroidConfig config, GameEventDispatcher eventDispatcher, ScreenBoundsChecker screenBoundsChecker) {
            _asteroidConfig = config;
            _eventDispatcher = eventDispatcher;
            _asteroidPool = new(
                poolInitialSize: config.AsteroidPoolInitialSize,
                spawnPeriod: config.AsteroidSpawnPeriod,
                createView: () => Object.Instantiate(config.AsteroidPrefab),
                createInit: createInit,
                getView: init => init.view,
                getPosition: _ => screenBoundsChecker.RandomPositionInsideBounds,
                data: Unit._
            );
            _miniAsteroidPool = new(
                poolInitialSize: config.AsteroidPoolInitialSize,
                createView: () => Object.Instantiate(config.AsteroidPrefab),
                createInit: createInitForAsteroidMini,
                getPosition: data => data.position,
                getView: init => init.view
            );

            Asteroid createInit(AsteroidView view, Unit _) {
                var movementVector = Random.insideUnitCircle.normalized * config.AsteroidSpeed;

                return new Asteroid(
                    view, movementVector, config.AsteroidScale, eventDispatcher, 
                    screenBoundsChecker
                );
            }

            AsteroidMini createInitForAsteroidMini(AsteroidView view, AsteroidMiniData data) {
                return new AsteroidMini(
                    view, data.movementVector, config.AsteroidMiniScale, eventDispatcher, 
                    screenBoundsChecker
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

        void DisableAsteroid(Asteroid asteroid, AsteroidView.PlayerWeaponType weaponType) {
            _asteroidPool.Release(asteroid);

            // We can split asteroids only using projectiles. Laser destroys asteroids immediately.
            if (weaponType == AsteroidView.PlayerWeaponType.Projectile) {
                for (var i = 0; i < _asteroidConfig.AsteroidMinisPerAsteroid; ++i) {
                    _miniAsteroidPool.SpawnObject(new(
                        position: asteroid.Position,
                        movementVector: Random.insideUnitCircle.normalized * _asteroidConfig.AsteroidMiniSpeed
                    ));
                }
            }
        }

        void DisableAsteroidMini(AsteroidMini asteroid) {
            _miniAsteroidPool.Release(asteroid);
        }
    }

    // Container of the initialization data for `AsteroidMini`
    struct AsteroidMiniData {
        public readonly Vector3 position;
        public readonly Vector3 movementVector;

        public AsteroidMiniData(Vector3 position, Vector3 movementVector) {
            this.position = position;
            this.movementVector = movementVector;
        }
    }
}