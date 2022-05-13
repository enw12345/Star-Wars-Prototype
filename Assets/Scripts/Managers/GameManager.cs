using HealthAndDamage;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Canvas instructionsCanvas;
        [SerializeField] private Canvas gameOverCanvas;

        [SerializeField] private TMP_Text playerScoreText;
        [SerializeField] private EnemySpawner es;
        private static bool _gameStart;
        public static bool GameStart => _gameStart;

        [SerializeField] private Health playerHealth;
        // Start is called before the first frame update
        void Start()
        {
            instructionsCanvas.gameObject.SetActive(true);
            gameOverCanvas.gameObject.SetActive(false);
            playerHealth.Died += PlayerDeath;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.anyKey && _gameStart == false)
            {
                _gameStart = true;
                instructionsCanvas.gameObject.SetActive(false);
            }

            if (EnemySpawner.DestroyedEnemies == es.MaxEnemyCount)
            {
                GameOver();
            }
        }

        private void PlayerDeath()
        {
            GameOver();
        }

        public void GameOver()
        {
            gameOverCanvas.gameObject.SetActive(true);
            playerScoreText.text = $"Your score is: {EnemySpawner.DestroyedEnemies}";
        }
    }
}