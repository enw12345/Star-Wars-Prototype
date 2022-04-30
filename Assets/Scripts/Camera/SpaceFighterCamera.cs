using System;
using SpaceFighter;
using UnityEngine;

namespace CameraBehaviour
{
    [RequireComponent((typeof(Camera)))]
    public class SpaceFighterCamera : MonoBehaviour
    {
        [SerializeField] private SpaceFighterBehaviour spaceFighter;
        [SerializeField] private Transform followTarget;
        [SerializeField] private float followDistance = 10f;
        [SerializeField] private float followSpeed = 2f;
        [SerializeField] private float followHeight = 2f;
        [SerializeField] private float accelerationDistance = 1;
        [SerializeField] [Range(0.1f , 0.5f)] private float maxAcceleration = .5f;
        [SerializeField] private float rotationOffset = 50f;

        private Vector3 _rotation = Vector3.zero;

        private float _maxAccelerationDistance;

        private void LateUpdate()
        {
            HandleAcceleration();
            Follow();
            Rotate();
        }

        private void HandleAcceleration()
        {
            _maxAccelerationDistance = spaceFighter.CurrentAcceleration > 1 ? (spaceFighter.CurrentAcceleration * maxAcceleration) : spaceFighter.CurrentAcceleration;

            accelerationDistance = Mathf.MoveTowards(accelerationDistance, _maxAccelerationDistance,
                followSpeed * Time.deltaTime);
        }

        private void Follow()
        {
            var transform1 = followTarget.transform;
            var position = transform1.position;
            var newPosition = new Vector3(position.x, position.y + followHeight, position.z - followDistance * accelerationDistance);
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * followSpeed);
        }

        private void Rotate()
        {
            var newRot = new Vector3(UpdatePitch(), UpdateYaw(), 0);
            var rotLerp = Vector3.Lerp(_rotation, _rotation += newRot, Time.deltaTime);
            transform.eulerAngles = rotLerp;
        }

        private float UpdatePitch()
        {
            var mouseY = Input.GetAxis("Mouse Y");
            if (mouseY == 0) return 0;
            return -(mouseY * 1);
        }
        
        private float UpdateYaw()
        {
            var mouseX = Input.GetAxis("Mouse X");
            if (mouseX == 0) return 0;
            return mouseX * 1;
        }
        
    }
}