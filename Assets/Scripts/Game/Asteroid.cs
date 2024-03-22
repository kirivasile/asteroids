using UnityEngine;
using Asteroids.Utils;
using Asteroids.Views;

namespace Asteroids.Game {
    // Base class containg the logic for the `AsteroidView`
    public abstract class AsteroidBase {
        public readonly AsteroidView view;

        readonly Vector3 _movementVector;
        readonly ScreenBoundsChecker _screenBoundsChecker;

        public Vector3 Position => view.transform.position;

        public AsteroidBase(AsteroidView view, Vector3 movementVector, float scale, ScreenBoundsChecker screenBoundsChecker) {
            this.view = view;

            view.transform.localScale = Vector3.one * scale;

            _movementVector = movementVector;
            _screenBoundsChecker = screenBoundsChecker;
        }

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

    // Class representing big asteroids. Can be destroyed into several `AsteroidMini`
    public class Asteroid : AsteroidBase {
        public Asteroid(
            AsteroidView view, Vector3 movementVector, float scale, IGameEventEmitter eventDispatcher, 
            ScreenBoundsChecker screenBoundsChecker
        ) : base(view, movementVector, scale, screenBoundsChecker) {
            view.onCollisionWithPlayerWeapon += (AsteroidView.PlayerWeaponType weaponType) => {
                eventDispatcher.PushAsteroidDestroyed(this, weaponType);
                eventDispatcher.PushPlayerScored(ScoreType.Asteroid);
            };
        }
    }

    // Class representing chunks of asteroids.
    public class AsteroidMini : AsteroidBase {
        public AsteroidMini(
            AsteroidView view, Vector3 movementVector, float scale, IGameEventEmitter eventDispatcher, 
            ScreenBoundsChecker screenBoundsChecker
        ) : base(view, movementVector, scale, screenBoundsChecker) {
            view.onCollisionWithPlayerWeapon += (AsteroidView.PlayerWeaponType _) => {
                eventDispatcher.PushAsteroidMiniDestroyed(this);
                eventDispatcher.PushPlayerScored(ScoreType.MiniAsteroid);
            };
        }
    }
}