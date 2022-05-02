using UnityEngine;

namespace Weapons
{
    public class SpaceWeaponSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _weaponSound;
        
        public void PlayWeaponSound()
        {
            _weaponSound.pitch = Random.Range(0.65f, 1f);
            _weaponSound.Play();
        }
    }
}