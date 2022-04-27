﻿using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace SpaceFighter
{
    public class SpaceFighterBehaviour : MonoBehaviour
    {
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float turnSpeed = 2f;
        [SerializeField] private Transform cameraFollowTransform;
        [SerializeField] private float turnConstraintAngle = 180f;
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
            TurnVertical();
            TurnHorizontal();

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartCoroutine(SpeedBoost());
            }
        }

        private void Move()
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, Time.deltaTime * acceleration);
            Vector3 newPosition = transform.forward * (currentSpeed * Time.deltaTime);
            // transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * currentSpeed);
            transform.position = newPosition;
        }
        

        private void TurnVertical()
        {
            var mouseX = Input.GetAxis("Mouse X");
            if (mouseX == 0) return;
            
            var rotationY = mouseX * sensitivity;
            rotationY = Mathf.Clamp( rotationY, -turnConstraintAngle, turnConstraintAngle);
            
            var newRotation = Quaternion.Euler(transform.rotation.x, rotationY, transform.rotation.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turnSpeed);
        }

        private void TurnHorizontal()
        {
            var mouseY = Input.GetAxis("Mouse Y");

            if (mouseY == 0) return;
            
            var rotationX = -mouseY * sensitivity;
            rotationX = Mathf.Clamp(rotationX, -turnConstraintAngle, turnConstraintAngle);
            
            var newRotation = Quaternion.Euler(rotationX, transform.rotation.y, transform.rotation.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turnSpeed);
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