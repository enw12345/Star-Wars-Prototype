using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Canvas instructionsCanvas;
        private static bool gameStart;
        public static bool GameStart => gameStart;

        // Start is called before the first frame update
        void Start()
        {
            instructionsCanvas.gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.anyKey)
            {
                gameStart = true;
                instructionsCanvas.gameObject.SetActive(false);
            }

        }
    }
}