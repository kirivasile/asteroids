using System;
using System.Collections;
using System.Collections.Generic;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;
using UnityEngine;

namespace Asteroids.Game {
    public abstract class AsteroidBase{
        public readonly AsteroidView view;

        readonly Vector3 _movementVector;
        readonly ScreenBoundsChecker _screenBoundsChecker;

        public Vector3 Position => view.transform.position;

        // TODO KV: Don't like this flag
        // The flag is used to prevent 
        protected bool _isEnabled;

        public AsteroidBase(AsteroidView view, Vector3 movementVector, float scale, ScreenBoundsChecker screenBoundsChecker) {
            this.view = view;

            view.transform.localScale = Vector3.one * scale;

            _movementVector = movementVector;
            _screenBoundsChecker = screenBoundsChecker;

            _isEnabled = true;
        }

        // Some of the asteroids get several calls during one update. When other is more rare
        public void Move(float deltaTime) {
            view.transform.position = _screenBoundsChecker.WrapScreenBounds(
                view.transform.position + _movementVector * deltaTime
            );
        }

        public override int GetHashCode() {
            return view.GetInstanceID().GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj is AsteroidBase && Equals((AsteroidBase)obj);
        }

        public bool Equals(AsteroidBase other) {
            return view.GetInstanceID() == other.view.GetInstanceID();
        }
    }

    public class Asteroid : AsteroidBase {
        public Asteroid(
            AsteroidView view, Vector3 movementVector, float scale, IGameEventEmitter eventDispatcher, 
            ScreenBoundsChecker screenBoundsChecker, GameConfigSO gameConfig
        ) : base(view, movementVector, scale, screenBoundsChecker) {
            view.onCollisionWithPlayerWeapon += (AsteroidView.PlayerWeaponType weaponType) => {
                eventDispatcher.PushAsteroidDestroyed(this, weaponType);
                eventDispatcher.PushPlayerScored(gameConfig.AsteroidScore);
            };
        }
    }

    public class AsteroidMini : AsteroidBase {
        public AsteroidMini(
            AsteroidView view, Vector3 movementVector, float scale, IGameEventEmitter eventDispatcher, 
            ScreenBoundsChecker screenBoundsChecker, GameConfigSO gameConfig
        ) : base(view, movementVector, scale, screenBoundsChecker) {
            view.onCollisionWithPlayerWeapon += (AsteroidView.PlayerWeaponType _) => {
                eventDispatcher.PushAsteroidMiniDestroyed(this);
                eventDispatcher.PushPlayerScored(gameConfig.MiniAsteroidScore);
            };
        }
    }
}