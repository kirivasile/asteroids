using Asteroids.Game;
using Asteroids.Utils;

namespace Asteroids.UI {
    // Represents a view that uses the whole screen.
    // Only one can be active at the moment
    public interface IScreenView {
        void SetActive(bool value);
    }

    public class UIController {
        readonly IUIPlayerDataModel _playerModel;
        readonly InGameUIView _inGameUI;

        public UIController(
            PreStartView preStartUI, InGameUIView inGameUI, PostGameUIView postGameUI,
            GameEventDispatcher eventDispatcher, 
            IScoreCounter scoreCounter, IUIPlayerDataModel playerModel
        ) {
            _playerModel = playerModel;
            _inGameUI = inGameUI;
            var screenSelector = new ScreenViewSelector(preStartUI, inGameUI, postGameUI);

            screenSelector.Show(preStartUI);

            preStartUI.BindStartGame(StartGame);
            postGameUI.BindRestartGame(StartGame);

            eventDispatcher.Subscribe(SimpleGameEvent.GameFinished, ShowPostGameUI);

            void StartGame() {
                eventDispatcher.Push(SimpleGameEvent.GameStarted);
                screenSelector.Show(inGameUI);
            }

            void ShowPostGameUI() {
                screenSelector.Show(postGameUI);
                postGameUI.UpdateData(scoreCounter.Score);
            }
        }

        public void OnUpdate() {
            if (!_playerModel.PlayerData.Value(out var playerData)) return;

            _inGameUI.UpdateView(playerData);
        }

        // Helper class to enforce the rull, that every `IScreenView` is shown alone.
        public class ScreenViewSelector {
            readonly IScreenView[] _screenViews;

            public ScreenViewSelector(params IScreenView[] screenViews) {
                _screenViews = screenViews;
            }

            public void Show(IScreenView screenView) {
                foreach (var view in _screenViews) {
                    view.SetActive(view == screenView);
                }
            }
        }
    }
}