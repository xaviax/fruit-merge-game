using UnityEngine;

public class FruitPhysicsTester : MonoBehaviour
{
  
    [Header("References")]
    [SerializeField] private Fruit fruitPrefab = null;
    [SerializeField] private FruitDefinition testDefinition = null;
    [SerializeField] private Transform fruitSpawnPoint = null;
    [SerializeField] private Transform fruitRoot = null;
    
    // gpt told me to store fruit? I mean, its already being stored 
    // but I think this is cleaner 
    private Fruit spawnedFruit = null;


    void Awake()
    {
        SpawnTestFruit();
    }

    [ContextMenu("Spawn Test Fruit")]
    private void SpawnTestFruit()
    {
        Debug.Log("Spawning test fruit", this);

        if (!Application.isPlaying)
        {
            Debug.LogWarning("Enter play mode to spawn fruits", this);
            return;
        }

        if (fruitPrefab == null || testDefinition == null || fruitSpawnPoint == null)
        {
            Debug.LogError("Missing references", this);
            return;
        }
        
        spawnedFruit = Instantiate(fruitPrefab, fruitSpawnPoint.position, Quaternion.identity, fruitRoot);
        
        spawnedFruit.Initialize(testDefinition);
        spawnedFruit.PrepareForAiming();
        
        
    }
    
    [ContextMenu("Drop Test Fruit")]
    public void DropTestFruit()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning(
                "Enter Play Mode before dropping the test fruit.",
                this
            );

            return;
        }

        if (spawnedFruit == null)
        {
            Debug.LogWarning(
                "There is no test fruit to drop.",
                this
            );

            return;
        }

        spawnedFruit.Release();
    }
    
    
    
}
