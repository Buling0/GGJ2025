using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip backgroundMusic; // 背景音乐音频剪辑
    private AudioSource audioSource;

    private void Awake()
    {
        // 确保只有一个MusicManager实例
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // 切换场景时不销毁
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // 设置循环播放
        audioSource.playOnAwake = false; // 不在Awake时播放
    }

    private void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
} 