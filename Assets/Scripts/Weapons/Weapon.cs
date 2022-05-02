using UnityEngine;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject _projectile;
        [SerializeField] private float _fireRate = 0.1f;
        [SerializeField] private Transform _projectileSpawner;
        [SerializeField] private Light _weaponFlash;
        [SerializeField] private SpaceWeaponSounds _sounds;

        protected bool Shoot(float timeSinceLastFired)
        {
            if (_weaponFlash.enabled) _weaponFlash.enabled = false;

            if (!(timeSinceLastFired >= _fireRate)) return false;
            Instantiate(_projectile, _projectileSpawner.position, _projectileSpawner.rotation);
            _weaponFlash.enabled = true;
            _sounds.PlayWeaponSound();

            return true;
        }
    }
}