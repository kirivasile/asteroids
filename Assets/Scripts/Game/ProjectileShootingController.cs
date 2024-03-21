using System.Collections.Generic;
using UnityEngine;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;
using System;

namespace Asteroids.Game {
    // TODO KV: Maybe delete all explicit interface implementation
    using ProjectileViewPool = PooledObjectsOnEvent<ProjectileView, Projectile, Vector3>;

    public class ProjectileShootingController {
        readonly GameConfigSO _gameConfig;
        readonly ProjectileViewPool _projectilePool;
        // TODO KV: docme
        readonly List<Projectile> _projectilesToDisableBuf;
        readonly IPlayerWithPosition _player;
        readonly LayerMask _collisionLayerMask;

        public ProjectileShootingController(GameConfigSO gameConfig, IPlayerWithPosition player) {
            _gameConfig = gameConfig;
            _player = player;

            _projectilePool = new(
                poolInitialSize: gameConfig.PoolInitialSize,
                createView: () => UnityEngine.Object.Instantiate(_gameConfig.PlayerShootingProjectile),
                createInit: createInit,
                getView: init => init.view,
                getPosition: data => data
            );
            _projectilesToDisableBuf = new();
            _collisionLayerMask = 1 << gameConfig.AsteroidPrefab.gameObject.layer | 1 << gameConfig.EnemyPrefab.gameObject.layer;

            Projectile createInit(ProjectileView view, Vector3 position) {
                return new Projectile(
                    view, movementVector: _player.ForwardVector.value * _gameConfig.ProjectileSpeed,
                    Time.time, _collisionLayerMask
                );
            }
        }

        // Maybe split dispose and update in interfaces
        public void OnUpdate(float dTime) {
            var currentTime = Time.time;

            _projectilesToDisableBuf.Clear();

            foreach (var projectile in _projectilePool.ActiveObjects) {
                var diedFromTime = projectile.creationTime + _gameConfig.ProjectileLifeDuration <= currentTime;
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
            _gameConfig.MainShootAction.Enable();
            _gameConfig.MainShootAction.performed += ctx => ShootProjectile();
        }

        public void Disable() {
            _gameConfig.MainShootAction.Disable();
            _projectilePool.Disable();
            _projectilesToDisableBuf.Clear();
        }

        void ShootProjectile() => _projectilePool.SpawnObject(_player.Position);

        void DisableProjectile(Projectile projectile) => _projectilePool.Release(projectile);
    }
}