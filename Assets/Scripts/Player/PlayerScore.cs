using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Class, responsible for counting player score
public class PlayerScore : MonoBehaviour
{
    public static UnityEvent<int> ScoreGainEvent = new UnityEvent<int>(); //called from Asteroid and UFO classes to increase the score
    public int Score { get; private set; }

    public void ResetToStart()
    {
        Score = 0;
    }

    private void Start()
    {
        ScoreGainEvent.AddListener(GainScore);
    }

    void GainScore(int _score)
    {
        Score += _score;
    }
}
