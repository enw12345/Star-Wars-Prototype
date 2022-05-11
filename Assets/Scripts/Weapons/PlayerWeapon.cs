using UnityEngine;
using Manager;

namespace Weapons
{
    public class PlayerWeapon : Weapon
    {
        private float _timeSinceLastFired;

        private void Update()
        {
            if (GameManager.GameStart)
            {
                _timeSinceLastFired += Time.deltaTime;

                if (!Input.GetMouseButton(0)) return;

                if (Shoot(_timeSinceLastFired))
                    _timeSinceLastFired = 0;
            }

        }
    }
}