using SpaceFighter;
using UnityEngine;

namespace AI
{
    public class AIController : SpaceFighterBehaviour
    {
        public static Transform PlayerTransform;
        [SerializeField] private float followDistance = 25;
        [SerializeField] private float _playerRange = 100f;
        private Vector3 Destination;
        private float timeSinceLastDestinationChange;
        private readonly float timeToChangeDestination = 2f;

        private float _speed = 1.5f;
        protected override void Awake()
        {
            base.Awake();
            PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }

        protected override void Update()
        {
            FindDestination();
            base.Update();
            timeSinceLastDestinationChange += Time.deltaTime;
        }

        protected override void Rotate()
        {
            var newRotation = Quaternion.Euler(UpdatePitch(), UpdateYaw(), UpdateRoll());
            var step = _speed * Time.deltaTime;
            
            transform.forward = Vector3.Lerp(transform.forward, Destination, step);
            // transform.forward = Destination;
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, step);
        }

        protected override float UpdatePitch()
        {
            var desiredRotation = -Mathf.Atan2(Destination.y, Destination.z);
            return Mathf.Rad2Deg * desiredRotation;
        }

        protected override float UpdateRoll()
        {
            var desiredRotation = Mathf.Atan2(Destination.y, Destination.x);
            return Mathf.Rad2Deg * desiredRotation;
        }

        protected override float UpdateYaw()
        {
            var desiredRotation = Mathf.Atan2(Destination.x, Destination.z);
            return Mathf.Rad2Deg * desiredRotation;
        }

        private void FindDestination()
        {
            var destination = PlayerTransform.position - transform.position;
            
            if (destination.magnitude < _playerRange)
            {
                if (timeSinceLastDestinationChange >= timeToChangeDestination)
                {
                    var offsetX = Random.Range(-1000, 1000);
                    var offsetY = Random.Range(-1000, 1000);
                    var offsetZ = Random.Range(-1000, 1000);
                    Destination = new Vector3(offsetX, offsetY, offsetZ);
                    timeSinceLastDestinationChange = 0;
                }
            }
            else
                Destination = new Vector3(destination.x, destination.y, destination.z);
        }
    }
}