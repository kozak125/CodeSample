using UnityEngine;
using TMPro;

namespace VRShooter.UI
{
    public class GameOverElement : MonoBehaviour
    {
        void Awake()
        {
            EventBroker.OnGameOver += GameOver;
        }

        private void GameOver()
        {
            GetComponent<TMP_Text>().enabled = true;
        }

        private void OnDestroy()
        {
            EventBroker.OnGameOver -= GameOver;
        }
    }
}
