using UnityEngine;

namespace HealthAndDamage
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 10f;
        [SerializeField] private float _currentHealth = 10f;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        public delegate void OnTookDamage();

        public delegate void OnDeath();
        
        public event OnTookDamage TookDamage;

        public event OnDeath Died;
        
        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            TookDamage?.Invoke();

            if (_currentHealth <= 0)
            {
                Explode();
                Death();
            }
        }

        private void Explode()
        {

        }

        private void Death()
        {
            Died?.Invoke();
            Destroy(gameObject);
        }
    }

}

