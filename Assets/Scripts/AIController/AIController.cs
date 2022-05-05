using SpaceFighter;
using UnityEngine;

namespace AIController
{
    public class AIController : SpaceFighterBehaviour
    {
        public static Transform PlayerTransform;
        [SerializeField] private float followDistance = 25;
        private Vector3 Destination;

        protected override void Awake()
        {
            base.Awake();
            PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }

        protected override void Update()
        {
            FindDestination();
            base.Update();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, Destination);
        }

        protected override void Rotate()
        {
            var newRotationEuler = new Vector3(UpdatePitch(), UpdateYaw(), UpdateRoll());
            var newRotation = Quaternion.Euler(newRotationEuler);
            
            transform.rotation = Quaternion.LookRotation(Destination, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * 500);
        }

        protected override float UpdatePitch()
        {
            var desiredRotation = -Mathf.Atan2(Destination.y, transform.position.z);
            return Mathf.Rad2Deg * desiredRotation;
        }

        protected override float UpdateRoll()
        {
            return base.UpdateRoll();
        }

        protected override float UpdateYaw()
        {
            var desiredRotation = Mathf.Atan2(Destination.x, transform.position.z);
            return Mathf.Rad2Deg * desiredRotation;
        }

        private void FindDestination()
        {
            var destination = PlayerTransform.position - transform.position;
            Destination = destination;
            // Destination = new Vector3(destination.x, destination.y, destination.z - followDistance);
        }
        
    }
}