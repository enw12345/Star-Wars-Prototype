using System;
using SpaceFighter;
using UnityEngine;

namespace Camera
{
    public class SpaceFighterCamera : MonoBehaviour
    {
        [SerializeField] private SpaceFighterBehaviour spaceFighter;
        private Transform followTransform;
        [SerializeField] private float followDistance = 10f;
        [SerializeField] private float followHeight = 2f;
        [SerializeField] private float followSpeed = 2f;
        [SerializeField] private float accelerationDistance = 1;
        [SerializeField] private float maxAcceleration = .5f;
        private float _maxAccelerationDistance;
        
        private void Awake()
        {
            followTransform = spaceFighter.GetFollowTransform(); ;
        }

        private void LateUpdate()
        {
            //Increase FOV with acceleration???
            
            if (spaceFighter.CurrentAcceleration > 1)
                _maxAccelerationDistance = spaceFighter.CurrentAcceleration * .15f;
            else
                _maxAccelerationDistance = spaceFighter.CurrentAcceleration;
            
            accelerationDistance = Mathf.MoveTowards(accelerationDistance, _maxAccelerationDistance, 
                    spaceFighter.CurrentAcceleration * Time.deltaTime);
            
            Vector3 newPosition = new Vector3(followTransform.position.x, followTransform.position.y + followHeight,
                followTransform.position.z - followDistance * accelerationDistance);
            
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * followSpeed);
        }
    }
}
