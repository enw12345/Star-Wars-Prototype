using System;
using UnityEngine;

namespace Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 100f;
        [SerializeField] private float _damage = 15f;
        
        private void Update()
        {
            transform.position += transform.forward * _speed;
        }
    }
}