using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSfx;
    // [SerializeField] private AudioSource audioBgm;
    [SerializeField] private AudioClip buttonClick;
    
    [Header("Memory Sequence")] 
    [SerializeField] private AudioClip lostGame;
    [SerializeField] private AudioClip levelUp;
    
    [Header("Verbal Memory")] 
    [SerializeField] private AudioClip incorrectClick;
    [SerializeField] private AudioClip correctClick;

    public static AudioManager Instance;

    private void Awake()
    {
        // if there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSfx.PlayOneShot(audioClip);
    }

    public void PlayLostGame()
    {
        audioSfx.PlayOneShot(lostGame);
    }
    
    public void PlayLevelUp()
    {
        audioSfx.PlayOneShot(levelUp);
    }

    public void PLayIncorrectClick()
    {
        audioSfx.PlayOneShot(incorrectClick);
    }
    
    public void PlayCorrectClick()
    {
        audioSfx.PlayOneShot(correctClick);
    }
}
