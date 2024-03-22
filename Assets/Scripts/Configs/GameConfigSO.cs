using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Asteroids.Views;

namespace Asteroids.Configs {
    public interface IPlayerMovementConfig {
        PlayerView PlayerPrefab { get; }
        InputAction RotateAction { get; }
        InputAction MoveAction { get; }
        Vector2 PlayerStartPosition { get; }
        float PlayerForwardAcceleration { get; }
        float PlayerDeceleration { get; }
        float PlayerRotateSpeed { get; }
        float PlayerMaxSpeed { get; }
    }

    public interface IShootingConfig {
        ProjectileView Projectile { get; }
        InputAction ShootAction { get; }
        float ProjectileSpeed { get; }
        float ProjectileLifeDuration { get; }
        int ProjectilePoolInitialSize { get; }
    }

    public interface ILaserConfig {
        InputAction ShootAction { get; }
        float LaserDuration { get; }
        float LaserLength { get; }
        float LaserChargeCooldown { get; }
        int LaserStartCharges { get; }
    }

    public interface IAsteroidConfig {
        AsteroidView AsteroidPrefab { get; }
        float AsteroidSpawnPeriod { get; }
        float AsteroidSpeed { get; }
        float AsteroidScale { get; }
        int AsteroidMinisPerAsteroid { get; }
        float AsteroidMiniSpeed { get; }
        float AsteroidMiniScale { get; }
        int AsteroidPoolInitialSize { get; }
    }

    public interface IEnemyConfig {
        EnemyView EnemyPrefab { get; }
        float EnemySpawnPeriod { get; }
        float EnemySpeed { get; }
        int EnemyPoolInitialSize { get; }
    }

    public interface IScoreConfig {
        int AsteroidScore { get; }
        int MiniAsteroidScore { get; }
        int EnemyScore { get; }
    }

    public interface IScreenBoundsConfig {
        float ScreenBoundsThreshold { get; }
        float TeleportPositionForScreenBounds { get; }
    }

