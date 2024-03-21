using UnityEngine;

namespace Asteroids.Views {
    public class PlayerView : MonoBehaviour {
        [SerializeField] LineRenderer _laser;

        public bool LaserIsActive {
            get => _laser.gameObject.activeSelf;
            set => _laser.gameObject.SetActive(value);
        }

        public void SetLinePosition(int idx, Vector3 position) => _laser.SetPosition(idx, position);
    }
}
