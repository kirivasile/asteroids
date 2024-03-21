using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Asteroids.UI {
    public class PostGameUIView : MonoBehaviour, IScreenView {
        [SerializeField] Canvas _canvas;
        [SerializeField] Button _restartGame;
        [SerializeField] TextMeshProUGUI _finalScoreText;

        public void SetActive(bool value) {
            _canvas.enabled = value;
        }
        
        public void BindRestartGame(UnityAction action) {
            _restartGame.onClick.AddListener(action);
        }

        public void UpdateData(int finalScore) {
            _finalScoreText.text = $"You lost. Final score is {finalScore}";
        }
    }
}
