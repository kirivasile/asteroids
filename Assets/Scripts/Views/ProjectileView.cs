using UnityEngine;
using Asteroids.Utils;

namespace Asteroids.Views {
    [RequireComponent(typeof(CircleCollider2D))]
    public class ProjectileView : MonoBehaviour, IPoolView {

        void IPoolView.OnRelease() {
        }

    }

}