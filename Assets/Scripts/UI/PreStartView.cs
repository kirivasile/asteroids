using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Asteroids.UI {
    // UI before the the game starts.
    public class PreStartView : MonoBehaviour, IScreenView {
        [SerializeField] Canvas _canvas;
        [SerializeField] Button _startGame;

        public void SetActive(bool value) {
            _canvas.enabled = value;
        }

        public void BindStartGame(UnityAction action) {
            _startGame.onClick.AddListener(action);
        }
    }
}