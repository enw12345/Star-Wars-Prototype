using SpaceFighter;
using UnityEngine;

namespace PlayerController
{
    public class PlayerController : SpaceFighterBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] [Range(0f, 1)] private float maxViewportClampY = 0.6f;
        [SerializeField] [Range(0f, 1f)] private float minViewportClampY = 0.4f;
        [SerializeField] [Range(0f, 1)] private float maxViewportClampX = 0.6f;
         [SerializeField] [Range(0f, 1f)] private float minViewportClampX = 0.4f;
         private Vector3 _rotation = Vector3.zero;

         protected override void Awake()
         {
             base.Awake();
             transform.eulerAngles = _rotation;
         }

         protected override void Update()
        {
            ClampToViewPort();
            base.Update();
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(SpeedBoost());
            }
        }

        private void ClampToViewPort()
        {
            Vector3 pos = _camera.WorldToViewportPoint (transform.position);
            pos.x = Mathf.Clamp(pos.x, minViewportClampX, maxViewportClampX);
            pos.y = Mathf.Clamp(pos.y, minViewportClampY, maxViewportClampY);
            transform.position = Vector3.Lerp(transform.position, _camera.ViewportToWorldPoint(pos), Time.deltaTime);
        }

        protected override void Rotate()
        {
            var addedRotation = new Vector3(UpdatePitch(), UpdateYaw(), UpdateRoll());
            _rotation += addedRotation;
            transform.eulerAngles = _rotation;
        }
        
        protected override float UpdatePitch()
        {
            var mouseY = Input.GetAxis("Mouse Y");
            if (mouseY == 0) return 0;
            return -(mouseY * sensitivity);
        }
        
        protected override float UpdateYaw()
        {
            var mouseX = Input.GetAxis("Mouse X");
            if (mouseX == 0) return 0;
            return mouseX * sensitivity;
        }
        
        protected override float UpdateRoll()
        {
            var roll = 0f;
            if (Input.GetKey(KeyCode.A))
            {
                roll = 1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                roll = -1;
            }

            return roll;
        }
        
    }
}