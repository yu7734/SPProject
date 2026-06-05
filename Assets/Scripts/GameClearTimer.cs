using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// 制限時間が経過したらゲームクリアにするスクリプト。
/// Inspector で制限時間（秒）を設定でき、残り時間を TextMeshPro で表示する。
/// 時間が 0 になると指定したクリアシーンへ遷移する。
///
/// 使い方：
///   1. 空の GameObject（例：「GameClearTimer」）を Game シーンに置く
///   2. このスクリプトをアタッチ
///   3. timeLimit に制限時間（秒）を入れる
///   4. timerText に画面の TextMeshProUGUI を割り当てる
///   5. clearSceneName にクリアシーン名（既定は "GameClear"）を入れる
/// </summary>
public class GameClearTimer : MonoBehaviour
{
    [SerializeField, Header("制限時間（秒）")]
    private float timeLimit = 60f;

    [SerializeField, Header("残り時間を表示するテキスト")]
    private TextMeshProUGUI timerText;

    [SerializeField, Header("クリア時に遷移するシーン名")]
    private string clearSceneName = "GameClear";

    [Header("===== 表示設定 =====")]
    [SerializeField, Tooltip("「mm:ss」形式で表示する。OFFなら秒数のみ")]
    private bool useMinutesFormat = true;

    [SerializeField, Tooltip("残り時間がこの秒数以下になると文字色を変える")]
    private float warningTime = 10f;

    [SerializeField, Tooltip("通常時の文字色")]
    private Color normalColor = Color.white;

    [SerializeField, Tooltip("警告時（残りわずか）の文字色")]
    private Color warningColor = new Color(1f, 0.3f, 0.3f, 1f);

    // 残り時間
    private float remainingTime;
    // 二重遷移を防ぐフラグ
    private bool hasCleared = false;

    void Start()
    {
        remainingTime = timeLimit;
        UpdateTimerText();
    }

    void Update()
    {
        if (hasCleared) return;

        // 時間を減らす（ジャスト回避のスローなどに合わせるなら deltaTime のままでOK）
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerText();
            GameClear();
            return;
        }

        UpdateTimerText();
    }

    /// <summary>
    /// 残り時間をテキストに反映する
    /// </summary>
    private void UpdateTimerText()
    {
        if (timerText == null) return;

        if (useMinutesFormat)
        {
            // 切り上げて「mm:ss」表示（00:00 でちょうどクリア）
            int totalSeconds = Mathf.CeilToInt(remainingTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            // 秒数のみ（小数1桁）
            timerText.text = Mathf.Max(0f, remainingTime).ToString("0.0");
        }

        // 警告色の切り替え
        timerText.color = (remainingTime <= warningTime) ? warningColor : normalColor;
    }

    /// <summary>
    /// ゲームクリア処理：クリアシーンへ遷移する
    /// </summary>
    private void GameClear()
    {
        if (hasCleared) return;
        hasCleared = true;

        // 念のため時間スケールを通常へ戻す（ジャスト回避のスロー対策）
        Time.timeScale = 1f;

        SceneManager.LoadScene(clearSceneName);
    }
}
