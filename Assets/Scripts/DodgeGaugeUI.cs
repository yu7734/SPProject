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
                // 回避中：dodgetime（PlayerManagerの内部時間）に応じて 1 → 0 へ減る
                gaugeImage.fillAmount = 1f - GetDodgeProgress();
                break;

            case PlayerManager.dodgeState.coolTime:
                // クールタイム中：時間経過に合わせて 0 → 1 へ回復
                gaugeImage.fillAmount = GetCooldownProgress();
                break;
        }
    }

    // 回避中の進捗（0〜1）。PlayerManager の dodgetime / 1秒（=回避時間）から算出
    private float GetDodgeProgress()
    {
        var t = typeof(PlayerManager);
        var instanceFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        // PlayerManager の実際の変数名は dodgetime（全部小文字）
        var dodgetimeField = t.GetField("dodgetime", instanceFlags);
        if (dodgetimeField == null) return 0f;

        float dodgetime = (float)dodgetimeField.GetValue(player);
        // PlayerManager 側は 1 秒経過で coolTime ステートに切り替わる
        return Mathf.Clamp01(dodgetime / 1f);
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
                // 回避中：残り回避時間を表示（1秒から減っていく）
                float dodgeRemain = Mathf.Max(0f, 1f - GetDodgeProgress());
                valueText.text = dodgeRemain.ToString("F1");
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
        // PlayerManager 側の実際の変数名は dodgeCoolTime（public）と coolTime（private）
        var t = typeof(PlayerManager);
        var instanceFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        var dodgeCoolTimeField = t.GetField("dodgeCoolTime", instanceFlags);
        var coolTimeField = t.GetField("coolTime", instanceFlags);

        if (dodgeCoolTimeField == null || coolTimeField == null) return 0f;

        float dodgeCoolTime = (float)dodgeCoolTimeField.GetValue(player);
        float coolTime = (float)coolTimeField.GetValue(player);

        if (coolTime <= 0f) return 1f;
        // 経過時間 / 全体時間 → 0 から 1 へ徐々に増える（ゲージが回復していくイメージ）
        return Mathf.Clamp01(dodgeCoolTime / coolTime);
    }

    private float GetRemainingTime()
    {
        var t = typeof(PlayerManager);
        var instanceFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        var dodgeCoolTimeField = t.GetField("dodgeCoolTime", instanceFlags);
        var coolTimeField = t.GetField("coolTime", instanceFlags);

        if (dodgeCoolTimeField == null || coolTimeField == null) return 0f;

        float dodgeCoolTime = (float)dodgeCoolTimeField.GetValue(player);
        float coolTime = (float)coolTimeField.GetValue(player);

        // 残り時間 = 全体時間 - 経過時間
        return Mathf.Max(0f, coolTime - dodgeCoolTime);
    }
}
