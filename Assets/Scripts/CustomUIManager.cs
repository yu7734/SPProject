using UnityEngine;
using TMPro;

/// <summary>
/// UI表示まわりを集約する窓口スクリプト。
/// スコア表示、ハイスコア表示などをまとめて管理する。
/// 既存のUIManagerには手を加えず、別系統として動作する。
///
/// 使い方：
///   - Gameシーン用：scoreText だけ刺せばOK
///   - GameOverシーン用：scoreText と highScoreText を刺せばリザルトとして使える
///   - タイトル画面用：highScoreText だけ刺せばハイスコアだけ表示
/// 不要な参照は空のままでOK（自動でスキップされる）
/// </summary>
public class CustomUIManager : MonoBehaviour
{
    [Header("===== スコア表示 =====")]
    [SerializeField, Header("現在スコア表示用テキスト")] private TextMeshProUGUI scoreText;
    [SerializeField, Header("ハイスコア表示用テキスト")] private TextMeshProUGUI highScoreText;
    [SerializeField, Header("スコアの接頭辞")] private string scorePrefix = "SCORE : ";
    [SerializeField, Header("ハイスコアの接頭辞")] private string highScorePrefix = "HI-SCORE : ";

    [Header("===== 表示桁数 =====")]
    [SerializeField, Header("ゼロ埋め桁数（0なら無効）")] private int padDigits = 6;

    void Update()
    {
        UpdateScoreUI();
        UpdateHighScoreUI();
    }

    /// <summary>現在スコアの表示更新</summary>
    private void UpdateScoreUI()
    {
        if (scoreText == null || ScoreManager.Instance == null) return;
        scoreText.text = scorePrefix + FormatScore(ScoreManager.Instance.CurrentScore);
    }

    /// <summary>ハイスコアの表示更新</summary>
    private void UpdateHighScoreUI()
    {
        if (highScoreText == null || ScoreManager.Instance == null) return;
        highScoreText.text = highScorePrefix + FormatScore(ScoreManager.Instance.HighScore);
    }

    /// <summary>スコアを整形（ゼロ埋め）</summary>
    private string FormatScore(int score)
    {
        if (padDigits > 0)
        {
            return score.ToString("D" + padDigits);
        }
        return score.ToString();
    }
}
