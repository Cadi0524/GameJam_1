using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SoundType
{

    // BGM ( 101)
    MainBGM = 101,



    // 효과음 ( 1 ~ 100 )
    ImageToColor = 1,
    Success = 2,
    Fail = 3,
    MailArrived = 4,
    PaperSwap = 5,
    PaperUP = 6,

    ClickUI = 7,
    PenUp = 8,
    PenDown = 9,
    MailBoxOpen = 10,


}

public class SoundManager : SerializedMonoBehaviour
{

    public static SoundManager Instance { get; private set; }

    [Title("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Title("Sound Dictionary")]
    public Dictionary<SoundType, AudioClip> soundDict = new Dictionary<SoundType, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 효과음 재생 (중첩 가능)
    /// </summary>
    public void PlaySFX(SoundType type)
    {
        if (soundDict.TryGetValue(type, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] {type}에 할당된 클립이 없습니다!");
        }
    }


    public void PlayBGM(SoundType type)
    {
        if (soundDict.TryGetValue(type, out AudioClip clip))
        {
            if (bgmSource.clip == clip && bgmSource.isPlaying) return;

            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM() => bgmSource.Stop();

}
