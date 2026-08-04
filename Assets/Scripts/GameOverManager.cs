using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField, Header("スコア表示テキスト")] private TextMeshProUGUI scoreText;
    [SerializeField, Header("ハイスコア表示テキスト")] private TextMeshProUGUI highScoreText;

    [SerializeField, Tooltip("フェードシステムのスクリプト")] private FadeManager fadeManager;
    [SerializeField, Tooltip("フェードの画像オブジェクト")] private GameObject fadeObject;

    void Start()
    {
        // ゲームオーバー画面では時間を通常通りに戻す
        Time.timeScale = 1f;

        fadeManager.FadeStart(fadeManager.GameStart);

        // スコア表示
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
        if (fadeManager.Bfade) return;//フェード中なら操作不可
        // スコアをリセットしてから遷移
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }
        fadeObject.SetActive(true);
        fadeManager.FadeStart(fadeManager.ChangeGameScene);// ゲームシーンに遷移
    }

    // タイトルへボタン → タイトルに戻る
    public void OnTitleButton()
    {
        if (fadeManager.Bfade) return;//フェード中なら操作不可
        // スコアをリセットしてから遷移
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }
        fadeObject.SetActive(true);
        fadeManager.FadeStart(fadeManager.ChangeTitleScene);
    }
}
