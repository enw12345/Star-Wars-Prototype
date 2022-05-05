using System;
using SpaceFighter;
using Unity.VisualScripting;
using UnityEngine;

namespace AIController
{
    public class AIController : SpaceFighterBehaviour
    {

        // public static SpaceFighterBehaviour Player;
        public static Transform PlayerTransform;
        private Vector3 Destination;
        [SerializeField] private float followDistance = 25;
        [SerializeField] private float rotationSpeed = 25f;
        [SerializeField] private float rotationThreshold = .15f;
        protected override void Awake()
        {
            base.Awake();
            // Player = GameObject.FindWithTag("Player").GetComponent<SpaceFighterBehaviour>();
            PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }

        protected override void Update()
        {
            FindDestination();
            base.Update();
        }

        protected override float UpdatePitch()
        {
            var desiredRotation = Mathf.Atan2(Destination.y , transform.position.z);
            if (transform.rotation.x > desiredRotation + rotationThreshold) {return rotationSpeed;}
            if (transform.rotation.x < desiredRotation - rotationThreshold) {return -rotationSpeed;}

            return 0;
        }

        protected override float UpdateRoll()
        {
            return base.UpdateRoll();
        }

        protected override float UpdateYaw()
        {
            var desiredRotation = Mathf.Atan2(Destination.x , transform.position.z);
            
            if (transform.rotation.y > desiredRotation + rotationThreshold) return -rotationSpeed;
            if (transform.rotation.y < desiredRotation - rotationThreshold) return rotationSpeed;
            return 0;
        }

        private void FindDestination()
        {
            Destination = PlayerTransform.position - transform.position;
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, Destination);
        }
    }
}