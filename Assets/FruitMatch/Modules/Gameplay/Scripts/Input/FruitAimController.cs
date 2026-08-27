
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using InputTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using InputTouchPhase = UnityEngine.InputSystem.TouchPhase;
public class FruitAimController : MonoBehaviour
{
   [Header("References")]
   [SerializeField] private FruitSpawner fruitSpawner = null;
   [SerializeField] private Camera gameplayCamera = null;
   [SerializeField] private Transform leftSpawnLimit = null;
   [SerializeField] private Transform rightSpawnLimit = null;
   
   [Header("Movement")]
   [SerializeField][Min(0f)] private float horizontalPadding = 0.05f;

   private const int NoFinger = -1;
   
   private int activeFingerId = NoFinger;
   private int blockedFingerId = NoFinger;
   
   private bool mouseGestureActive = false;
   private bool invalidBoundsLogged = false;

   private void Awake()
   {
      if (!ValidateReferences())
      {
         enabled = false;
      }
      
      
   }

   private void OnEnable()
   {
      EnhancedTouchSupport.Enable();
   }

   private void OnDisable()
   {
      EnhancedTouchSupport.Disable();
      ResetGestureState();
   }


   private void LateUpdate()
   
   {
      if (!GameManager.Instance.IsGameplayActive)
      {

         if (mouseGestureActive || activeFingerId != NoFinger || blockedFingerId != NoFinger)
         {
            ResetGestureState();
         }
          
         return;
      }
      
      
      
      if (InputTouch.activeTouches.Count > 0 )
      {
         HandleTouchInput();
         return;
      }

      HandleMouseInput();
   }

   private void HandleMouseInput()
   {
      Mouse mouse = Mouse.current;

      if (mouse == null)
      {
         return;
      }
      
      if (mouse.leftButton.wasPressedThisFrame)
      {
         if (IsMouseOverUI() || !CanAim())
         {
            mouseGestureActive = false;
            return;
         }
         
         mouseGestureActive = true;
         
         
      }

      if (mouseGestureActive && mouse.leftButton.isPressed)
      {
         MoveCurrentFruit(mouse.position.ReadValue());
      }

      if (mouse.leftButton.wasReleasedThisFrame)
      {
         if (mouseGestureActive && CanAim())
         {
            fruitSpawner.DropCurrentFruit();
         }
         
         mouseGestureActive = false;
         
      }
      
   }

   private void HandleTouchInput()
   {
      
      var activeTouches = InputTouch.activeTouches;
      
      
      for (int index =0; index < activeTouches.Count; index++)
      {
          InputTouch touch = activeTouches[index];

         if (touch.phase == InputTouchPhase.Began && activeFingerId == NoFinger && blockedFingerId == NoFinger)
         {

            if (IsTouchOverUI(touch.touchId))
            {
               blockedFingerId = touch.touchId;
            }
            
            else if (CanAim())
            {
               activeFingerId = touch.touchId;
            }
            
            
         }

         if (touch.touchId == activeFingerId)
         {
            HandleActiveTouch(touch);
         }
         
         else if(touch.touchId == blockedFingerId && (touch.phase == InputTouchPhase.Ended || touch.phase == InputTouchPhase.Canceled))
         {
            blockedFingerId = NoFinger;
         }
         
      }
      
      
   }

   private void HandleActiveTouch(InputTouch touch)
   {
      switch (touch.phase)
      {
         case InputTouchPhase.Began:
         case InputTouchPhase.Moved:
            MoveCurrentFruit(touch.screenPosition);
            break;
         
         case InputTouchPhase.Ended:
            if (CanAim())
            {
               fruitSpawner.DropCurrentFruit();
            }
            
            activeFingerId = NoFinger;
            break;
         
         case InputTouchPhase.Canceled:
            activeFingerId = NoFinger;
            break;
            
      }
   }

   private void MoveCurrentFruit(Vector2 screenPosition)
   {
      Fruit currentFruit = fruitSpawner.CurrentFruit;

      if (currentFruit == null || !currentFruit.IsHeld)
      {
         return;
      }

      float radius = currentFruit.ColliderRadius;

      float minimumX = leftSpawnLimit.position.x + radius + horizontalPadding;
      
      float maximumX = rightSpawnLimit.position.x - radius - horizontalPadding;

      if (minimumX > maximumX)
      {

         if (!invalidBoundsLogged)
         {
            Debug.LogError("The fruit spawn limits are invalid", this);
            invalidBoundsLogged = true;
         }
         
         return;
         
      }
      
      invalidBoundsLogged = false;

      float distanceFromCamera = Mathf.Abs(
         currentFruit.transform.position.z -
         gameplayCamera.transform.position.z
      );

      Vector3 pointerPosition = new Vector3(
         screenPosition.x,
         screenPosition.y,
         distanceFromCamera
      );
      
      Vector3 worldPosition = gameplayCamera.ScreenToWorldPoint(pointerPosition);
      
      float clampedX = Mathf.Clamp(worldPosition.x, minimumX, maximumX);

      currentFruit.SetHeldHorizontalPosition(clampedX);

      

   }


   private bool CanAim()
   {
      return fruitSpawner.CurrentFruit != null && 
             fruitSpawner.CurrentFruit.IsHeld;
   }

   private bool IsMouseOverUI()
   {
      return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Mouse.current.deviceId);
   }

   private bool IsTouchOverUI(int touchId)
   {
      return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId);
   }
   
   private bool ValidateReferences()
   {
      if (fruitSpawner == null)
      {
         Debug.LogError(
            "FruitAimController has no FruitSpawner assigned.",
            this
         );

         return false;
      }

      if (gameplayCamera == null)
      {
         Debug.LogError(
            "FruitAimController has no gameplay camera assigned.",
            this
         );

         return false;
      }

      if (leftSpawnLimit == null || rightSpawnLimit == null)
      {
         Debug.LogError(
            "FruitAimController has missing spawn limits.",
            this
         );

         return false;
      }

      if (leftSpawnLimit.position.x >=
          rightSpawnLimit.position.x)
      {
         Debug.LogError(
            "The left spawn limit must be left of the right limit.",
            this
         );

         return false;
      }

      return true;
   }


   private void OnValidate()
   {
      horizontalPadding =Mathf.Max(0f, horizontalPadding);
   }
   
   private void ResetGestureState()
   {
      mouseGestureActive = false;
      activeFingerId = NoFinger;
      blockedFingerId = NoFinger;
   }
}
