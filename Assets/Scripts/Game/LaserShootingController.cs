using System.Collections.Generic;
using UnityEngine;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;

namespace Asteroids.Game {
    public class LaserShootingController {
        readonly ILaserConfig _gameConfig;
        readonly ILaserWeapon _player;
        readonly LayerMask _collisionLayerMask;
        readonly int _laserMaxCharges;

        int _laserNumCharges;
        Option<float> _lastLaserShootStarted = None._;
        Option<float> _lastLaserRechargeHappened = None._;

        public int LaserCount => _laserNumCharges;
        public float LaserRechargeTimer {
            get {
                if (_laserNumCharges == _laserMaxCharges) return _gameConfig.LaserChargeCooldown;
                else {
                    if (!_lastLaserShootStarted.Value(out var lastLaserShootStarted)) {
                        Debug.LogError("LaserShootStarted field is not set, however the number of charges is less than initial one.");
                        return 0f;
                    }
                    else if (_lastLaserRechargeHappened.Value(out var lastLaserRechargeHappened)) {
                        return _gameConfig.LaserChargeCooldown - (Time.time - lastLaserRechargeHappened);
                    }
                    else {
                        return _gameConfig.LaserChargeCooldown - (Time.time - lastLaserShootStarted);
                    }
                }
            }
        }

        public LaserShootingController(ILaserWeapon player, ILaserConfig laserConfig, LayerMask collisionLayerMask) {
            _gameConfig = laserConfig;
            _player = player;

            _collisionLayerMask = collisionLayerMask;
            _laserNumCharges = laserConfig.LaserStartCharges;
            _laserMaxCharges = laserConfig.LaserStartCharges;
        }

        public void OnUpdate() {
            // If there wasn't any laser shot yet, we don't want to do anything.
            if (
                !_lastLaserShootStarted.Value(out var lastLaserShootStarted)
                || !_lastLaserRechargeHappened.Value(out var lastLaserRechargeHappened)
            ) return;

            if (_player.IsLaserActive) HandleExistingLaser();
            HandleLaserCharges();

            void HandleExistingLaser() {
                if (Time.time >= lastLaserShootStarted + _gameConfig.LaserDuration) {
                    _player.IsLaserActive = false;
                }
                else {
                    var hits = Physics2D.LinecastAll(start: _player.LaserStartPosition, end: _player.LaserEndPosition, layerMask: _collisionLayerMask);
                    foreach (var hit in hits) {
                        if (hit.transform.TryGetComponent<AsteroidView>(out var asteroid)) {
                            asteroid.OnCollisionWithPlayerWeapon(AsteroidView.PlayerWeaponType.Laser);
                        }
                        if (hit.transform.TryGetComponent<EnemyView>(out var enemy)) {
                            enemy.OnCollisionWithPlayerWeapon();
                        }
                    }
                }
            }

            void HandleLaserCharges() {
                if (_laserNumCharges >= _laserMaxCharges) return;

                if (Time.time >= lastLaserRechargeHappened + _gameConfig.LaserChargeCooldown) {
                    _laserNumCharges++;
                    _lastLaserRechargeHappened = Some._(Time.time);
                }
            }
        }

        public void Enable() {
            _gameConfig.LaserShootAction.Enable();
            _gameConfig.LaserShootAction.performed += ctx => ShootWithLaser();

            _laserNumCharges = _gameConfig.LaserStartCharges;
            _lastLaserRechargeHappened = None._;
            _lastLaserShootStarted = None._;
        }

        public void Disable() {
            _gameConfig.LaserShootAction.Disable();
        }

        void ShootWithLaser() {
            // If there was a maximum stacks of laser, then recharge cooldown should start with the shot.
            // Else we handle it in update.
            if (_laserNumCharges == _laserMaxCharges) {
                _lastLaserRechargeHappened = Some._(Time.time);
            }

            _laserNumCharges--;

            _lastLaserShootStarted = Some._(Time.time);
            _player.IsLaserActive = true;
        }
    }
}