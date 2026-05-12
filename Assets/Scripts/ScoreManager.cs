using UnityEngine;

/// <summary>
/// スコアを管理するシングルトン。
/// 他のスクリプトから ScoreManager.Instance.AddScore(100) のように呼べる。
/// シーンをまたいでも保持される。
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField, Header("現在のスコア")] private int currentScore = 0;
    [SerializeField, Header("ハイスコア")] private int highScore = 0;

    public int CurrentScore => currentScore;
    public int HighScore => highScore;

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

        // ハイスコアをPlayerPrefsから読み込み
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    /// <summary>
    /// スコアを加算する
    /// </summary>
    public void AddScore(int point)
    {
        currentScore += Mathf.Max(0, point);

        // ハイスコア更新
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// スコアを0にリセット（リトライ時などに呼ぶ）
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
    }
}
