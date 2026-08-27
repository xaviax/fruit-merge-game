using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameplayCameraFitter : MonoBehaviour
{
    [Header("Reference Resolution")]
    [SerializeField] private float referenceWidth = 10.8f;

    [SerializeField] private float referenceHeight = 19.2f;
    
    
    [Header("Bottom Alignment")] 
    [SerializeField] private SpriteRenderer containerRenderer = null;
    
    
    
    
    private Camera gameplayCamera = null;
    
    private void Awake()
    {
        gameplayCamera = GetComponent<Camera>();

        gameplayCamera.orthographic = true;
        UpdateCameraSize();
        AlignContainerWithScreenBottom();
        Debug.LogWarning("CAMERA OPTIMIZED!");

    }

    
    private void UpdateCameraSize()
    {
        float referenceAspect = referenceWidth / referenceHeight;
        
        float currentAspect = (float)Screen.width / Screen.height;

        float referenceOrthographicSize = referenceHeight * 0.5f;

        if (currentAspect < referenceAspect)
        {
            gameplayCamera.orthographicSize = referenceOrthographicSize * (referenceAspect / currentAspect);
            
        }

        else
        {
            gameplayCamera.orthographicSize = referenceOrthographicSize;
        }
        
    }


    private void AlignContainerWithScreenBottom()
    {
        float containerBottomY = containerRenderer.bounds.min.y;
        
        Vector3 cameraPosition = transform.position;

        cameraPosition.y = containerBottomY + gameplayCamera.orthographicSize;
        
        transform.position = cameraPosition;



    }
    
    
}
