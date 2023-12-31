using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [ReadOnly]
    public int Score = 0;
    [ReadOnly]
    public int Combo = 0;
    [ReadOnly]
    public int Kills = 0;

    public float ComboTimeWindow = 3.0f;

    [ReadOnly]
    public bool trackingCombo = false;

    private float trackingTimer = 0.0f;

    private bool comboEnabled = true;

    public event Action<int> OnComboEnded;

    public event Action<int> OnScoreAdded;

    public event Action<int> OnComboAdded;

    public event Action<int> OnKillAdded;

    public int lastGameScore = 0;

    public void AddCombo()
    {
        trackingCombo = true;
        trackingTimer = 0.0f;
        Combo++;
        OnComboAdded?.Invoke(1);
    }

    public void AddScore(int amount = 1)
    {
        Score += amount;
        OnScoreAdded?.Invoke(amount);
    }

    public void AddKill()
    {
        Kills++;
        OnKillAdded?.Invoke(1);
        if (comboEnabled)
            AddCombo();
    }

    public float GetComboProgress()
    {
        if (!trackingCombo)
            return 0.0f;
        return 1 - (trackingTimer / ComboTimeWindow);
    }

    void Awake()
    {
        if (GameManager.Instance.Scores != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.Scores = this;
        Reset();
    }

    protected void Update()
    {
        if (trackingCombo)
        {
            trackingTimer += Time.deltaTime;
            if (trackingTimer >= ComboTimeWindow)
            {
                trackingCombo = false;
                OnComboEnded?.Invoke(Combo);

                const int ComboScale = 10;

                AddScore(Combo * ComboScale);
                Combo = 0;
            }
        }
    }

    public void Reset()
    {
        OnComboEnded = null;
        OnScoreAdded = null;
        OnComboAdded = null;
        OnKillAdded = null;
        Score = 0;
        Combo = 0;
        Kills = 0;
    }
}
