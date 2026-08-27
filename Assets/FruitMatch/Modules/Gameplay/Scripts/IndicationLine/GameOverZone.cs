using System;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class GameOverZone : MonoBehaviour
{


    private float requiredOverlapTime = 5f;

    private readonly Dictionary<Fruit, float> trackedFruits = new Dictionary<Fruit, float>();
    
    private readonly HashSet<Fruit> thresholdReached = new HashSet<Fruit>();
    
    private readonly List<Fruit> trackedFruitBuffer = new List<Fruit>();
    
    
    

    private void FixedUpdate()
    {
        trackedFruitBuffer.Clear();
        trackedFruitBuffer.AddRange(trackedFruits.Keys);

        // created a temp list because the dict cannot 
        foreach (Fruit fruit in trackedFruitBuffer)
        {

            if (!IsEligible(fruit))
            {
                StopTrackingFruit(fruit);
                continue;
            }
            
            
            // This fruit has already completed the timer 
            if (thresholdReached.Contains(fruit))
            {
                continue;
            }

            float elapsedTime = trackedFruits[fruit] + Time.fixedDeltaTime;
            trackedFruits[fruit] = elapsedTime;

            if (elapsedTime >= requiredOverlapTime)
            {
                thresholdReached.Add(fruit);

                Debug.LogWarning(
                    $"{fruit.gameObject.name} remained inside " +
                    $"the indication line for {requiredOverlapTime} seconds.",
                    fruit
                );
                
                GameManager.Instance.GameOver();
                return;
                
            }

        }
        
    }

    private void OnDisable()
    {
            trackedFruits.Clear();
            thresholdReached.Clear();
            trackedFruitBuffer.Clear();
    }


    private void TryTrackFruit(Collider2D other)
    {
        if (!other.TryGetComponent(out Fruit fruit))
        {
            return;
        }

        if (!IsEligible(fruit))
        {
            StopTrackingFruit(fruit);
            return;
        }

        if (trackedFruits.ContainsKey(fruit))
        {
            return;
        }
        
        trackedFruits.Add(fruit,0f);
        Debug.LogWarning($"{fruit.gameObject.name} is now being timed",fruit);
        
        
    }

    private bool IsEligible(Fruit fruit)
    {
        return fruit !=null && 
               fruit.gameObject.activeInHierarchy && 
               fruit.IsDropped && 
               !fruit.IsMerging;
    }

    private void StopTrackingFruit(Fruit fruit)
    {
        trackedFruits.Remove(fruit);
        thresholdReached.Remove(fruit);
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryTrackFruit(other);
        
    }

    
    private void OnTriggerStay2D(Collider2D other)
    {
        TryTrackFruit(other);
    }
    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Fruit fruit))
        {
            return;
        }

        if (!trackedFruits.ContainsKey(fruit))
        {
            return;
        }

        Debug.LogWarning(
            $"{fruit.gameObject.name} left the indication line. " +
            "Its timer was reset.",
            fruit
        );
        
        StopTrackingFruit(fruit);
        
    }
}
