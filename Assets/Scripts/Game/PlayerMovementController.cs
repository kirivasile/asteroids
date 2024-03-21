using UnityEngine;
using Asteroids.Configs;
using Asteroids.Utils;

namespace Asteroids.Game {
    // Can be done with delegates as well. 
    public interface IPlayerSpeedSubscription {
        float CurrentSpeed { get; }
    }

    public class PlayerMovementController : IPlayerSpeedSubscription {
        readonly IMovablePlayer _player;
        readonly IPlayerMovementConfig _config;

        public float CurrentSpeed { get; private set; }

        public PlayerMovementController(IMovablePlayer player, IPlayerMovementConfig config) {
            _player = player;
            _config = config;
        }

        public void OnUpdate(float deltaTime) {
            var rotationProof = HandleRotationInput(rotationInput: _config.RotateAction.ReadValue<float>(), rotateSpeed: _config.PlayerRotateSpeed);
            var forwardMovementProof = HandleMovementInput(
                movementInput: _config.MoveAction.ReadValue<float>(), currentSpeed: CurrentSpeed, maxSpeed: _config.PlayerMaxSpeed,
                playerAcceleration: _config.PlayerForwardAcceleration, playerDeceleration: _config.PlayerDeceleration, 
                playerForwardVector: _player.ForwardVector, deltaTime: deltaTime
            );
            UpdatePlayerPositionAndRotation(_player, rotationProof, forwardMovementProof);

            CurrentSpeed = forwardMovementProof.movementSpeed;
        }

        
        static RotationInputHandledProof HandleRotationInput(float rotationInput, float rotateSpeed) {
            return new RotationInputHandledProof(rotationInput * rotateSpeed);
        }

        static MovementInputHandledProof HandleMovementInput(
            float movementInput, float currentSpeed, float maxSpeed, float playerAcceleration, float playerDeceleration, Normalized<Vector3> playerForwardVector, float deltaTime
        ) {
            currentSpeed += (movementInput * playerAcceleration - playerDeceleration) * deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

            var movementVector = playerForwardVector.value * currentSpeed * deltaTime;

            return new MovementInputHandledProof(movementVector, currentSpeed);
        }

        void UpdatePlayerPositionAndRotation(
            IMovablePlayer player, RotationInputHandledProof rotationProof, MovementInputHandledProof movementProof
        ) {
            player.Move(movementProof.movementVector);
            player.Rotate(rotationProof.addedRotation);
        }

        public void Enable() {
            _config.RotateAction.Enable();
            _config.MoveAction.Enable();
        }

        public void Disable() {
            CurrentSpeed = 0f;

            _config.RotateAction.Disable();
            _config.MoveAction.Disable();
        }

        // Set of structs, which enforce the correct order of the operations in `Update` methods.
        // At first we calculate new values and only then we use them to update the views.
        readonly struct MovementInputHandledProof{
            public readonly Vector3 movementVector;
            public readonly float movementSpeed;

            public MovementInputHandledProof(Vector3 movementVector, float movementSpeed) {
                this.movementVector = movementVector;
                this.movementSpeed = movementSpeed;
            }
        }

        readonly struct RotationInputHandledProof {
            public readonly float addedRotation;

            public RotationInputHandledProof(float addedRotation) {
                this.addedRotation = addedRotation;
            }
        }
    }
}