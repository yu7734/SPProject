using UnityEngine;

/// <summary>
/// 「撃墜した敵機の数」を管理するシングルトン。
/// シーンをまたいで保持されるので、Game シーンで数えた値を
/// GameOver / GameClear シーンでそのまま表示できる。
///
/// 【重要】このクラスはシーンに配置する必要がありません。
/// RuntimeInitializeOnLoadMethod により、ゲーム起動時に自動で生成されます。
/// （既存の ScoreManager はどのシーンにも配置されていないため動作していませんが、
/// 　この方式ならヒエラルキーの設定漏れで動かなくなることがありません）
/// </summary>
public class KillCountManager : MonoBehaviour
{
    public static KillCountManager Instance { get; private set; }

    [SerializeField, Tooltip("現在の撃墜数（実行中の確認用。手動で変更しないこと）")]
    private int killCount = 0;

    /// <summary> 現在の撃墜数 </summary>
    public int KillCount => killCount;

    /// <summary>
    /// ゲーム起動時に自動でインスタンスを生成する。
    /// どのシーンから再生を始めても必ず存在する状態になる。
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;

        GameObject obj = new GameObject(nameof(KillCountManager));
        obj.AddComponent<KillCountManager>();
    }

    void Awake()
    {
        // シングルトン化（重複したら自分を消す）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 撃墜数を1加算する。敵を弾で撃破したときに呼ぶ。
    /// </summary>
    public void AddKill()
    {
        killCount++;
    }

    /// <summary>
    /// 撃墜数を0に戻す。リトライ・タイトルへ戻るときに呼ぶ。
    /// </summary>
    public void ResetKillCount()
    {
        killCount = 0;
    }
}
