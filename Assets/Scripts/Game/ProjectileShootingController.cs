using System.Collections.Generic;
using UnityEngine;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;

namespace Asteroids.Game {
    using ProjectileViewPool = PooledObjectsOnEvent<ProjectileView, Projectile, Vector3>;

    public class ProjectileShootingController {
        readonly IShootingConfig _config;
        readonly ProjectileViewPool _projectilePool;
        readonly IPlayerWithPosition _player;
        readonly LayerMask _collisionLayerMask;

        // Buffer to collect all the projectiles to delete expired or destroyed projectiles during the frame.
        readonly List<Projectile> _projectilesToDisableBuf;

        public ProjectileShootingController(IPlayerWithPosition player, IShootingConfig shootingConfig, LayerMask collisionMask) {
            _config = shootingConfig;
            _player = player;
            _collisionLayerMask = collisionMask;

            _projectilePool = new(
                poolInitialSize: shootingConfig.ProjectilePoolInitialSize,
                createView: () => Object.Instantiate(_config.PlayerShootingProjectile),
                createInit: createInit,
                getView: init => init.view,
                getPosition: data => data
            );
            _projectilesToDisableBuf = new();

            Projectile createInit(ProjectileView view, Vector3 position) {
                return new Projectile(
                    view, movementVector: _player.ForwardVector.value * _config.ProjectileSpeed,
                    Time.time, _collisionLayerMask
                );
            }
        }

        // Maybe split dispose and update in interfaces
        public void OnUpdate(float dTime) {
            var currentTime = Time.time;

            _projectilesToDisableBuf.Clear();

            foreach (var projectile in _projectilePool.ActiveObjects) {
                var diedFromTime = projectile.creationTime + _config.ProjectileLifeDuration <= currentTime;
                var diedFromCollision = projectile.CheckCollisions(dTime);
                if (diedFromTime || diedFromCollision) {
                    _projectilesToDisableBuf.Add(projectile);


                    {if (diedFromCollision && diedFromCollision.transform.TryGetComponent<AsteroidView>(out var asteroid)) {
                        asteroid.OnCollisionWithPlayerWeapon(AsteroidView.PlayerWeaponType.Projectile);
                    }}
                    {if (diedFromCollision && diedFromCollision.transform.TryGetComponent<EnemyView>(out var enemy) && enemy.gameObject.activeSelf) {
                        enemy.OnCollisionWithPlayerWeapon();
                    }}
                }
                else {
                    projectile.Move(dTime);
                }
            }

            foreach (var projectile in _projectilesToDisableBuf) {
                DisableProjectile(projectile);
            }
        }

        public void Enable() {
            _config.MainShootAction.Enable();
            _config.MainShootAction.performed += ctx => ShootProjectile();
        }

        public void Disable() {
            _config.MainShootAction.Disable();
            _projectilePool.Disable();
            _projectilesToDisableBuf.Clear();
        }

        void ShootProjectile() => _projectilePool.SpawnObject(_player.Position);

        void DisableProjectile(Projectile projectile) => _projectilePool.Release(projectile);
    }
}