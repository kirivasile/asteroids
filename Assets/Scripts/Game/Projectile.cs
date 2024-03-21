using UnityEngine;
using Asteroids.Views;

namespace Asteroids.Game {
    public class Projectile {
        public readonly ProjectileView view;

        // Vector contains the direction and the speed of the projectile.
        public readonly Vector3 movementVector;

        // The moment the projectile was created
        public readonly float creationTime;

        readonly float _collisionRadius;

        readonly LayerMask _collisionLayerMask;
        readonly float _speed;

        public Projectile(ProjectileView view, Vector3 movementVector, float creationTime, LayerMask collisionLayerMask) {
            this.view = view;
            this.movementVector = movementVector;
            this.creationTime = creationTime;

            _collisionRadius = view.transform.localScale.x * view.GetComponent<CircleCollider2D>().radius;
            _collisionLayerMask = collisionLayerMask;
            _speed = movementVector.magnitude;
        }

        public void Move(float deltaTime) {
            view.transform.Translate(movementVector * deltaTime);
        }

        // I can't use Unity Physics for the movement, that's why I can't use Rigidbodies to check collisions as well.
        // That's why I need to check collisions ourselves.
        public RaycastHit2D CheckCollisions(float deltaTime) =>
            Physics2D.CircleCast(origin: view.transform.position, radius: _collisionRadius, direction: movementVector, distance: _speed * deltaTime, layerMask: _collisionLayerMask);

        public override int GetHashCode() {
            return view.GetHashCode();
        }
    }
}