    [CreateAssetMenu(fileName = "Game Config", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfigSO : ScriptableObject {
        [SerializeField] PlayerMovementConfigSO _playerMovementConfig;
        [SerializeField] ShootingConfigSO _shootingConfig;
        [SerializeField] LaserConfigSO _laserConfig;
        [SerializeField] AsteroidConfigSO _asteroidConfig;
        [SerializeField] EnemyConfigSO _enemyConfig;
        [SerializeField] ScoreConfigSO _scoreConfig;     
        [SerializeField] ScreenBoundsConfigSO _screenBoundsConfig;

        public PlayerMovementConfigSO PlayerMovementConfig => _playerMovementConfig;
        public ShootingConfigSO ShootingConfig => _shootingConfig;
        public LaserConfigSO LaserConfig => _laserConfig;
        public AsteroidConfigSO AsteroidConfig => _asteroidConfig;
        public EnemyConfigSO EnemyConfig => _enemyConfig;
        public ScoreConfigSO ScoreConfig => _scoreConfig;
        public ScreenBoundsConfigSO ScreenBoundsConfig => _screenBoundsConfig;
    }

    [Serializable]
    public class PlayerMovementConfigSO : IPlayerMovementConfig {
        [SerializeField] PlayerView _playerPrefab;
        [SerializeField] InputAction _rotateAction;
        [SerializeField] InputAction _moveAction;
        [SerializeField] Vector2 _playerStartPosition;
        [SerializeField] float _playerForwardAcceleration;
        [SerializeField] float _playerDeceleration;
        [SerializeField] float _playerRotateSpeed;
        [SerializeField] float _playerMaxSpeed;

        public PlayerView PlayerPrefab => _playerPrefab;
        public InputAction RotateAction => _rotateAction;
        public InputAction MoveAction => _moveAction;
        public Vector2 PlayerStartPosition => _playerStartPosition;
        public float PlayerForwardAcceleration => _playerForwardAcceleration;
        public float PlayerDeceleration => _playerDeceleration;
        public float PlayerRotateSpeed => _playerRotateSpeed;
        public float PlayerMaxSpeed => _playerMaxSpeed;
    }

    [Serializable]
    public class ShootingConfigSO : IShootingConfig {
        [SerializeField] ProjectileView _projectile;
        [SerializeField] InputAction _shootAction;
        [SerializeField] float _projectileSpeed;
        [SerializeField] int _projectileLifeDuration;
        [SerializeField] int _projectilePoolInitialSize;

        public ProjectileView Projectile => _projectile;
        public InputAction ShootAction => _shootAction;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifeDuration => _projectileLifeDuration;
        public int ProjectilePoolInitialSize => _projectilePoolInitialSize;
    }

    [Serializable]
    public class LaserConfigSO : ILaserConfig {
        [SerializeField] InputAction _shootAction;
        [SerializeField] float _laserDuration;
        [SerializeField] float _laserLength;
        [SerializeField] float _laserChargeCooldown;
        [SerializeField] int _laserStartCharges;

        public InputAction ShootAction => _shootAction;
        public float LaserDuration => _laserDuration;
        public float LaserLength => _laserLength;
        public float LaserChargeCooldown => _laserChargeCooldown;
        public int LaserStartCharges => _laserStartCharges;
    }

    [Serializable]
    public class AsteroidConfigSO : IAsteroidConfig {
        [SerializeField] AsteroidView _asteroidPrefab;
        [SerializeField] float _asteroidSpawnPeriod;
        [SerializeField] float _asteroidSpeed;
        [SerializeField] float _asteroidScale;
        [SerializeField] int _asteroidPoolInitialSize;
        [SerializeField] int _miniAsteroidsPerAsteroid;
        [SerializeField] float _miniAsteroidSpeed;
        [SerializeField] float _miniAsteroidScale;

        public AsteroidView AsteroidPrefab => _asteroidPrefab;
        public float AsteroidSpawnPeriod => _asteroidSpawnPeriod;
        public float AsteroidSpeed => _asteroidSpeed;
        public float AsteroidScale => _asteroidScale;
        public int AsteroidMinisPerAsteroid => _miniAsteroidsPerAsteroid;
        public float AsteroidMiniSpeed => _miniAsteroidSpeed;
        public float AsteroidMiniScale => _miniAsteroidScale;
        public int AsteroidPoolInitialSize => _asteroidPoolInitialSize;
    }

    [Serializable]
    public class EnemyConfigSO : IEnemyConfig {
        [SerializeField] EnemyView _enemyPrefab;
        [SerializeField] float _enemySpawnPeriod;
        [SerializeField] float _enemySpeed;
        [SerializeField] int _enemyPoolInitialSize;

        public EnemyView EnemyPrefab => _enemyPrefab;
        public float EnemySpawnPeriod => _enemySpawnPeriod;
        public float EnemySpeed => _enemySpeed;
        public int EnemyPoolInitialSize => _enemyPoolInitialSize;
    }

    [Serializable]
    public class ScoreConfigSO : IScoreConfig {
        [SerializeField] int _asteroidScore;
        [SerializeField] int _miniAsteroidScore;
        [SerializeField] int _enemyScore;

        public int AsteroidScore => _asteroidScore;
        public int MiniAsteroidScore => _miniAsteroidScore;
        public int EnemyScore => _enemyScore;
    }

    [Serializable]
    public class ScreenBoundsConfigSO : IScreenBoundsConfig {
        // Sets the distance from the borders of the screen, where the entity will be teleported to another border.
        [SerializeField, Range(0f, 0.2f)] float _screenBoundsThreshold;
        // Set the distance from the borders of the screen, where the entities will be teleported.
        // This value should be greater than `_screenBoundsThreshold`
        [SerializeField, Range(0f, 0.2f)] float _teleportPositionForScreenBounds;

        public float ScreenBoundsThreshold => _screenBoundsThreshold;
        public float TeleportPositionForScreenBounds => _teleportPositionForScreenBounds;
    }
}
