using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームオーバー画面の管理。撃墜数表示と「もう一度／タイトルへ」ボタンを扱う。
/// GameClearManager とほぼ同じ作り。
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [SerializeField, Header("撃墜数表示テキスト")] private TextMeshProUGUI killCountText;
    [SerializeField, Tooltip("表示フォーマット（{0}に撃墜数が入る）")]
    private string killCountFormat = "撃墜した敵機の数: {0}";
    [SerializeField, Tooltip("撃墜数の桁数。4なら 0012 のようにゼロ埋めされる")]
    private int killCountDigits = 4;

    [SerializeField, Tooltip("フェードシステムのスクリプト")] private FadeManager fadeManager;
    [SerializeField, Tooltip("フェードの画像オブジェクト")] private GameObject fadeObject;

    void Start()
    {
        // ゲームオーバー画面では時間を通常通りに戻す
        Time.timeScale = 1f;

        fadeManager.FadeStart(fadeManager.GameStart);

        DisplayKillCount();
    }

    /// <summary> 撃墜数をテキストに反映する </summary>
    private void DisplayKillCount()
    {
        if (killCountText == null) return;

        int kills = (KillCountManager.Instance != null) ? KillCountManager.Instance.KillCount : 0;
        killCountText.text = string.Format(killCountFormat, kills.ToString("D" + Mathf.Max(1, killCountDigits)));
    }

    // もう一度ボタン → ゲームシーンに戻る
    public void OnRetryButton()
    {
        if (fadeManager.Bfade) return;//フェード中なら操作不可

        ResetResult();
        fadeObject.SetActive(true);
        fadeManager.FadeStart(fadeManager.ChangeGameScene);// ゲームシーンに遷移
    }

    // タイトルへボタン → タイトルに戻る
    public void OnTitleButton()
    {
        if (fadeManager.Bfade) return;//フェード中なら操作不可

        ResetResult();
        fadeObject.SetActive(true);
        fadeManager.FadeStart(fadeManager.ChangeTitleScene);
    }

    /// <summary> リザルトの数値をリセットする </summary>
    private void ResetResult()
    {
        if (KillCountManager.Instance != null)
            KillCountManager.Instance.ResetKillCount();
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();
    }
}
