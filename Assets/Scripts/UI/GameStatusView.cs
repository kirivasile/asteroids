using UnityEngine;
using TMPro;
using Asteroids.Game;

namespace Asteroids.UI {
    // In-game UI displaying the player's stats
    public class GameStatusView: MonoBehaviour {
        [SerializeField] TextMeshProUGUI _playerPosition;
        [SerializeField] TextMeshProUGUI _playerRotation;
        [SerializeField] TextMeshProUGUI _playerSpeed;
        [SerializeField] TextMeshProUGUI _laserChargeCount;
        [SerializeField] TextMeshProUGUI _laserRechargeTimer;

        public void UpdateData(IPlayerUIData playerUIData) {
            _playerPosition.text = $"Position: {playerUIData.PlayerPosition}";
            _playerRotation.text = $"Rotation: {playerUIData.PlayerRotation:0.0}";
            _playerSpeed.text = $"Speed: {playerUIData.PlayerSpeed:0.0}";
            _laserChargeCount.text = $"Laser charges: {playerUIData.LaserChargeCount}";
            _laserRechargeTimer.text = $"Laser recharges in: {playerUIData.LaserRechargeTimer:0.0}";
        }
    }
}