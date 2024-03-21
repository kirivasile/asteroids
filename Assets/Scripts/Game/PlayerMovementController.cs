using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Controls;
using Asteroids.Configs;
using Asteroids.Utils;
using System;

namespace Asteroids.Game {
    // Can be done with delegates as well. 
    public interface IPlayerSpeedSubscription {
        float CurrentSpeed { get; }
    }

    public class PlayerMovementController : IPlayerSpeedSubscription {
        readonly IMovablePlayer _player;
        readonly GameConfigSO _gameConfig;

        // TODO KV: docme
        float _currentSpeed;

        public float CurrentSpeed => _currentSpeed;

        public PlayerMovementController(IMovablePlayer player, GameConfigSO gameConfig) {
            _player = player;
            _gameConfig = gameConfig;
        }

        public void OnUpdate(float deltaTime) {
            var rotationProof = HandleRotationInput(rotationInput: _gameConfig.RotateAction.ReadValue<float>(), rotateSpeed: _gameConfig.PlayerRotateSpeed);
            var forwardMovementProof = HandleMovementInput(
                movementInput: _gameConfig.MoveAction.ReadValue<float>(), currentSpeed: _currentSpeed, maxSpeed: _gameConfig.PlayerMaxSpeed,
                playerAcceleration: _gameConfig.PlayerForwardAcceleration, playerDeceleration: _gameConfig.PlayerDeceleration, 
                playerForwardVector: _player.ForwardVector, deltaTime: deltaTime
            );
            UpdatePlayerPositionAndRotation(_player, rotationProof, forwardMovementProof);

            _currentSpeed = forwardMovementProof.movementSpeed;
        }

        // TODO KV: docme
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
            _gameConfig.RotateAction.Enable();
            _gameConfig.MoveAction.Enable();
        }

        public void Disable() {
            _gameConfig.RotateAction.Disable();
            _gameConfig.MoveAction.Disable();
        }

        // TODO KV: docme
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