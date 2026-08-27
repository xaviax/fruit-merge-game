using UnityEngine;

public class RaycastLineDrawer : MonoBehaviour
{
    // Reference to the LineRenderer component
    public LineRenderer lineRenderer;

    public LayerMask mask;
    // Update is called once per frame
    void Update()
    {
        // Create a ray from the game object's position in the forward direction
        Vector2 rayOrigin = transform.position;
       // Vector2 rayDirection = -transform.up*20; // Assuming you want to cast the ray in the right direction
        Vector2 rayDirection = Vector2.down; // Assuming you want to cast the ray in the right direction
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection,Mathf.Infinity, mask);

        // Check if the ray hits something
        if (hit.collider != null)
        {
           // Debug.Log(hit.collider.gameObject.name, hit.collider.gameObject);
            // If the ray hits, draw a line from the game object's position to the hit point
            //   DrawLine(rayOrigin, hit.transform.localPosition);

            Vector2 rayDirection2 = Vector2.up; // Assuming you want to cast the ray in the right direction
            RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin, rayDirection2, Mathf.Infinity, mask);
            if (hit2.collider != null)
            {

                DrawLine(hit2.point, hit.point);
            }
            else
            {
                DrawLine(rayOrigin, hit.point);
            }
        }
    }

    // Method to draw a line using LineRenderer
    void DrawLine(Vector2 start, Vector2 end)
    {
        // If LineRenderer component is not attached, attach it
        if (lineRenderer == null)
        {
           // lineRenderer = gameObject.AddComponent<LineRenderer>();
            //lineRenderer.startWidth = 0.1f;
            //lineRenderer.endWidth = 0.1f;
            //lineRenderer.material.color = Color.red;
        }

        // Set the line's positions
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
