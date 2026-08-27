using UnityEngine;

[CreateAssetMenu(
    
    fileName = "FruitDefinition", 
    menuName = "Fruit Merge /FruitDefinition"


)]
public sealed class FruitDefinition : ScriptableObject
{
    [Header("Identity")] 
    [SerializeField, Min(1)] private int tier = 1;
    [SerializeField] private string displayName = "Fruit";
    
    [Header("Visual")]
    [SerializeField] private Sprite sprite = null;
    [SerializeField, Min(0.01f)] private float visualDiameter = 1;
    
    [Header("Physics")]
    [SerializeField, Min(0.01f)] private float colliderRadius = 0.5f;
    [SerializeField,Min(0.01f)] private float mass = 1f;
    
    [Header("Scoring")]
    [SerializeField, Min(0)] private int mergeScore = 10;


    public int Tier => tier;
    public string DisplayName => displayName;
    public Sprite Sprite => sprite;
    public float VisualDiameter => visualDiameter;
    public float ColliderRadius => colliderRadius;
    public float Mass => mass;
    public int MergeScore => mergeScore;


}
