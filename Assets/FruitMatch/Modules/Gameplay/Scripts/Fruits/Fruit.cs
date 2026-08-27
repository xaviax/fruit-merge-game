using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]




public class Fruit : MonoBehaviour
{
   [Header("References")]
   [SerializeField] private SpriteRenderer visual = null;
   
   
   private Rigidbody2D fruitRigidbody = null;
   private CircleCollider2D fruitCollider = null;
   
   public bool IsHeld { get; private set; }
   public bool IsDropped { get; private set; }
   
   public bool IsMerging { get; private set; }
   
   public FruitDefinition Definition { get; private set; }
   public float ColliderRadius => fruitCollider != null ? fruitCollider.radius : 0f;

   private const float spriteDiameter = 5.12f;

   private bool hasPlayedImpactVfx = false;
   
   private void Awake()
   {
      fruitRigidbody = GetComponent<Rigidbody2D>();
      fruitCollider = GetComponent<CircleCollider2D>();
      
      
   }

   public void Initialize(FruitDefinition definition)
   {
      
      Definition = definition;

      gameObject.name = $"Fruit_{definition.Tier}_{definition.DisplayName}";

      ApplyVisual();
      ApplyPhysics();

   }

   public bool CanMergeWith(Fruit otherFruit)
   {
      return otherFruit != null && 
             otherFruit != this && 
             Definition != null && 
             otherFruit.Definition != null &&
             Definition == otherFruit.Definition && 
             IsDropped && 
             otherFruit.IsDropped &&
             !IsMerging &&
             !otherFruit.IsMerging;
      
   }

   public bool TryBeginMergingWith(Fruit otherFruit)
   {

      if (!CanMergeWith(otherFruit))
      {
         return false;
      }

      IsMerging = true;
      otherFruit.IsMerging = true;
      
      return true;



   }
   
   private void OnCollisionEnter2D(Collision2D collision)
   {
      if (collision.gameObject.TryGetComponent(out Fruit otherFruit) && TryBeginMergingWith(otherFruit))
      {
         FruitMergeSystem.Instance.TryMerge(this, otherFruit);
         return;
      }
      
      PlayVFXOnCollisionPoint(collision);
      
   }

   
   

   private void ApplyVisual()
   {
      
      visual.sprite = Definition.Sprite;
      
      
      //Debug.LogWarning("X: " + Definition.Sprite.bounds.size.x + " Y: " + Definition.Sprite.bounds.size.y);
      
      
      float visualScale = Definition.VisualDiameter / spriteDiameter;
      
      visual.transform.localScale = new Vector3(visualScale, visualScale, 1);
      
      
   }

   private void ApplyPhysics()
   {
      fruitRigidbody.mass = Definition.Mass;
      fruitCollider.radius = Definition.ColliderRadius;
      
      fruitRigidbody.linearVelocity = Vector2.zero;
      fruitRigidbody.angularVelocity = 0f;
      fruitRigidbody.WakeUp();
      
      Debug.Log("Updated Physics application for fruit");
      
   }


 

   public void PrepareForAiming()
   {
      if (Definition == null)
      {
         Debug.LogError("FruitDefinition is null", this);
         return;
      }
      
      fruitRigidbody.linearVelocity = Vector2.zero;
      fruitRigidbody.angularVelocity = 0f;
      
      fruitRigidbody.bodyType = RigidbodyType2D.Kinematic;
      fruitRigidbody.gravityScale = 0f;
      
      fruitCollider.enabled = false;
      
      IsHeld = true;
      IsDropped = false;
      
      
      
   }


   public void PrepareAsMergedFruit()
   {
      if (Definition == null)
      {
         Debug.LogError("Cannot prepare merged fruit without fruit definition", this);
         return;
      }
      
      
      
      fruitRigidbody.bodyType = RigidbodyType2D.Dynamic;
      fruitRigidbody.gravityScale = 1f;
      
      fruitRigidbody.linearVelocity = Vector2.zero;
      fruitRigidbody.angularVelocity = 0f;
      
      
      fruitCollider.enabled = enabled;
      
      IsHeld = false;
      IsDropped = true;
      IsMerging = false;
      
      
   }


   public void Release()
   {
      if (!IsHeld)
      {
         return;
      }
      
      fruitRigidbody.linearVelocity = Vector2.zero;
      fruitRigidbody.angularVelocity = 0f;
      
      fruitRigidbody.bodyType = RigidbodyType2D.Dynamic;
      fruitRigidbody.gravityScale = 1;
      
      fruitCollider.enabled = true;
      
      IsHeld = false;
      IsDropped = true;
      
   }

   public void SetHeldHorizontalPosition(float xPosition)
   {
      if (!IsHeld)
      {
         return;
      }
      
      Vector2 position = fruitRigidbody.position;
      position.x = xPosition;
      
      // you can literally just do what's written below
      fruitRigidbody.position = position;
      
      
   }

   private void PlayVFXOnCollisionPoint(Collision2D collision)
   {
      if (hasPlayedImpactVfx)
      {
         return;
      }
      
      hasPlayedImpactVfx = true;
      Vector2 contactPosition = collision.GetContact(0).point;
      VFXManager.Instance.PlayDropVFX(contactPosition);
      
     
      
   }
   
   
   
   
    
}
