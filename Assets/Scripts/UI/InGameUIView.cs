using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Asteroids.Game;

namespace Asteroids.UI {
    public class InGameUIView : MonoBehaviour, IScreenView {
        [SerializeField] Canvas _canvas;
        [SerializeField] GameStatusView _gameStatusView;

        public void SetActive(bool value) {
            _canvas.enabled = value;
        }

        public void UpdateView(IPlayerUIData updateData) {
            _gameStatusView.UpdateData(updateData);
        }
    }
}
