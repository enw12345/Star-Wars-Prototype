using UnityEngine;

namespace SpaceFighter
{
    public class SpaceFighterSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _weaponSound;
        [SerializeField] private AudioSource _flightMedium;
        [SerializeField] private AudioSource _flightHigh;

        public void PlayWeaponSound()
        {
            _weaponSound.pitch = Random.Range(0.65f, 1f);
            _weaponSound.Play();
        }

        public void PlayFlightMedium()
        {
            _flightMedium.Play();
        }

        public void PlayFlightHigh()
        {
            _flightHigh.Play();
        }
    }
}