using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    private static SoundEffectsManager instance;
    public static SoundEffectsManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 如果实例不存在，先尝试在场景中查找
                instance = FindObjectOfType<SoundEffectsManager>();
                
                // 如果场景中没有，则创建一个新的
                if (instance == null)
                {
                    GameObject go = new GameObject("SoundEffectsManager");
                    instance = go.AddComponent<SoundEffectsManager>();
                }
            }
            return instance;
        }
    }

    public AudioClip hoverSound; // 悬停音效
    public AudioClip clickSound; // 点击音效
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSource();
    }

    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.volume = 0.5f; // 调整音量到合适的大小
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.volume = 0.5f; // 调整音量到合适的大小
            audioSource.PlayOneShot(clickSound);
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