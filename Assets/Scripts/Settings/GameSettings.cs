using System;
using UnityEngine;

/// <summary>
/// ゲーム全体の設定値（操作の反転・音量）を保持するシングルトン。
/// PlayerPrefs に保存されるので、アプリを終了しても設定が残る。
///
/// 【重要】このクラスはシーンに配置する必要がありません。
/// KillCountManager と同じく RuntimeInitializeOnLoadMethod で自動生成され、
/// DontDestroyOnLoad でシーンをまたいで生き続けます。
///
/// 読み書きの例：
///   GameSettings.Instance.InvertY = true;          // 上下反転をON
///   float v = GameSettings.Instance.SeVolume;      // SE音量を取得
/// 値が変わると OnSettingsChanged が呼ばれます。
/// </summary>
public class GameSettings : MonoBehaviour
{
    // ===== PlayerPrefs のキー名（変更するとユーザーの設定がリセットされるので注意） =====
    private const string KeyMasterVolume = "SP_Settings_MasterVolume";
    private const string KeyBgmVolume    = "SP_Settings_BgmVolume";
    private const string KeySeVolume     = "SP_Settings_SeVolume";
    private const string KeyInvertX      = "SP_Settings_InvertX";
    private const string KeyInvertY      = "SP_Settings_InvertY";

    // ===== 初期値（既存のゲームバランスを変えないよう、音量は全て 1.0 = 等倍） =====
    public const float DefaultMasterVolume = 1f;
    public const float DefaultBgmVolume    = 1f;
    public const float DefaultSeVolume     = 1f;
    public const bool  DefaultInvertX      = false;
    public const bool  DefaultInvertY      = false;

    private static GameSettings instance;
    private static bool isQuitting = false;

    /// <summary>
    /// どこからでも呼べる本体。まだ無ければその場で生成される。
    /// 編集中（再生していない時）は生成せずに null を返す。
    /// エディタ拡張から触られた時にシーンへ余計なオブジェクトを作らないため。
    /// </summary>
    public static GameSettings Instance
    {
        get
        {
            if (instance == null && !isQuitting && Application.isPlaying)
            {
                CreateInstance();
            }
            return instance;
        }
    }

    /// <summary>設定値が変更されたときに呼ばれる（音量の即時反映などに使う）</summary>
    public event Action OnSettingsChanged;

    private float masterVolume = DefaultMasterVolume;
    private float bgmVolume    = DefaultBgmVolume;
    private float seVolume     = DefaultSeVolume;
    private bool  invertX      = DefaultInvertX;
    private bool  invertY      = DefaultInvertY;

    /// <summary>全体音量（0〜1）</summary>
    public float MasterVolume
    {
        get => masterVolume;
        set => SetFloat(ref masterVolume, value, KeyMasterVolume);
    }

    /// <summary>BGM音量（0〜1）</summary>
    public float BgmVolume
    {
        get => bgmVolume;
        set => SetFloat(ref bgmVolume, value, KeyBgmVolume);
    }

    /// <summary>効果音の音量（0〜1）</summary>
    public float SeVolume
    {
        get => seVolume;
        set => SetFloat(ref seVolume, value, KeySeVolume);
    }

    /// <summary>左右反転（ONなら横移動の入力を反転する）</summary>
    public bool InvertX
    {
        get => invertX;
        set => SetBool(ref invertX, value, KeyInvertX);
    }

    /// <summary>上下反転（ONなら縦移動の入力を反転する）</summary>
    public bool InvertY
    {
        get => invertY;
        set => SetBool(ref invertY, value, KeyInvertY);
    }

    /// <summary>ゲーム起動時に自動生成する（どのシーンから再生を始めても存在する状態になる）</summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (instance != null) return;
        CreateInstance();
    }

    private static void CreateInstance()
    {
        if (instance != null) return;

        // 既にシーン内に置かれている場合はそれを使う
        instance = FindAnyObjectByType<GameSettings>();
        if (instance != null) return;

        GameObject obj = new GameObject(nameof(GameSettings));
        obj.AddComponent<GameSettings>();
    }

    private void Awake()
    {
        // シングルトン化（重複したら自分を消す）
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
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

    /// <summary>PlayerPrefs から読み込む</summary>
    public void Load()
    {
        masterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(KeyMasterVolume, DefaultMasterVolume));
        bgmVolume    = Mathf.Clamp01(PlayerPrefs.GetFloat(KeyBgmVolume,    DefaultBgmVolume));
        seVolume     = Mathf.Clamp01(PlayerPrefs.GetFloat(KeySeVolume,     DefaultSeVolume));
        invertX      = PlayerPrefs.GetInt(KeyInvertX, DefaultInvertX ? 1 : 0) != 0;
        invertY      = PlayerPrefs.GetInt(KeyInvertY, DefaultInvertY ? 1 : 0) != 0;

        OnSettingsChanged?.Invoke();
    }

    /// <summary>全ての設定を初期値に戻す</summary>
    public void ResetToDefault()
    {
        masterVolume = DefaultMasterVolume;
        bgmVolume    = DefaultBgmVolume;
        seVolume     = DefaultSeVolume;
        invertX      = DefaultInvertX;
        invertY      = DefaultInvertY;

        PlayerPrefs.SetFloat(KeyMasterVolume, masterVolume);
        PlayerPrefs.SetFloat(KeyBgmVolume,    bgmVolume);
        PlayerPrefs.SetFloat(KeySeVolume,     seVolume);
        PlayerPrefs.SetInt(KeyInvertX, invertX ? 1 : 0);
        PlayerPrefs.SetInt(KeyInvertY, invertY ? 1 : 0);
        PlayerPrefs.Save();

        OnSettingsChanged?.Invoke();
    }

    /// <summary>移動入力に反転設定を適用して返す（PlayerManager から呼ぶ）</summary>
    public Vector2 ApplyInvert(Vector2 input)
    {
        if (invertX) input.x = -input.x;
        if (invertY) input.y = -input.y;
        return input;
    }

    private void SetFloat(ref float field, float value, string key)
    {
        value = Mathf.Clamp01(value);
        if (Mathf.Approximately(field, value)) return;

        field = value;
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
        OnSettingsChanged?.Invoke();
    }

    private void SetBool(ref bool field, bool value, string key)
    {
        if (field == value) return;

        field = value;
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
        OnSettingsChanged?.Invoke();
    }
}
