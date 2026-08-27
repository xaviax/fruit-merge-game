using System;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    
    public static VFXManager Instance {get; private set;}
    
    [Header("VFX Prefabs")]
    [SerializeField] private OneShotParticleVFX impactVfxPrefab = null;
    [SerializeField] private OneShotParticleVFX dropVfxPrefab = null;
    [SerializeField] private HeldFruitGuide heldFruitGuide = null;

    private void Awake()
    {
       Instance = this; 
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void PlayImpactVFX(Vector2 contactPosition)
    {
        Play(impactVfxPrefab, contactPosition);
    }
    
    public void PlayDropVFX(Vector2 contactPosition)
    {
        Play(dropVfxPrefab, contactPosition);
    }


    public void ShowHeldFruitGuide(Fruit fruit)
    {
        heldFruitGuide.Show(fruit);
    }

    public void HideHeldFruitGuide()
    {
        heldFruitGuide.Hide();
    }
    
    
    private void Play(OneShotParticleVFX vfxPrefab, Vector2 contactPosition)
    {
        OneShotParticleVFX vfx = Instantiate(vfxPrefab, contactPosition, Quaternion.identity);
        vfx.Play();
    }
    
}
