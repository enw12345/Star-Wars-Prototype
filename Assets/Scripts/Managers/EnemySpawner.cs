using HealthAndDamage;
using UnityEngine;

namespace Managers
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemy;
        [SerializeField][Range(3, 10)] private int maxEnemyCount = 5;
        [SerializeField][Range(1f, 5f)] private float spawnTime = 3f;
        [SerializeField][Range(100f, 250f)] private float minSpawnRange = 300f;
        [SerializeField][Range(250f, 500f)] private float maxSpawnRange = 300f;
        private float _spawnTimer;

        private int _enemyCount;

        public static int DestroyedEnemies;

        public int MaxEnemyCount => maxEnemyCount;

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.GameStart) return;
            _spawnTimer += Time.deltaTime;
            
            if (_spawnTimer >= spawnTime && _enemyCount < maxEnemyCount){
                SpawnEnemies();
                _spawnTimer = 0;
            }
        }

        private void SpawnEnemies()
        {
            var x = Random.Range(minSpawnRange, maxSpawnRange);
            var y = Random.Range(minSpawnRange, maxSpawnRange);
            var z = Random.Range(minSpawnRange, maxSpawnRange);

            var spawnPosition = new Vector3(x, y, z);

            var spawnedEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);
            _enemyCount++;

            spawnedEnemy.GetComponent<Health>().Died += EnemyDeath;
        }

        private void EnemyDeath()
        {
            DestroyedEnemies++;
        }
    }
}