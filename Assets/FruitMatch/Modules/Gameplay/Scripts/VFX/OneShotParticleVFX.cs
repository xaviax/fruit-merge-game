using System.Collections;
using UnityEngine;

public class OneShotParticleVFX : MonoBehaviour
{
   private ParticleSystem rootParticleSystem;

   private void Awake()
   {
      rootParticleSystem = GetComponent<ParticleSystem>();
   }

   public void Play()
   {
      rootParticleSystem.Play(true);

      StartCoroutine(DestroyWhenFinished());
   }

   private IEnumerator DestroyWhenFinished()
   {
      yield return null;

      while (rootParticleSystem.IsAlive(true))
      {
         yield return null;
      }
      
      Destroy(gameObject);
      
   }
   
}
