using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Asteroids.Views;

namespace Asteroids.Configs {
    [CreateAssetMenu(fileName = "Game Config", menuName = "ScriptableObjects/GameConfig", order = 1)]
    // TODO KV: Maybe extract public variables into interfaces?
    public class GameConfigSO : ScriptableObject {
        // TODO KV: group and re-order them
        [Header("Prefabs")]
        [SerializeField] PlayerView _playerPrefab;
        [SerializeField] EnemyView _enemyPrefab;
        [SerializeField] AsteroidView _asteroidPrefab;
        [SerializeField] ProjectileView _playerShootingProjectile;

        [Header("Player")]
        [SerializeField] Vector2 _playerStartPosition;
        [SerializeField] float _playerForwardAcceleration;
        [SerializeField] float _playerDeceleration;
        // TODO KV: Adjust parameters as in video https://www.youtube.com/watch?v=1a9ag16PeFw
        [SerializeField] float _playerRotateSpeed;
        [SerializeField] float _playerMaxSpeed;

        [Header("Input")]
        [SerializeField] InputAction _rotateAction;
        [SerializeField] InputAction _moveAction;
        [SerializeField] InputAction _mainShootAction;
        [SerializeField] InputAction _laserShootAction;

        [Header("Screen bounds")]
        // TODO KV: docme
        [SerializeField, Range(0f, 0.2f)] float _screenBoundsThreshold;
        // TODO KV: rename
        [SerializeField, Range(0f, 0.2f)] float _teleportPositionForScreenBounds;

        [Header("Pooling")]
        [SerializeField] int _poolInitialSize;

        [Header("Projectiles")]
        [SerializeField] float _projectileSpeed;
        [SerializeField] int _projectileLifeDuration;

        [Header("Asteroids")]
        [SerializeField] float _asteroidSpawnPeriod;
        [SerializeField] float _asteroidSpeed;
        [SerializeField] float _asteroidScale;

        [Header("Mini-asteroids")]
        [SerializeField] int _miniAsteroidsPerAsteroid;
        [SerializeField] float _miniAsteroidSpeed;
        [SerializeField] float _miniAsteroidScale;

        [Header("Laser")]
        [SerializeField] float _laserDuration;
        [SerializeField] float _laserLength;
        [SerializeField] float _laserChargeCooldown;
        [SerializeField] int _laserStartCharges;

        [Header("Enemies")]
        [SerializeField] float _enemySpawnPeriod;
        [SerializeField] float _enemySpeed;

        [Header("Scores")]
        [SerializeField] int _asteroidScore;
        [SerializeField] int _miniAsteroidScore;
        [SerializeField] int _enemyScore;

        public PlayerView PlayerPrefab => _playerPrefab;
        public EnemyView EnemyPrefab => _enemyPrefab;
        public AsteroidView AsteroidPrefab => _asteroidPrefab;
        public ProjectileView PlayerShootingProjectile => _playerShootingProjectile;

        public Vector2 PlayerStartPosition => _playerStartPosition;
        public float PlayerForwardAcceleration => _playerForwardAcceleration;
        public float PlayerDeceleration => _playerDeceleration;
        public float PlayerRotateSpeed => _playerRotateSpeed;
        public float PlayerMaxSpeed => _playerMaxSpeed;

        public InputAction RotateAction => _rotateAction;
        public InputAction MoveAction => _moveAction;
        public InputAction MainShootAction => _mainShootAction;
        public InputAction LaserShootAction => _laserShootAction;

        public float ScreenBoundsThreshold => _screenBoundsThreshold;
        public float TeleportPositionForScreenBounds => _teleportPositionForScreenBounds;

        public int PoolInitialSize => _poolInitialSize;

        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifeDuration => _projectileLifeDuration;

        public float AsteroidSpawnPeriod => _asteroidSpawnPeriod;
        public float AsteroidSpeed => _asteroidSpeed;
        public float AsteroidScale => _asteroidScale;

        public int AsteroidMinisPerAsteroid => _miniAsteroidsPerAsteroid;
        public float AsteroidMiniSpeed => _miniAsteroidSpeed;
        public float AsteroidMiniScale => _miniAsteroidScale;

        public float LaserDuration => _laserDuration;
        public float LaserLength => _laserLength;
        public float LaserChargeCooldown => _laserChargeCooldown;
        public int LaserStartCharges => _laserStartCharges;
        public float EnemySpawnPeriod => _enemySpawnPeriod;
        public float EnemySpeed => _enemySpeed;

        public int AsteroidScore => _asteroidScore;
        public int MiniAsteroidScore => _miniAsteroidScore;
        public int EnemyScore => _enemyScore;
    }
}
