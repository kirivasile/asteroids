using UnityEngine;

namespace Asteroids.Game {
    // TODO KV: do we need interface?
    public interface IPlayerUIData {
        Vector2 PlayerPosition { get; }
        float PlayerRotation { get; }
        float PlayerSpeed { get; }
        int LaserChargeCount { get; }
        float LaserRechargeTimer { get; }
    }

    public readonly struct PlayerUIData : IPlayerUIData {
        readonly Vector2 _playerPosition;
        readonly float _playerRotation;
        readonly float _playerSpeed;
        readonly int _laserChargeCount;
        readonly float _laserRechargeTimer;

        public PlayerUIData(Vector2 playerPosition, float playerRotation, float playerSpeed, int laserCount, float laserRechargeTimer) {
            _playerPosition = playerPosition;
            _playerRotation = playerRotation;
            _playerSpeed = playerSpeed;
            _laserChargeCount = laserCount;
            _laserRechargeTimer = laserRechargeTimer;
        }

        Vector2 IPlayerUIData.PlayerPosition => _playerPosition;
        float IPlayerUIData.PlayerRotation => _playerRotation;
        float IPlayerUIData.PlayerSpeed => _playerSpeed;
        int IPlayerUIData.LaserChargeCount => _laserChargeCount;
        float IPlayerUIData.LaserRechargeTimer => _laserRechargeTimer;
    }
}