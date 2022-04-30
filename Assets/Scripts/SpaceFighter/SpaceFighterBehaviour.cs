using System;
using System.Collections;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceFighter
{
    public class SpaceFighterBehaviour : MonoBehaviour
    {
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float turnSpeed = 2f;
        [SerializeField] private float turnConstraintAngle = 45f;
        [SerializeField] private float sensitivity = 100f;
        private float currentSpeed = 0;
        [SerializeField] private float energy = 5f;
        [SerializeField] private float speedBoostMultiplier = 10f;
        private Vector3 rotation = Vector3.zero;
        [SerializeField] [Range(0.5f, 1)] private float maxViewportClamp = 0.6f;
        [SerializeField] [Range(0.1f, 0.5f)] private float minViewportClamp = 0.4f;
        public float CurrentAcceleration { get; private set; } = 1f;
        [SerializeField] private Camera _camera;
        public float Sensitivity => sensitivity;
        public float TurnConstraintAngle => turnConstraintAngle;
        public float TurnSpeed => turnSpeed;
        private void Awake()
        {
            Input.ResetInputAxes();
            Cursor.lockState = CursorLockMode.Locked;
            transform.eulerAngles = rotation;
        }

        private void Update()
        {
            ClampToViewPort();
            Move();
            Rotate();
            
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(SpeedBoost());
            }
        }

        private void ClampToViewPort()
        {
            Vector3 pos = _camera.WorldToViewportPoint (transform.position);
            pos.x = Mathf.Clamp(pos.x, minViewportClamp, maxViewportClamp);
            pos.y = Mathf.Clamp(pos.y, minViewportClamp, maxViewportClamp);
            // transform.position = _camera.ViewportToWorldPoint(pos);
            transform.position = Vector3.Lerp(transform.position, _camera.ViewportToWorldPoint(pos), Time.deltaTime);
        }
        

        private void Move()
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, CurrentAcceleration);
            Vector3 newPosition = transform.forward * (currentSpeed * Time.deltaTime);
            transform.position += newPosition;
        }
        
        private void Rotate()
        {
            var newRot = new Vector3(UpdatePitch(), UpdateYaw(), 0);
            rotation += newRot;
            transform.eulerAngles = rotation;
        }
        
        private float UpdatePitch()
        {
            var mouseY = Input.GetAxis("Mouse Y");
            if (mouseY == 0) return 0;
            return -(mouseY * sensitivity);
        }

        // private void UpdateRoll()
        // {
        //     var mouseX = Input.GetAxis("Mouse X");
        //
        //     if (mouseX == 0) return;
        //     
        //     var roll = -mouseX * sensitivity;
        //
        //     var rotation = transform.rotation;
        //     var newRotation = Quaternion.Euler(rotation.x, rotation.y, roll);
        //     transform.rotation = Quaternion.Slerp(rotation, newRotation, Time.deltaTime * turnSpeed);
        // }

        private float UpdateYaw()
        {
            var mouseX = Input.GetAxis("Mouse X");
            if (mouseX == 0) return 0;
            return mouseX * sensitivity;
        }

        private IEnumerator SpeedBoost()
        {
            var currentMaxSpeed = maxSpeed;
            var currentMaxEnergy = energy;
            maxSpeed *= speedBoostMultiplier;
            
            while (energy > 0 && Input.GetKey(KeyCode.W))
            {
                CurrentAcceleration = maxSpeed;
                energy -= 1;
                yield return new WaitForSeconds(1f);
            }

            maxSpeed = currentMaxSpeed;
            energy = currentMaxEnergy;
            CurrentAcceleration = 1;
        }
    }
}