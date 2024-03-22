using UnityEngine;
using Asteroids.Configs;
using Asteroids.Utils;
using Asteroids.Views;

namespace Asteroids.Game {
    // Interface, that provide only get methods for the player's position and rotation.
    public interface IPlayerWithPosition {
        Vector3 Position { get; }
        float Rotation { get; }
        float CollisionRadius { get; }
        Normalized<Vector3> ForwardVector { get; }
    }

    // Interface, that provides the control for the player's position and rotation.
    public interface IMovablePlayer : IPlayerWithPosition {
        void Rotate(float value);
        void Move(Vector3 movementVector);
        void ResetPosition();
    }

    // Interface for the laser weapon of the player.
    public interface ILaserWeapon {
        bool IsLaserActive { get; set; }
        Vector3 LaserStartPosition { get; }
        Vector3 LaserEndPosition { get; }
    }

    // Class, containg the logic for the player.
    public class Player : ILaserWeapon, IMovablePlayer {
        readonly PlayerView _view;
        readonly ScreenBoundsChecker _screenBoundsChecker;
        readonly float _playerCollisionRadius;
        readonly Vector3 _initialPosition;
        readonly float _laserLength;

        public Vector3 Position => _view.transform.position;
        public float Rotation => _view.transform.eulerAngles.z;
        public float CollisionRadius => _playerCollisionRadius;
        public Normalized<Vector3> ForwardVector => ConvertEulerAnglesToVector(_view.transform.eulerAngles);

        public Vector3 LaserStartPosition => Position;
        public Vector3 LaserEndPosition => LaserStartPosition + ForwardVector.value * _laserLength;

        public bool IsLaserActive {
            get => _view.LaserIsActive;
            set => _view.LaserIsActive = value;
        }

        public Player(PlayerView view, IPlayerMovementConfig playerMovementConfig, ILaserConfig laserConfig, ScreenBoundsChecker screenBoundsChecker) {
            _view = view;
            _screenBoundsChecker = screenBoundsChecker;
            _laserLength = laserConfig.LaserLength;
            // Player have a Polygon Collider. It's easier to use just scale for the collision radius.
            _playerCollisionRadius = view.transform.localScale.x * 0.5f;

            _initialPosition = playerMovementConfig.PlayerStartPosition;

            var forwardVector = ConvertEulerAnglesToVector(_view.transform.eulerAngles);

            view.SetLinePosition(0, view.transform.position);
            view.SetLinePosition(1, view.transform.position + forwardVector.value * laserConfig.LaserLength);
        }

        public void Rotate(float value) {
            // We use backward vector. Because rotating left in our game means adding a positive angle to the rotation
            _view.transform.Rotate(Vector3.back * value);
        }

        public void Move(Vector3 movementVector) {
            _view.transform.position = _screenBoundsChecker.WrapScreenBounds(_view.transform.position + movementVector);
        }

        public void ResetPosition() {
            _view.transform.position = _initialPosition;
        }

        public void SetActive(bool value) {
            _view.gameObject.SetActive(value);
        }

        static Normalized<Vector3> ConvertEulerAnglesToVector(Vector3 eulerAngles) {
            var angle = (eulerAngles.z + 90f) * Mathf.Deg2Rad;
            return new Normalized<Vector3>(new(Mathf.Cos(angle), Mathf.Sin(angle)));
        }
    }
}