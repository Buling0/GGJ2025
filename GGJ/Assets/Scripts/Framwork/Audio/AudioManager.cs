using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : BaseManager<AudioManager>
{
    private AudioSource bgmSource = null;
    private float bgmValue = 1f;

    private GameObject AudioObj = null;
    private List<AudioSource> AudioList = new List<AudioSource>();
    private float audioValue = 1f;

    public AudioManager()
    {
        MonoManager.GetInstance().AddUpdateListener(AudioUpdate);
    }

    private void AudioUpdate()
    {
        for (int i = AudioList.Count - 1; i >= 0; i--)
        {
            if (!AudioList[i].isPlaying)
            {
                GameObject.Destroy(AudioList[i]);
                AudioList.RemoveAt(i);
            }
        }
    }
    
    
    /// <summary>
    /// 播放BGM
    /// </summary>
    /// <param name="name"></param>
    public void PalyBGM(string name)
    {
        if (bgmSource == null)
        {
            GameObject obj = new GameObject("BGMPlayer");
            bgmSource = obj.AddComponent<AudioSource>();
        }
        
        //异步加载BGM，BGM一般较大
        ResManager.GetInstance().LoadAsync<AudioClip>("Test/" + name, (clip) =>
        {
            bgmSource.clip = clip;
            bgmSource.volume = bgmValue;
            bgmSource.loop = true;
            bgmSource.Play();
        });
    }

    
    /// <summary>
    /// 改变音量大小
    /// </summary>
    /// <param name="x"></param>
    public void ChangerbgmValue(float x)
    {
        bgmValue = x;
        if (bgmSource == null)
        {
            return;
        }
        bgmSource.volume = bgmValue;
    }
    
    /// <summary>
    /// 暂停播放BGM
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource == null)
        {
            return;
        }
        bgmSource.Pause();
    }

    /// <summary>
    /// 停止播放BGM
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource == null)
        {
            return;
        }
        bgmSource.Stop();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    public void PlayAudio(string name, bool isLoop, UnityAction<AudioSource> callback = null)
    {
        if (AudioObj == null)
        {
            AudioObj = new GameObject("AudioRoot");
        }

        AudioSource source = AudioObj.AddComponent<AudioSource>();
        //当资源异步加载结束后 再吧音效添加进入List
        ResManager.GetInstance().LoadAsync<AudioClip>("Test/" + name, (clip) =>
        {
            source.clip = clip;
            source.volume = audioValue;
            source.loop = isLoop;
            source.Play();
            AudioList.Add(source);

            if (callback != null)
            {
                callback(source);
            }
        });
    }

    /// <summary>
    /// 改变所有音效的大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeAudioValue(float value)
    {
        audioValue = value;
        for (int i = 0; i < AudioList.Count; i++)
        {
            AudioList[i].volume = audioValue;
        }
    }
    
    /// <summary>
    /// 移除音效
    /// </summary>
    /// <param name="source"></param>
    public void StopAudio(AudioSource source)
    {
        if (AudioList.Contains(source))
        {
            AudioList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}
