using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Instance 
    public static SoundManager Instance;
    
    // Music Audio Source 
    [Header("Music Audio Source")]
    [SerializeField] private AudioSource musicAudioSource = null;
    
    // Sound Effect Audio Source 
    [Header("Sound Effect Audio Source")]
    [SerializeField] private AudioSource fruitMergeAudioSource = null;
    [SerializeField] private AudioSource fruitExplodeAudioSource = null;
    [SerializeField] private AudioSource onclickAudioSource = null;
    
    
    
    [Header("Clips")]
    [SerializeField] private AudioClip musicClip = null;
    [SerializeField] private AudioClip fruitMergeClip = null;
    [SerializeField] private AudioClip fruitExplodeClip = null;
    [SerializeField] private AudioClip onclickClip = null;
    
    
    
    

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        musicAudioSource.clip = musicClip;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
        
        fruitMergeAudioSource.clip = fruitMergeClip;
        fruitExplodeAudioSource.clip = fruitExplodeClip;
        onclickAudioSource.clip = onclickClip;
        
    }


    public void PlayFruitMergeSound()
    {
        fruitMergeAudioSource.Play();
    }

    public void PlayFruitExplodeSound()
    {
        fruitExplodeAudioSource.Play();
    }

    public void PlayOnClickSound()
    {
        onclickAudioSource.Play();
    }
    
    

    public void ToggleMusic(bool isEnabled)
    {
        musicAudioSource.mute = !isEnabled;
    }
    
    public void ToggleSoundEffects(bool isEnabled)
    {
        fruitMergeAudioSource.mute = !isEnabled;
        fruitExplodeAudioSource.mute = !isEnabled;
    }
    
    
}
