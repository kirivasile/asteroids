using Asteroids.Game;

namespace Asteroids.UI {
    // Represents a view that uses the whole screen.
    // Only one can be active at the moment
    public interface IScreenView {
        void SetActive(bool value);
    }

    // Controller for every part of UI in the game.
    public class UIController {
        public UIController(
            PreStartView preStartUI, InGameUIView inGameUI, PostGameUIView postGameUI,
            GameEventDispatcher eventDispatcher, 
            IScoreCounter scoreCounter, IUIPlayerDataModel playerModel
        ) {
            var screenSelector = new ScreenViewSelector(preStartUI, inGameUI, postGameUI);

            screenSelector.Show(preStartUI);

            preStartUI.BindStartGame(StartGame);
            postGameUI.BindRestartGame(StartGame);

            eventDispatcher.Subscribe(SimpleGameEvent.GameFinished, ShowPostGameUI);
            playerModel.Observe += inGameUI.UpdateView;

            void StartGame() {
                eventDispatcher.Push(SimpleGameEvent.GameStarted);
                screenSelector.Show(inGameUI);
            }

            void ShowPostGameUI() {
                screenSelector.Show(postGameUI);
                postGameUI.UpdateData(scoreCounter.Score);
            }
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