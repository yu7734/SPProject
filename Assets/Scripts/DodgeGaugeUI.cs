using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 左下のDODGE円形ゲージ用。
/// 既存のDodgeGauge.csとは別に、スクショのデザインに合わせて
/// 「中央に数値表示」「下にラベル」「色のシンプル化」を行ったバージョン。
/// </summary>
public class DodgeGaugeUI : MonoBehaviour
{
    [SerializeField, Header("プレイヤー参照")] private PlayerManager player;
    [SerializeField, Header("円形ゲージ本体（Filled / Radial360）")] private Image gaugeImage;
    [SerializeField, Header("中央の数値テキスト")] private TextMeshProUGUI valueText;
    [SerializeField, Header("下のラベルテキスト（DODGE）")] private TextMeshProUGUI labelText;

    [Header("===== 色設定 =====")]
    [SerializeField] private Color readyColor = new Color(0.3f, 0.7f, 1f, 1f);     // 青
    [SerializeField] private Color cooldownColor = new Color(0.3f, 0.7f, 1f, 0.5f); // 半透明青
    [SerializeField] private Color dodgingColor = new Color(1f, 0.9f, 0.2f, 1f);    // 黄

    [Header("===== 表示 =====")]
    [SerializeField] private string labelString = "DODGE";
    [SerializeField] private string readyString = "READY";

    void Reset()
    {
        gaugeImage = GetComponent<Image>();
    }

    void Update()
    {
        if (player == null || gaugeImage == null) return;

        UpdateGauge();
        UpdateText();
        UpdateColor();
    }

    private void UpdateGauge()
    {
        switch (player._state)
        {
            case PlayerManager.dodgeState.None:
                gaugeImage.fillAmount = 1f;
                break;

            case PlayerManager.dodgeState.JustDodge:
            case PlayerManager.dodgeState.dodge:
                gaugeImage.fillAmount = 0f;
                break;

            case PlayerManager.dodgeState.coolTime:
                gaugeImage.fillAmount = GetCooldownProgress();
                break;
        }
    }

    private void UpdateText()
    {
        if (labelText != null) labelText.text = labelString;
        if (valueText == null) return;

        switch (player._state)
        {
            case PlayerManager.dodgeState.None:
                valueText.text = readyString;
                break;

            case PlayerManager.dodgeState.JustDodge:
            case PlayerManager.dodgeState.dodge:
                valueText.text = "0.0";
                break;

            case PlayerManager.dodgeState.coolTime:
                // 残り時間を表示
                float remaining = GetRemainingTime();
                valueText.text = remaining.ToString("F1");
                break;
        }
    }

    private void UpdateColor()
    {
        switch (player._state)
        {
            case PlayerManager.dodgeState.None:
                gaugeImage.color = readyColor;
                break;
            case PlayerManager.dodgeState.JustDodge:
            case PlayerManager.dodgeState.dodge:
                gaugeImage.color = dodgingColor;
                break;
            case PlayerManager.dodgeState.coolTime:
                gaugeImage.color = cooldownColor;
                break;
        }
    }

    private float GetCooldownProgress()
    {
        var t = typeof(PlayerManager);
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        var dodgeTimeField = t.GetField("dodgeTime", flags);
        var coolTimeField = t.GetField("coolTime", flags);
        if (dodgeTimeField == null || coolTimeField == null) return 0f;
        float dodgeTime = (float)dodgeTimeField.GetValue(player);
        float coolTime = (float)coolTimeField.GetValue(player);
        if (coolTime <= 0f) return 1f;
        return Mathf.Clamp01(dodgeTime / coolTime);
    }

    private float GetRemainingTime()
    {
        var t = typeof(PlayerManager);
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        var dodgeTimeField = t.GetField("dodgeTime", flags);
        var coolTimeField = t.GetField("coolTime", flags);
        if (dodgeTimeField == null || coolTimeField == null) return 0f;
        float dodgeTime = (float)dodgeTimeField.GetValue(player);
        float coolTime = (float)coolTimeField.GetValue(player);
        return Mathf.Max(0f, coolTime - dodgeTime);
    }
}
