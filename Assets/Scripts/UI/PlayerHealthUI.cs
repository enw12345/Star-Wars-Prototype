using UnityEngine;
using HealthAndDamage;
using UnityEngine.UI;

namespace UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        private Image _healthBar;

        private void Awake()
        {
            _healthBar = GetComponent<Image>();
            playerHealth.TookDamage += UpdateHealthBar;
        }
        
        private void UpdateHealthBar()
        {
            _healthBar.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        }
    }
}