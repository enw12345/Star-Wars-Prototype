using UnityEngine;

namespace Manager
{

    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemy;
        [SerializeField][Range(3, 10)] private int maxEnemyCount = 5;
        [SerializeField][Range(1f, 5f)] private float spawnTime = 3f;
        [SerializeField][Range(100f, 250f)] private float minSpawnRange = 300f;
        [SerializeField][Range(250f, 500f)] private float maxSpawnRange = 300f;
        private float spawnTimer = 0;

        // Update is called once per frame
        void Update()
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnTime){
                SpawnEnemies();
                spawnTimer = 0;
            }
        }

        private void SpawnEnemies()
        {
            var x = Random.Range(minSpawnRange, maxSpawnRange);
            var y = Random.Range(minSpawnRange, maxSpawnRange);
            var z = Random.Range(minSpawnRange, maxSpawnRange);

            var spawnPosition = new Vector3(x, y, z);

            Instantiate(enemy, spawnPosition, Quaternion.identity);
        }
    }

}