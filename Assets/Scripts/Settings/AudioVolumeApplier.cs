using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameSettings の音量設定を、実際に鳴っている音へ反映するスクリプト。
///
/// ・全体音量 … AudioListener.volume にそのまま設定（すべての音がまとめて下がる）
/// ・BGM / SE … シーン内の AudioSource を1つずつ見て、種別ごとに音量を掛ける
///
/// 種別の判定は
///   1. AudioCategory コンポーネントが付いていればその指定を優先
///   2. 無ければ loop が ON なら BGM、OFF なら SE
/// という順番。ループしない BGM などは AudioCategory を付けて指定すること。
///
/// 【重要】このクラスもシーンに配置不要。起動時に自動生成されます。
/// AudioMixer を使わない方式なので、既存の SoundManager / ButtonSoundManager に
/// 手を加えなくてもそのまま音量調整が効きます。
/// </summary>
public class AudioVolumeApplier : MonoBehaviour
{
    private static AudioVolumeApplier instance;
    private static bool isQuitting = false;

    public static AudioVolumeApplier Instance
    {
        get
        {
            if (instance == null && !isQuitting)
            {
                CreateInstance();
            }
            return instance;
        }
    }

    [SerializeField, Tooltip("新しく増えた AudioSource を探し直す間隔（秒）")]
    private float rescanInterval = 0.5f;

    /// <summary> AudioSource ごとの「本来の音量」。ここに設定値を掛けたものを実際の音量にする </summary>
    private readonly Dictionary<AudioSource, float> baseVolumes = new Dictionary<AudioSource, float>();

    /// <summary> 破棄済み AudioSource の掃除用（毎フレームの確保を避けるため使い回す） </summary>
    private readonly List<AudioSource> removeCache = new List<AudioSource>();

    private float rescanTimer = 0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (instance != null) return;
        CreateInstance();
    }

    private static void CreateInstance()
    {
        if (instance != null) return;

        instance = FindAnyObjectByType<AudioVolumeApplier>();
        if (instance != null) return;

        GameObject obj = new GameObject(nameof(AudioVolumeApplier));
        obj.AddComponent<AudioVolumeApplier>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Rescan();
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void LateUpdate()
    {
        // 一定間隔で AudioSource を探し直す（シーン遷移や実行中に追加された分を拾う）
        rescanTimer -= Time.unscaledDeltaTime;
        if (rescanTimer <= 0f)
        {
            rescanTimer = Mathf.Max(0.1f, rescanInterval);
            Rescan();
        }

        Apply();
    }

    /// <summary>シーン内の AudioSource を集めて、本来の音量を覚えておく</summary>
    public void Rescan()
    {
        // 非アクティブなオブジェクトの AudioSource も含めて取得する
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < sources.Length; ++i)
        {
            AudioSource source = sources[i];
            if (source == null) continue;

            // 初めて見つけた AudioSource は、その時点の音量を「本来の音量」として記憶する
            if (!baseVolumes.ContainsKey(source))
            {
                baseVolumes.Add(source, source.volume);
            }
        }
    }

    /// <summary>覚えている AudioSource すべてに現在の設定を反映する</summary>
    private void Apply()
    {
        GameSettings settings = GameSettings.Instance;
        if (settings == null) return;

        // 全体音量はリスナー側でまとめて掛ける
        AudioListener.volume = settings.MasterVolume;

        removeCache.Clear();

        foreach (KeyValuePair<AudioSource, float> pair in baseVolumes)
        {
            AudioSource source = pair.Key;

            // 破棄された AudioSource は後でリストから外す
            if (source == null)
            {
                removeCache.Add(source);
                continue;
            }

            float categoryVolume = IsBgm(source) ? settings.BgmVolume : settings.SeVolume;
            source.volume = pair.Value * categoryVolume;
        }

        for (int i = 0; i < removeCache.Count; ++i)
        {
            baseVolumes.Remove(removeCache[i]);
        }
        removeCache.Clear();
    }

    /// <summary>その AudioSource が BGM 扱いかどうか</summary>
    private bool IsBgm(AudioSource source)
    {
        // AudioCategory が付いていればそちらを優先
        if (source.TryGetComponent<AudioCategory>(out var category))
        {
            return category.CurrentCategory == AudioCategory.Category.BGM;
        }

        // 付いていなければ「ループする音＝BGM」とみなす
        return source.loop;
    }

    /// <summary>
    /// 実行中に AudioSource の音量を意図的に変えた場合、その値を「本来の音量」として覚え直す。
    /// （BGMのフェードなどを自前で行うスクリプトから呼ぶ用。通常は使わなくてよい）
    /// </summary>
    public void OverrideBaseVolume(AudioSource source, float baseVolume)
    {
        if (source == null) return;
        baseVolumes[source] = Mathf.Clamp01(baseVolume);
    }
}
