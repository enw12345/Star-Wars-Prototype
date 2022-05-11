using UnityEngine;

namespace HealthAndDamage
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _health = 10f;

        public void TakeDamage(float damage)
        {
            _health -= damage;
            Explode();
        }

        private void Explode()
        {

        }
    }

}

