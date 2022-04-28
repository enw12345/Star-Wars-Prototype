using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceFighter
{
    public class SpaceFighterBehaviour : MonoBehaviour
    {
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float turnSpeed = 2f;
        [SerializeField] private Transform cameraFollowTransform;
        [SerializeField] private float turnConstraintAngle = 45f;
        [SerializeField] private float sensitivity = 100f;
        private float acceleration = 1f;
        private float currentSpeed = 0;
        [SerializeField] private float energy = 5f;
        [SerializeField] private float speedBoostMultiplier = 10f;
        
        public float CurrentAcceleration => acceleration;

        private void Start()
        {
            Input.ResetInputAxes();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Move();
            UpdatePitch();
            UpdateRoll();
            UpdateYaw();

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartCoroutine(SpeedBoost());
            }
        }

        private void Move()
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration);
            Vector3 newPosition = transform.forward * (currentSpeed * Time.deltaTime);
            // transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * currentSpeed);
            transform.position += newPosition;
        }
        

        private void UpdatePitch()
        {
            var mouseY = Input.GetAxis("Mouse Y");
            if (mouseY== 0) return;
            
            var pitch = -mouseY * sensitivity;
            pitch = Mathf.Clamp( pitch, -turnConstraintAngle, turnConstraintAngle);

            var rotation = transform.rotation;
            var newRotation = Quaternion.Euler(pitch, rotation.y, rotation.z);
            rotation = Quaternion.Slerp(rotation, newRotation, Time.deltaTime * turnSpeed);
            transform.rotation = rotation;
        }

        private void UpdateRoll()
        {
            var mouseX = Input.GetAxis("Mouse X");

            if (mouseX == 0) return;
            
            var roll = mouseX * sensitivity;
            roll = -Mathf.Clamp(roll, -turnConstraintAngle, turnConstraintAngle);
            var rotation = transform.rotation;
            var newRotation = Quaternion.Euler(rotation.x, rotation.y, roll);
            rotation = Quaternion.Slerp(rotation, newRotation, Time.deltaTime * turnSpeed);
            transform.rotation = rotation;
        }

        private void UpdateYaw()
        {
            var mouseX = Input.GetAxis("Mouse X");

            if (mouseX == 0) return;
            
            var yaw = -mouseX * sensitivity;
            yaw = -Mathf.Clamp(yaw, -turnConstraintAngle, turnConstraintAngle);
            
            var rotation = transform.rotation;
            var newRotation = Quaternion.Euler(rotation.x, yaw, rotation.z);
            rotation = Quaternion.Slerp(rotation, newRotation, Time.deltaTime * turnSpeed);
            transform.rotation = rotation;
        }

        private IEnumerator SpeedBoost()
        {
            var currentMaxSpeed = maxSpeed;
            var currentMaxEnergy = energy;
            maxSpeed *= speedBoostMultiplier;
            
            while (energy > 0 && Input.GetKey(KeyCode.LeftShift))
            {
                acceleration = 15f;
                energy -= 1;
                yield return new WaitForSeconds(1f);
            }

            maxSpeed = currentMaxSpeed;
            energy = currentMaxEnergy;
            acceleration = 1;
        }



        public Transform GetFollowTransform()
        {
            return cameraFollowTransform;
        }
    }
}