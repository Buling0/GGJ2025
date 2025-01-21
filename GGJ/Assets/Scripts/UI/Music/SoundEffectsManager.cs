using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    private static SoundEffectsManager instance;
    private AudioSource audioSource;
    public AudioClip hoverSound; // 悬停音效
    public AudioClip clickSound; // 点击音效

    public static SoundEffectsManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 确保获取并启用 AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.enabled = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayHoverSound()
    {
        PlaySound(hoverSound);
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            // 确保 AudioSource 是启用的
            if (!audioSource.enabled)
            {
                audioSource.enabled = true;
            }
            PlaySound(clickSound);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null && audioSource.enabled)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Failed to play sound: AudioSource is not ready or clip is null");
        }
    }

    // 添加音量控制方法
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    // 添加静音控制方法
    public void SetMute(bool isMuted)
    {
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
} 