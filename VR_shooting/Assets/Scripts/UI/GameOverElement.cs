using UnityEngine;
using TMPro;

namespace VRShooter.UI
{
    public class GameOverElement : MonoBehaviour
    {
        private void Awake()
        {
            EventBroker.OnGameOver += GameOver;
        }

        private void OnDestroy()
        {
            EventBroker.OnGameOver -= GameOver;
        }

        private void GameOver()
        {
            GetComponent<TMP_Text>().enabled = true;
        }
    }
}
