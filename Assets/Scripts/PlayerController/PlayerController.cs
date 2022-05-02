using SpaceFighter;
using UnityEngine;

namespace PlayerController
{
    public class PlayerController : SpaceFighterBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] [Range(0.5f, 1)] private float maxViewportClamp = 0.6f;
        [SerializeField] [Range(0.1f, 0.5f)] private float minViewportClamp = 0.4f;
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
            pos.x = Mathf.Clamp(pos.x, minViewportClamp, maxViewportClamp);
            pos.y = Mathf.Clamp(pos.y, minViewportClamp, maxViewportClamp);

            transform.position = Vector3.Lerp(transform.position, _camera.ViewportToWorldPoint(pos), Time.deltaTime);
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