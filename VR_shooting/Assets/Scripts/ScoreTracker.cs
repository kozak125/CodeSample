using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    private TMP_Text scoreText;
    private int score = 0;

    private void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        UpdateScoreText();
        EventBroker.OnEnemyDestroyed += AddScore;
    }

    private void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    private void OnDisable()
    {
        EventBroker.OnEnemyDestroyed -= AddScore;
    }
}
