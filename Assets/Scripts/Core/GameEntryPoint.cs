using System;
using UnityEngine;

using Asteroids.Game;
using Asteroids.UI;
using Asteroids.Configs;
using Asteroids.Utils;

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
            // This controller won't call any dispose. 
            // I use it for the the subscriptions that should persist after the game finished and restarted.
            var noDisposableController = new NoDisposableController();
            var scoreCounter = new ScoreCounter(gameEventDispatcher, _gameConfig.ScoreConfig, noDisposableController);

            var gameController = new GameController(
                _gameConfig, _mainCamera, gameEventDispatcher, noDisposableController, scoreCounter
            );
            var uiController = new UIController(
                _preStartUI, _inGameUI, _postGameUI, gameEventDispatcher, noDisposableController, 
                scoreCounter,  gameController.PlayerDataModel
            );

            _onUpdate += gameController.OnUpdate;
        }

    }
}