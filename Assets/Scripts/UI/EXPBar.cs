using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EXPBar : MonoBehaviour
{
    [SerializeField, Header("UIManager参照")] private UIManager uiManager;
    [SerializeField, Header("EXPバー Slider")] private Slider expSlider;
    [SerializeField, Header("EXP数値テキスト（例：100/100）")] private TextMeshProUGUI expText;
    [SerializeField, Header("レベル表示テキスト（例：Level - 1）")] private TextMeshProUGUI levelText;
    [SerializeField, Header("レベル表示のフォーマット（{0}が現在レベル）")] private string levelFormat = "Level - {0}";
    [SerializeField, Header("バーの追従速度")] private float smoothSpeed = 8f;
    //[SerializeField, Header("レベルアップに必要な経験値")] private int maxExperience = 100;

    void Update()
    {
        if (uiManager == null || expSlider == null) return;

        // UIManagerから直接経験値を取得
        int currentExp = uiManager.experiencePoint;
        int maxExp = uiManager.maxExprrience;

        float ratio = (float)currentExp / Mathf.Max(1, maxExp);
        ratio = Mathf.Clamp01(ratio);

        // ゲーム停止中（アイテム選択中）でも動くように unscaledDeltaTime を使用
        expSlider.value = Mathf.Lerp(expSlider.value, ratio, Time.unscaledDeltaTime * smoothSpeed);

        // EXPの数値を「現在EXP/最大EXP」の形式で表示
        if (expText != null)
        {
            expText.text = $"{Mathf.Clamp(currentExp, 0, maxExp)}/{maxExp}";
        }

        // レベル表示
        if (levelText != null)
        {
            levelText.text = string.Format(levelFormat, uiManager.level);
        }
    }
}
