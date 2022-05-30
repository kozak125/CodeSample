using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField]
    private FloatValue updateScoreInSeconds;
    
    private TMP_Text scoreText;
    private int score;
    private float timeToUpdateText;
    private bool isCoroutineRunning = false;
    // if coroutine is running when enemy destroyed add value

    private void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        UpdateScoreText();
        EventBroker.OnEnemyDestroyed += AddScore;
    }

    private void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        TryStartUpdatingScoreText();
    }

    private void TryStartUpdatingScoreText()
    {
        if (!isCoroutineRunning)
        {
            timeToUpdateText = updateScoreInSeconds.Value;
            StartCoroutine(nameof(UpdateScoreText));
            return;
        }

        timeToUpdateText += updateScoreInSeconds.Value;
    }

    private IEnumerator UpdateScoreText()
    {
        isCoroutineRunning = true;
        while (timeToUpdateText >= 0)
        {
            timeToUpdateText -= Time.deltaTime;
            yield return null;
        }

        scoreText.text = score.ToString();
        isCoroutineRunning = false;
    }

    private void OnDisable()
    {
        EventBroker.OnEnemyDestroyed -= AddScore;
    }
}
