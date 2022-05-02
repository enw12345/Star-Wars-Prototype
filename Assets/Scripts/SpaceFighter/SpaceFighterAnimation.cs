using UnityEngine;

namespace SpaceFighter
{
    public class SpaceFighterAnimation : MonoBehaviour
    {
        [Header("Animation Variables")] [SerializeField]
        private float animationSmoothing = .1f;

        [SerializeField] private float maxAnimationPositionOffset = 20f;
        [SerializeField] private float maxAnimationRotation = 20f;
        [SerializeField] private float minAnimationRotation = -20f;

        private Vector3 _animatedPosition;
        private Quaternion _animatedRotation;
        private float currentSmooth;

        private void LateUpdate()
        {
            Animate();
        }

        private void Animate()
        {
            if (currentSmooth >= 1)
                // GetNewPosition();
                // GetNewRotation();
                currentSmooth = 0;

            currentSmooth += Time.deltaTime * animationSmoothing;

            // transform.position = Vector3.Lerp(transform.position, _animatedPosition, currentSmooth);
            // transform.rotation = Quaternion.Slerp(transform.rotation, _animatedRotation, currentSmooth);
        }

        private void GetNewPosition()
        {
            _animatedPosition = transform.up * (Random.Range(-1, 1) * maxAnimationPositionOffset) +
                                transform.right * (Random.Range(-1, 1) * maxAnimationPositionOffset);
        }

        private void GetNewRotation()
        {
            var newZRotation = Random.insideUnitCircle * Random.Range(minAnimationRotation, maxAnimationRotation);
            _animatedRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, newZRotation.x);
        }
    }
}