using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Asteroids.Utils;

namespace Asteroids.Views {
    [RequireComponent(typeof(CircleCollider2D))]
    public class ProjectileView : MonoBehaviour, IPoolView {
        public static string TAG = "Projectile";

        void IPoolView.OnRelease() {
        }

    }

}