using UnityEngine;

public class HeldFruitGuide : MonoBehaviour
{
   
    [Header("References")]
    [SerializeField] private LineRenderer descentLine = null;

    [SerializeField] private ParticleSystem yellowSparkle = null;
    
    [Header("Collision Detection")]
    [SerializeField] private LayerMask landingMask;

    [SerializeField] [Min(0.1f)] private float maximumCastDistance = 20f; 
    
    private Fruit targetFruit = null;

    private void Awake()
    {
        descentLine.positionCount = 2;
        descentLine.useWorldSpace = true;
        descentLine.enabled = false;
        
        yellowSparkle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
    }

    private void LateUpdate()
    {

        if (targetFruit == null || !targetFruit.IsHeld)
        {
            Hide();
            return;
        }

        UpdateFruitGuide();

    }


    public void Show(Fruit fruit)
    {
        if (fruit == null)
        {
            return;
        }
        
        Debug.Log($"Showing guide for {fruit}");
        // set target fruit as the current instantiated fruit 
        targetFruit = fruit;
        
        //enable the descent line 
        descentLine.enabled = true;
        
        // set the position for yellow sparkle 
        yellowSparkle.transform.position = targetFruit.transform.position;
        
        yellowSparkle.Play(true);
        
    }


    public void Hide()
    {
        targetFruit = null;
        descentLine.enabled = false;
        yellowSparkle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }


    private void UpdateFruitGuide()
    {
        Vector2 fruitPosition = targetFruit.transform.position;
        float fruitRadius = targetFruit.ColliderRadius;
        
        RaycastHit2D hit = Physics2D.CircleCast(fruitPosition, fruitRadius, Vector2.down, maximumCastDistance, landingMask);
        
        yellowSparkle.transform.position = targetFruit.transform.position;

        if (hit.collider == null)
        {
            descentLine.enabled = false;
            return;
        }
        
        descentLine.enabled = true;

        Vector3 lineStart = new Vector3(
            fruitPosition.x,
            fruitPosition.y - fruitRadius,
            targetFruit.transform.position.z);

        Vector3 lineEnd = new Vector3(
            hit.centroid.x,
            hit.centroid.y,
            targetFruit.transform.position.z);
        
        descentLine.SetPosition(0, lineStart);
        descentLine.SetPosition(1, lineEnd);



    }
}
