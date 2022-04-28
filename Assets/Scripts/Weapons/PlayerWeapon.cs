using UnityEngine;

namespace Weapons
{
    public class PlayerWeapon : Weapon
    {
        private float _timeSinceLastFired = 0;

        private void Update()
        {
            _timeSinceLastFired += Time.deltaTime;

            if (!Input.GetMouseButton(0)) return;
            
            if (Shoot(_timeSinceLastFired))
                _timeSinceLastFired = 0;
        }
    }
}