using System;
using UnityEngine;

using Asteroids.Game;
using Asteroids.UI;
using Asteroids.Configs;

namespace Asteroids.Core {
    // Game entry point.
    public class GameEntryPoint : MonoBehaviour {
        [SerializeField] PreStartView _preStartUI;
        [SerializeField] InGameUIView _inGameUI;
        [SerializeField] PostGameUIView _postGameUI;
        [SerializeField] GameConfigSO _gameConfig;
        [SerializeField] Camera _mainCamera;

        GameController _gameController;
        UIController _uiController; 
        GameEventDispatcher _gameEventDispatcher;

        event Action<float> _onUpdate;

        void Start() {
            Initialize();
        }

        void Update() => _onUpdate.Invoke(Time.deltaTime);

        void Initialize() {
            _gameEventDispatcher = new GameEventDispatcher();
            var gameController = _gameController = new GameController(_gameConfig, _mainCamera, _gameEventDispatcher);
            _uiController = new UIController(
                _preStartUI, _inGameUI, _postGameUI, _gameEventDispatcher, gameController.ScoreCounter, 
                playerModel: gameController
            );

            _onUpdate += _gameController.OnUpdate;
            _onUpdate += _ => _uiController.OnUpdate();
        }

    }
}