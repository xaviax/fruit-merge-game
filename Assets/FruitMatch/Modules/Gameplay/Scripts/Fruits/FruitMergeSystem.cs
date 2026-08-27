using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class FruitMergeSystem : MonoBehaviour
{
  
    public static FruitMergeSystem Instance { get; private set; }
    
    [Header("References")]
    [SerializeField] private FruitCatalog fruitCatalog = null;
    [SerializeField] private Fruit fruitPrefab = null;
    [SerializeField] private Transform fruitRoot = null;


    void Awake()
    {
        Instance = this;
    }

    public void TryMerge(Fruit firstFruit, Fruit secondFruit)
    {
        FruitDefinition  currentFruitDefinition = firstFruit.Definition; 
        
        int currentFruitTier = firstFruit.Definition.Tier;
        
        Vector3 mergePosition = (
            (firstFruit.transform.position + secondFruit.transform.position) * 0.5f
        );

        bool hasNextTier = fruitCatalog.TryGetNext(currentFruitDefinition, out FruitDefinition nextFruitDefinition);
            
        
        
        DestroyFruit(firstFruit, secondFruit);

        if (!hasNextTier)
        {
            return;
        }
        
        
        SpawnNextTier(mergePosition, nextFruitDefinition);
        
        Debug.Log($"Merged {currentFruitTier} to {nextFruitDefinition.Tier}");
        
        ScoreManager.Instance.AwardMergeScore(currentFruitTier);
        
    }

    private void DestroyFruit(Fruit firstFruit, Fruit secondFruit)
    {
        firstFruit.gameObject.SetActive(false);
        secondFruit.gameObject.SetActive(false);
        Destroy(firstFruit.gameObject);
        Destroy(secondFruit.gameObject);
        
        SoundManager.Instance.PlayFruitExplodeSound();
    }

    private void SpawnNextTier(Vector3 mergePosition, FruitDefinition nextFruitDefinition)
    {
        
        
        Fruit mergedFruit = Instantiate(fruitPrefab, mergePosition,Quaternion.identity, fruitRoot);
        
        mergedFruit.Initialize(nextFruitDefinition);
        mergedFruit.PrepareAsMergedFruit();
        
        VFXManager.Instance.PlayImpactVFX(mergePosition);
        SoundManager.Instance.PlayFruitMergeSound();
        
    }
    
    



}
