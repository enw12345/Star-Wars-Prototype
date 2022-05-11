using HealthAndDamage;
using UnityEngine;

namespace Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 100f;
        [SerializeField] private float _damage = 15f;
        [SerializeField] private float _lifeTime = 2f;
        
        private float _timeAlive;
        
        private void Update()
        {
            transform.position += transform.forward * _speed;
            _timeAlive += Time.deltaTime;
            if (_timeAlive >= _lifeTime)
            {
                Destroy(transform.gameObject);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent(out IDamageable damageable);
            if(damageable != null)
                    damageable.TakeDamage(_damage);

            //Play particle effect
            Destroy(transform.gameObject);
        }
    }
}