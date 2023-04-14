using System.Collections;
using UnityEngine;
using TMPro;

namespace VRShooter.UI
{
    public class SingleEnemyScoreTracker : MonoBehaviour
    {
        [SerializeField]
        private FloatValue showScoreForSeconds;

        private TMP_Text singleEnemyScoreText;
        private int enemyScore;
        private bool isShowingScore = false;
        private float showScoreTimer;

        private void Start()
        {
            singleEnemyScoreText = GetComponent<TMP_Text>();
            singleEnemyScoreText.text = "";
            EventBroker.OnEnemyDestroyed += ShowEnemyScore;
        }

        private void OnDisable()
        {
            EventBroker.OnEnemyDestroyed -= ShowEnemyScore;
        }

        private void ShowEnemyScore(int scoreToShow)
        {
            if (!isShowingScore)
            {
                enemyScore = scoreToShow;
                UpdateEnemyScoreText(enemyScore);
                StartCoroutine(ShowEnemyScoreForSeconds());
                return;
            }

            enemyScore += scoreToShow;
            UpdateEnemyScoreText(enemyScore);
            showScoreTimer += showScoreForSeconds.Value;
        }

        private void UpdateEnemyScoreText(int scoreToShow)
        {
            singleEnemyScoreText.text = string.Concat("+", scoreToShow.ToString());
            isShowingScore = true;
        }

        private IEnumerator ShowEnemyScoreForSeconds()
        {
            showScoreTimer = showScoreForSeconds.Value;
            while (showScoreTimer >= 0)
            {
                showScoreTimer -= Time.deltaTime;
                yield return null;
            }

            HideScore();
        }

        private void HideScore()
        {
            enemyScore = 0;
            singleEnemyScoreText.text = "";
            isShowingScore = false;
        }
    }
}
