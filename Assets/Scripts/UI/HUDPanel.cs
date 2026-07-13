using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 右上のHULLパネル / 左上のEXPERIENCEパネルなど、
/// 「ラベル＋数値＋バー」の構成のHUDパネル全体を1つにまとめて管理する。
/// 既存のHPBar, EXPBar, UIManagerには手を加えない。
///
/// 用途：
///   - HULLパネル：currentValueにplayer.playerHP、maxValueにplayer.MaxPlayerHPを反映
///   - EXPパネル：currentValueにuiManager.experiencePoint、maxValueに100
///   - SCORE表示：scoreText に ScoreManager の値を表示
/// </summary>
public class HUDPanel : MonoBehaviour
{
    public enum SourceType
    {
        PlayerHP,        // PlayerManagerのHPを表示
        Experience,      // UIManagerの経験値を表示
        ScoreOnly,       // スコアだけ（バー無し）
    }

    [Header("===== データソース =====")]
    [SerializeField] private SourceType sourceType = SourceType.PlayerHP;
    [SerializeField, Header("プレイヤー参照（PlayerHP用）")] private PlayerManager player;
    [SerializeField, Header("UIManager参照（Experience用）")] private UIManager uiManager;
    [SerializeField, Header("EXPの最大値")] private int maxExperience = 100;
    [SerializeField, Header("LV計算用：経験値÷この値+1がレベル")] private int expPerLevel = 100;

    [Header("===== UI参照 =====")]
    [SerializeField, Header("ラベルテキスト（例：HULL、EXPERIENCE）")] private TextMeshProUGUI labelText;
    [SerializeField, Header("現在値テキスト（例：78/100）")] private TextMeshProUGUI valueText;
    [SerializeField, Header("バーのSlider")] private Slider barSlider;
    [SerializeField, Header("スコア表示用テキスト")] private TextMeshProUGUI scoreText;

    [Header("===== 表示設定 =====")]
    [SerializeField, Header("ラベル文字列（例：HULL）")] private string labelPrefix = "● HULL";
    [SerializeField, Header("レベル表示を含めるか")] private bool showLevel = false;
    [SerializeField, Header("バーの追従速度")] private float smoothSpeed = 8f;
    [SerializeField, Header("値表示のフォーマット 0=78/100 1=78 2=空")] private int valueFormat = 0;

    void Update()
    {
        switch (sourceType)
        {
            case SourceType.PlayerHP:
                UpdateHP();
                break;
            case SourceType.Experience:
                UpdateEXP();
                break;
            case SourceType.ScoreOnly:
                // バーもラベルも無いタイプ
                break;
        }

        UpdateScore();
    }

    private void UpdateHP()
    {
        if (player == null) return;

        int current = player.playerHP;
        int max = player.MaxPlayerHP;

        // ラベル
        if (labelText != null) labelText.text = labelPrefix;

        // 数値
        if (valueText != null) valueText.text = FormatValue(current, max);

        // バー
        if (barSlider != null)
        {
            float ratio = (float)current / Mathf.Max(1, max);
            barSlider.value = Mathf.Lerp(barSlider.value, Mathf.Clamp01(ratio), Time.deltaTime * smoothSpeed);
        }
    }

    private void UpdateEXP()
    {
        if (uiManager == null) return;

        int current = uiManager.experiencePoint;
        int max = maxExperience;

        // ラベル（レベル表示モード）
        if (labelText != null)
        {
            if (showLevel)
            {
                int lv = (current / Mathf.Max(1, expPerLevel)) + 1;
                labelText.text = labelPrefix + " · LV " + lv;
            }
            else
            {
                labelText.text = labelPrefix;
            }
        }

        if (valueText != null) valueText.text = FormatValue(current, max);

        if (barSlider != null)
        {
            float ratio = (float)current / Mathf.Max(1, max);
            barSlider.value = Mathf.Lerp(barSlider.value, Mathf.Clamp01(ratio), Time.unscaledDeltaTime * smoothSpeed);
        }
    }

    private void UpdateScore()
    {
        if (scoreText == null || ScoreManager.Instance == null) return;
        scoreText.text = ScoreManager.Instance.CurrentScore.ToString("N0"); // 12,450 形式
    }

    private string FormatValue(int current, int max)
    {
        switch (valueFormat)
        {
            case 1: return current.ToString();
            case 2: return "";
            default: return current + " / " + max;
        }
    }
}
