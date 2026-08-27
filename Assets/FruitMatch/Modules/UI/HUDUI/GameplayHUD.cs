
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameplayHUD : MonoBehaviour
{
    
    [SerializeField] private TMP_Text scoreText = null;
    [SerializeField] private Image nextFruitSprite = null;
    
    //private ScoreManager _scoreManager;

    [SerializeField] private FruitSpawner fruitSpawner = null;


    private void Start()
    {
        ScoreManager.Instance.ScoreChanged += UpdateScoreText;
        fruitSpawner.NextFruitToSpawn += UpdateNextFruitImage;

    }

    

    private void UpdateScoreText(int score)
    {
        Debug.Log("Score changed");
        scoreText.text = score.ToString();
    }

    public void OnClickSettings()
    {
        UIManager.Instance.ShowSettings();
    }

    private void UpdateNextFruitImage(FruitDefinition fruitDefinition)
    {
        nextFruitSprite.sprite = fruitDefinition.Sprite;
        nextFruitSprite.preserveAspect = true;
        nextFruitSprite.enabled = true;
        
    }
    
}
