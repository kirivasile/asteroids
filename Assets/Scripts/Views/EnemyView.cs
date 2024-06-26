using System;
using Asteroids.Utils;
using UnityEngine;

namespace Asteroids.Views {
    [RequireComponent(typeof(Collider2D))]
    public class EnemyView : MonoBehaviour, IPoolView {
        public static readonly string TAG = "Enemy";

        public event Action onCollisionWithPlayerWeapon;

        public void OnCollisionWithPlayerWeapon() => onCollisionWithPlayerWeapon?.Invoke();

        void IPoolView.OnRelease() {
            onCollisionWithPlayerWeapon = null;
        }
    }
}
