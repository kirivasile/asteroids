using System;
using Asteroids.Utils;
using UnityEngine;

namespace Asteroids.Views {
    [RequireComponent(typeof(Collider2D))]
    public class AsteroidView : MonoBehaviour, IPoolView {
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
