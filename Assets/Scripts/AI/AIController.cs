using SpaceFighter;
using UnityEngine;
using Manager;

namespace AI
{
    public class AIController : SpaceFighterBehaviour
    {
        public static Transform PlayerTransform;
        [SerializeField] private float followDistance = 25;
        [SerializeField] private float _playerRange = 100f;

        private readonly float _speed = 1.5f;
        private readonly float _timeToChangeDestination = 2f;
        private Vector3 _destination;
        private float _timeSinceLastDestinationChange;
        private float roll, pitch, yaw;

        protected override void Awake()
        {
            base.Awake();
            PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
            _destination = PlayerTransform.position - transform.position;
        }

        protected override void Update()
        {
            if (GameManager.GameStart)
            {
                SetForward();
                FindDestination();
                base.Update();
                _timeSinceLastDestinationChange += Time.deltaTime;
            }
        }

        protected override void Rotate()
        {
            var newRotation = Quaternion.Euler(UpdatePitch(), UpdateYaw(), UpdateRoll());
            var step = _speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, step);
        }

        private void SetForward()
        {
            transform.forward = Vector3.MoveTowards(transform.forward, _destination, 1f);
        }

        protected override float UpdatePitch()
        {
            var desiredRotation = -Mathf.Atan2(_destination.y, _destination.z);
            pitch = Mathf.Rad2Deg * desiredRotation;
            return pitch;
        }

        protected override float UpdateRoll()
        {
            var desiredRotation = Mathf.Atan2(_destination.y, _destination.x);
            roll = Mathf.Rad2Deg * desiredRotation;
            return roll;
        }

        protected override float UpdateYaw()
        {
            var desiredRotation = Mathf.Atan2(_destination.x, _destination.z);
            yaw = Mathf.Rad2Deg * desiredRotation;
            return yaw;
        }

        private void FindDestination()
        {
            var destination = _destination;


            if (_timeSinceLastDestinationChange >= _timeToChangeDestination)
            {
                if (destination.magnitude <= _playerRange)
                {
                    var offsetX = Random.Range(-1000, 1000);
                    var offsetY = Random.Range(-1000, 1000);
                    var offsetZ = Random.Range(-1000, 1000);
                    _destination = new Vector3(offsetX, offsetY, offsetZ);
                    _timeSinceLastDestinationChange = 0;
                }

                else
                {
                    _destination = PlayerTransform.position - transform.position;
                }
            }
        }
    }
}