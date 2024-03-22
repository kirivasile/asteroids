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

        event Action<float> _onUpdate;

        void Start() {
            Initialize();
        }

        void Update() => _onUpdate.Invoke(Time.deltaTime);

        void Initialize() {
            var gameEventDispatcher = new GameEventDispatcher();
            var gameController = new GameController(_gameConfig, _mainCamera, gameEventDispatcher);
            var uiController = new UIController(
                _preStartUI, _inGameUI, _postGameUI, gameEventDispatcher, gameController.ScoreCounter, 
                playerModel: gameController.PlayerDataModel
            );

            _onUpdate += gameController.OnUpdate;
        }

    }
}