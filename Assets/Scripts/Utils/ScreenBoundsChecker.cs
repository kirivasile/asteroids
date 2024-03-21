using UnityEngine;

namespace Asteroids.Utils {
    public class ScreenBoundsChecker {
        readonly Camera _mainCamera;
        readonly float _screenBoundsThreshold;
        readonly float _teleportThresholdForScreenBounds;

        // These two varaibles describe the borders of the screen/camera in world space.
        readonly Vector2 _leftBottomFieldWorldPos, _rightTopFieldWorldPos;

        public ScreenBoundsChecker(Camera mainCamera, float screenBoundsThreshold, float teleportThresholdForScreenBounds) {
            _mainCamera = mainCamera;
            _screenBoundsThreshold = screenBoundsThreshold;
            _teleportThresholdForScreenBounds = teleportThresholdForScreenBounds;

            var cameraAspect = mainCamera.aspect;
            var bottomLeftViewportPos = new Vector2(
                screenBoundsThreshold / cameraAspect,
                screenBoundsThreshold
            );
            var topRightViewportPos = new Vector2(
                1f - screenBoundsThreshold / cameraAspect,
                1f - screenBoundsThreshold
            );

            _leftBottomFieldWorldPos = mainCamera.ViewportToWorldPoint(bottomLeftViewportPos);
            _rightTopFieldWorldPos = mainCamera.ViewportToWorldPoint(topRightViewportPos);
        }

        // If the `position` is outside of screen bounds, the method calculates and returns new position within the screen bounds.
        // e.g. If the point is near the left border of the screen, it will get the position near the right border of the screen.
        public Vector3 WrapScreenBounds(Vector3 position) {
            var playerScreenPos = _mainCamera.WorldToViewportPoint(position);

            var newPlayerScreenPos = playerScreenPos;

            if (playerScreenPos.x < _screenBoundsThreshold / _mainCamera.aspect) {
                newPlayerScreenPos.x = 1f - (playerScreenPos.x + _teleportThresholdForScreenBounds) / _mainCamera.aspect;
            }
            if (playerScreenPos.x > 1f - _screenBoundsThreshold / _mainCamera.aspect) {
                newPlayerScreenPos.x = (1f - playerScreenPos.x + _teleportThresholdForScreenBounds) / _mainCamera.aspect;
            }

            if (playerScreenPos.y < _screenBoundsThreshold) {
                newPlayerScreenPos.y = 1f - playerScreenPos.y - _teleportThresholdForScreenBounds;
            }
            if (playerScreenPos.y > 1f - _screenBoundsThreshold) {
                newPlayerScreenPos.y = 1f - playerScreenPos.y + _teleportThresholdForScreenBounds;
            }

            return _mainCamera.ViewportToWorldPoint(newPlayerScreenPos);
        }

        // Get random position within the screen
        public Vector3 RandomPositionInsideBounds => new Vector3(
            Random.Range(_leftBottomFieldWorldPos.x, _rightTopFieldWorldPos.x),
            Random.Range(_leftBottomFieldWorldPos.y, _rightTopFieldWorldPos.y),
            0f
        );
    }
}