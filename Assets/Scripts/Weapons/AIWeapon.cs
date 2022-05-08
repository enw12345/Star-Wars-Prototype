using UnityEngine;
using AI;

namespace Weapons
{
    public class AIWeapon : Weapon
    {
        private float _timeSinceLastFired;
        private static Transform playerTransform;
        [SerializeField] private float shootDistance = 50f;
        [SerializeField] [Range(0.1f, 1f)] private float accuracy = .95f;
        private void Start()
        {
            Init();
        }

        private void Update()
        {
            _timeSinceLastFired += Time.deltaTime;

            if (!CanShoot()) return;
            
            if (Shoot(_timeSinceLastFired))
                _timeSinceLastFired = 0;
        }

        private bool CanShoot()
        {
            var playerPosition = playerTransform.position;
            var position = transform.position;
            
            var playerDirection = playerPosition - position;
            var playerDistance = Vector3.Distance(playerPosition, position);

            var playerInRange = playerDistance <= shootDistance;
            var playerInSights = Vector3.Dot(transform.forward.normalized, playerDirection.normalized) >= accuracy;
            
            return playerInRange && playerInSights;
        }

        private static void Init()
        {
            playerTransform = AIController.PlayerTransform;
        } 
    }
}