using UnityEngine;

namespace SpaceFighter
{
    public class SpaceFighterSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _flightMedium;
        [SerializeField] private AudioSource _flightHigh;

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