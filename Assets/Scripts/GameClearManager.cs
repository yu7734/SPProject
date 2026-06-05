using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// ゲームクリア画面の管理。スコア表示と「もう一度／タイトルへ」ボタンを扱う。
/// GameOverManager とほぼ同じ作り。
/// </summary>
public class GameClearManager : MonoBehaviour
{
    [SerializeField, Header("スコア表示テキスト")] private TextMeshProUGUI scoreText;
    [SerializeField, Header("ハイスコア表示テキスト")] private TextMeshProUGUI highScoreText;

    void Start()
    {
        // クリア画面では時間を通常通りに戻す
        Time.timeScale = 1f;

        if (ScoreManager.Instance != null)
        {
            if (scoreText != null)
                scoreText.text = "SCORE : " + ScoreManager.Instance.CurrentScore.ToString("N0");
            if (highScoreText != null)
                highScoreText.text = "HIGH SCORE : " + ScoreManager.Instance.HighScore.ToString("N0");
        }
    }

    // もう一度ボタン → ゲームシーンに戻る
    public void OnRetryButton()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene("Game");
    }

    // タイトルへボタン → タイトルに戻る
    public void OnTitleButton()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene("Title");
    }
}
