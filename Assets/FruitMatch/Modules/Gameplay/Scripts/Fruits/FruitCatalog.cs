using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "FruitCatalog",
    menuName = "Fruit Merge /Fruit Catalog")]
public sealed class FruitCatalog : ScriptableObject
{

    [SerializeField] private List<FruitDefinition> fruitsByTier = new List<FruitDefinition>();
    
    
    public int FruitCount => fruitsByTier.Count;
    
    private int currentFruitIndex = 0;
    private int nextFruitIndex = 0;

    public FruitDefinition GetByTier(int tier)
    {
        int index = tier - 1;

        if (index < 0 || index >= fruitsByTier.Count)
        {
           Debug.LogWarning($"Fruit Index tier out of bounds {tier}", this);
           return null;
        }
        
        return fruitsByTier[index];
        
    }

    public bool TryGetNext(FruitDefinition currentFruit, out FruitDefinition nextFruit)
    {
        nextFruit = null;

        if (currentFruit == null)
        {
            return false;
        }
        
        Debug.Log($"We are in Try Get Next, current fruit is {currentFruit}, next fruit is {nextFruit}");

         currentFruitIndex = fruitsByTier.IndexOf(currentFruit);
         nextFruitIndex = currentFruitIndex + 1;

        if (currentFruitIndex < 0 || nextFruitIndex >= fruitsByTier.Count)
        {
            return false;
        }
        
        nextFruit = fruitsByTier[nextFruitIndex];
        return true;
    }
    

}
