using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    
    public static ScoreManager Instance {get; private set;}

    public event Action<int> ScoreChanged;
    
    [SerializeField] [Min(1)] private int pointsPerTier = 10; 
    public int CurrentScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentScore = 0;

    }

    public void AwardMergeScore(int mergedTier)
    {
        if (mergedTier < 1)
        {
            Debug.LogWarning("You are not allowed to merge merged tiers.", this);
            return;
        }
        
        int awardedScore = mergedTier * pointsPerTier;
        AddScore(awardedScore);
    }

    private void AddScore(int score)
    {
        CurrentScore += score;
        ScoreChanged?.Invoke(CurrentScore);
        Debug.Log($"Score: {CurrentScore} - {score}");
    }

    private void OnDestroy()
    {
        Instance = null;
    }


}
