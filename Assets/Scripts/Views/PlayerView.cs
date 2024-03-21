using UnityEngine;

namespace Asteroids.Views {
    // TODO KV: Maybe better split views and logic?
    public class PlayerView : MonoBehaviour {
        public static string TAG = "Player";

        [SerializeField] LineRenderer _laser;

        public bool LaserIsActive {
            get => _laser.gameObject.activeSelf;
            set => _laser.gameObject.SetActive(value);
        }

        public void SetLinePosition(int idx, Vector3 position) => _laser.SetPosition(idx, position);
    }
}
