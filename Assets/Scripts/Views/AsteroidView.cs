using System;
using System.Collections;
using System.Collections.Generic;
using Asteroids.Utils;
using UnityEngine;

namespace Asteroids.Views {
    // TODO KV:  Change for small asteroids
    public class AsteroidView : MonoBehaviour, IPoolView {
        // TODO KV: REmove all tags
        public static readonly string TAG = "Asteroid";

        public event Action<PlayerWeaponType> onCollisionWithPlayerWeapon;

        public void OnCollisionWithPlayerWeapon(PlayerWeaponType weaponType) {
            onCollisionWithPlayerWeapon?.Invoke(weaponType);
        } 

        void IPoolView.OnRelease() {
            onCollisionWithPlayerWeapon = null;
        }

        public enum PlayerWeaponType { Projectile = 0, Laser = 1 };

    }
}
