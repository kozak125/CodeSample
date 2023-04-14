using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooter.Player.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField]
        private Slider damageBar;

        private Slider healthBar;
        private bool isCoroutineRunning = false;

        private readonly WaitForSeconds waitSecondsToSubstractDamage = new WaitForSeconds(1f);
        private readonly WaitForSeconds damageSubstractionSpeed = new WaitForSeconds(0.1f);
        private const int DAMAGE_SUBSTRACTION_SMOOTHING = 1;

        public void Setup(int maxHealth)
        {
            healthBar = GetComponent<Slider>();
            healthBar.value = healthBar.maxValue = maxHealth;
            damageBar.value = damageBar.maxValue = maxHealth;

            EventBroker.OnGameOver += GameOver;
        }

        public void SubstractHealth(int amount)
        {
            healthBar.value -= amount;
            if (!isCoroutineRunning)
            {
                StartCoroutine(nameof(SmoothSubstractDamage));
            }
        }

        private IEnumerator SmoothSubstractDamage()
        {
            isCoroutineRunning = true;
            yield return waitSecondsToSubstractDamage;
            while (damageBar.value > healthBar.value)
            {
                damageBar.value -= DAMAGE_SUBSTRACTION_SMOOTHING;
                yield return damageSubstractionSpeed;
            }

            isCoroutineRunning = false;
        }

        private void GameOver()
        {
            gameObject.SetActive(false);
            damageBar.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBroker.OnGameOver -= GameOver;
        }
    }
}
