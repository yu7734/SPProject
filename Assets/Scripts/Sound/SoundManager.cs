using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //AudioSource(スピーカー)を同時に鳴らしたい音の数を生成
    private AudioSource[] audioSources = new AudioSource[10];

    private void Awake()
    {
        //audioSource分の配列の数だけをAudioSouceを自分自身に生成して配列に格納
        for (int i = 0; i < audioSources.Length; ++i)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //未使用のAudioSourceの取得　全て使用中は全てnullを返す

    private AudioSource GetUnUsedeAudioSource() => audioSources.FirstOrDefault(t => t.isPlaying == false);

    //指定されたAudioClipを未使用のAudioSourceで再生
    public void Play(AudioClip clip)
    {
        var audioSource = GetUnUsedeAudioSource();
        if (audioSource == null) return;
        audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
    }
}
