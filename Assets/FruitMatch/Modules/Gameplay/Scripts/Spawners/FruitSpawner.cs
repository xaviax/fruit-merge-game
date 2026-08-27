using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;




public class FruitSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Fruit fruitPrefab = null;
    [SerializeField] private FruitCatalog fruitCatalog = null;
    [SerializeField] private Transform fruitRoot = null;
    [SerializeField] private Transform fruitSpawnPoint = null;
    
    
    [Header("Gameplay Variables")]
    [SerializeField] private float spawnDelay = 0.5f;
    
    
    public Fruit CurrentFruit { get; private set; }
    
    public FruitDefinition CurrentDefinition { get; private set; }
    
    public FruitDefinition NextDefinition { get; private set; }
    
    public event Action<FruitDefinition> NextFruitToSpawn;
    
    

    private int totalWeight = 100;
    private static readonly int[] SpawnWeights =
    {
        40, 30, 20,10  
    };
    
    
    
    private void Start()
    {
        InitializeDefinitionQueue();
        SpawnFruit();
        
    }
    

    private void InitializeDefinitionQueue()
    {
        CurrentDefinition = SelectRandomDefinition();
        SelectNextDefinition();
        
        
    }
    
    
    private void AdvanceDefinitionQueue()
    {
        // our next definition becomes the current def

        CurrentDefinition = NextDefinition;
        
        // now we get a new NextDef
        SelectNextDefinition();
        
        
    }

    private void SelectNextDefinition()
    {
        NextDefinition = SelectRandomDefinition();
        NextFruitToSpawn?.Invoke(NextDefinition);
    }
    
    

    private FruitDefinition SelectRandomDefinition()
    {
        
        int randomWeight = Random.Range(0, totalWeight);
        Debug.Log($"Random Weight: {randomWeight}");

        for (int i = 0; i < SpawnWeights.Length; i++)
        {
            if (randomWeight < SpawnWeights[i])
            {
                int selectedTier = i + 1;
                return fruitCatalog.GetByTier(selectedTier);
            }
            
            randomWeight -= SpawnWeights[i];
            
        }
        
        
        return fruitCatalog.GetByTier(1);
       
    }
    

    private void SpawnFruit()
    {

        if (!ValidateSpawnFruit())
        {
            return;
        }
        
        
        CurrentFruit = Instantiate(fruitPrefab, fruitSpawnPoint.position, Quaternion.identity, fruitRoot);
        
        CurrentFruit.Initialize(CurrentDefinition);
        CurrentFruit.PrepareForAiming();
        VFXManager.Instance.ShowHeldFruitGuide(CurrentFruit);
        
    }
    
    
   
    public void DropCurrentFruit()
    {
        if (!CanDropCurrentFruit())
        {
            return;
        }
        
        CurrentFruit.Release();
        CurrentFruit = null;
        VFXManager.Instance.HideHeldFruitGuide();
        AdvanceDefinitionQueue();
        StartCoroutine(SpawnNextFruit());
        
    }

    private bool CanDropCurrentFruit()
    {
        /*if (CurrentFruit == null || !CurrentFruit.IsHeld)
        {
            Debug.LogWarning(
                "FruitSpawner has no current fruit to drop.",
                this
            );

            return false;
        }
        
        return true;*/
        
        
        return GameManager.Instance != null && 
               GameManager.Instance.IsGameplayActive && 
               CurrentFruit != null && 
               CurrentFruit.IsHeld;
        
        
    }
    
    



    private IEnumerator SpawnNextFruit()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnFruit();
        
    }

    
    private bool ValidateSpawnFruit()
    {
        if (CurrentFruit != null)
        {
            Debug.LogWarning(
                "FruitSpawner already has a fruit spawned.",
                this
            );

            return false;
        }

        if (CurrentDefinition == null)
        {
            Debug.LogError(
                "FruitSpawner has no current definition.",
                this
            );

            return false;
        }
        
        return true;


    }



}
