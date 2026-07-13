using UnityEngine;

/// <summary>
/// UI ボタンの効果音を一括管理するシングルトン Manager。
/// シーンに1つ置いておけば、ButtonHoverEffect などから ButtonSoundManager.Instance で呼び出せる。
///
/// 使い方：
/// 1. 空のGameObjectを作って、このスクリプトをアタッチ
/// 2. Inspector で hoverClip / clickClip にAudioClip（効果音ファイル）をドラッグ＆ドロップ
/// 3. ButtonHoverEffect から自動的に呼ばれる
/// </summary>
public class ButtonSoundManager : MonoBehaviour
{
    // どこからでも呼べるようにシングルトン化
    public static ButtonSoundManager Instance { get; private set; }

    [Header("===== AudioSource =====")]
    [Tooltip("再生用 AudioSource。未設定なら自動で追加される")]
    [SerializeField] private AudioSource audioSource;

    [Header("===== 効果音クリップ =====")]
    [Tooltip("選択（ホバー/カーソル移動）したときの音")]
    [SerializeField] private AudioClip hoverClip;

    [Tooltip("ボタンを押した（クリック/決定）ときの音")]
    [SerializeField] private AudioClip clickClip;

    [Tooltip("パネルやメニューを閉じたときの音")]
    [SerializeField] private AudioClip closeClip;

    [Header("===== 音量 =====")]
    [Range(0f, 1f), Tooltip("選択音の音量")]
    [SerializeField] private float hoverVolume = 0.6f;

    [Range(0f, 1f), Tooltip("クリック音の音量")]
    [SerializeField] private float clickVolume = 0.8f;

    [Range(0f, 1f), Tooltip("閉じる音の音量")]
    [SerializeField] private float closeVolume = 0.8f;

    [Header("===== 連続再生の抑制 =====")]
    [Tooltip("同じ選択音を短時間に何度も鳴らさないためのクールタイム（秒）")]
    [SerializeField] private float hoverCooldown = 0.05f;

    // 最後にホバー音を鳴らした時刻（unscaledTime）。連打防止用
    private float lastHoverTime = -999f;

    private void Awake()
    {
        // シングルトン設定（重複したら自分を破棄）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // AudioSource が未設定なら自動でアタッチ
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // UI 音はループ不要、初期は自動再生しない
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// 選択（ホバー/カーソル移動）音を再生
    /// </summary>
    public void PlayHover()
    {
        if (audioSource == null || hoverClip == null) return;

        // 連打防止：短時間に何度も鳴らさない
        if (Time.unscaledTime - lastHoverTime < hoverCooldown) return;
        lastHoverTime = Time.unscaledTime;

        audioSource.PlayOneShot(hoverClip, hoverVolume);
    }

    /// <summary>
    /// クリック（決定）音を再生
    /// </summary>
    public void PlayClick()
    {
        if (audioSource == null || clickClip == null) return;

        audioSource.PlayOneShot(clickClip, clickVolume);
    }

    /// <summary>
    /// パネルやメニューを閉じたときの音を再生
    /// </summary>
    public void PlayClose()
    {
        if (audioSource == null || closeClip == null) return;

        audioSource.PlayOneShot(closeClip, closeVolume);
    }
}
