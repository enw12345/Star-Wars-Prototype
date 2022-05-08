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
        [SerializeField] private float fullEnergy = 5f;
        [SerializeField] private float speedBoostMultiplier = 10f;
        protected float currentEnergy;

        public float CurrentAcceleration { get; private set; } = 1f;
        public float Sensitivity => sensitivity;
        public float TurnSpeed => turnSpeed;
        protected bool EnergyIsFull => currentEnergy >= fullEnergy;
        protected virtual void Awake()
        {
            Input.ResetInputAxes();
            Cursor.lockState = CursorLockMode.Locked;
            currentEnergy = fullEnergy;
        }

        protected virtual void Update()
        {
            Rotate();
            Move();
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
            maxSpeed *= speedBoostMultiplier;
            
            while (currentEnergy > 0 && Input.GetKey(KeyCode.W))
            {
                CurrentAcceleration = maxSpeed;
                currentEnergy -= 1;
                yield return new WaitForSeconds(1f);
            }

            maxSpeed = currentMaxSpeed;
            currentEnergy = fullEnergy;
            CurrentAcceleration = 1;
        }
    }
}