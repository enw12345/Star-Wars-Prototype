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
        [SerializeField] protected float turnSpeed = 2f;
        [SerializeField] protected float sensitivity = 1f;
        private float currentSpeed = 0;
        [SerializeField] private float energy = 5f;
        [SerializeField] private float speedBoostMultiplier = 10f;

        public float CurrentAcceleration { get; private set; } = 1f;
        public float Sensitivity => sensitivity;
        public float TurnSpeed => turnSpeed;
        protected virtual void Awake()
        {
            Input.ResetInputAxes();
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected virtual void Update()
        {
            Move();
            Rotate();
        }
        
        private void Move()
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, CurrentAcceleration);
            Vector3 newPosition = transform.forward * (currentSpeed * Time.deltaTime);
            transform.position += newPosition;
        }
        
        protected virtual void Rotate()
        {

        }
        
        protected virtual float UpdatePitch()
        {
            return 0;
        }

        protected virtual float UpdateRoll()
        {
            return 0;
        }

        protected virtual float UpdateYaw()
        {
            return 0;
        }

        protected IEnumerator SpeedBoost()
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