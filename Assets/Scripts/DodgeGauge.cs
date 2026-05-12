using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのジャスト回避クールタイムを円形ゲージで表示するスクリプト。
/// PlayerManager の状態(_state)を外から監視するだけで、本体には手を加えない。
///
/// Image Type = Filled / Fill Method = Radial 360 のImageを参照する。
/// fillAmount を 0〜1 で滑らかに動かす。
/// </summary>
public class DodgeGauge : MonoBehaviour
{
    [SerializeField, Header("プレイヤー参照")] private PlayerManager player;
    [SerializeField, Header("円形ゲージ本体（Filled / Radial360）")] private Image gaugeImage;
    [SerializeField, Header("ゲージの追従速度")] private float smoothSpeed = 12f;

    [Header("===== 色設定 =====")]
    [SerializeField, Header("使用可能な時の色")] private Color readyColor = new Color(0.2f, 1f, 1f, 1f);     // 水色
    [SerializeField, Header("クールタイム中の色")] private Color cooldownColor = new Color(0.6f, 0.6f, 0.6f, 1f); // 灰色
    [SerializeField, Header("回避中の色（無敵時間中）")] private Color dodgingColor = new Color(1f, 0.9f, 0.2f, 1f); // 黄色

    [Header("===== 演出 =====")]
    [SerializeField, Header("使用可能になった時に光らせる")] private bool flashOnReady = true;
    [SerializeField, Header("フラッシュの強さ")] private float flashIntensity = 1.5f;
    [SerializeField, Header("フラッシュの減衰速度")] private float flashFadeSpeed = 4f;

    private float targetFill = 1f;
    private float flashAmount = 0f;
    private bool wasReady = true;

    void Reset()
    {
        // コンポーネント追加時に同オブジェクトのImageを自動セット
        gaugeImage = GetComponent<Image>();
    }

    void Update()
    {
        if (player == null || gaugeImage == null) return;

        // PlayerManagerの状態に応じて目標値を計算
        UpdateTargetFill();

        // 状態によって追従の仕方を変える
        if (player._state == PlayerManager.dodgeState.coolTime)
        {
            // クールタイム中は線形でそのまま反映（時間ベースで進むので追従Lerpすると不自然）
            gaugeImage.fillAmount = targetFill;
        }
        else
        {
            // 回避発動の瞬間や復帰時は滑らかに追従
            gaugeImage.fillAmount = Mathf.MoveTowards(
                gaugeImage.fillAmount,
                targetFill,
                Time.deltaTime * smoothSpeed
            );
        }

        // 色を更新
        UpdateColor();

        // フラッシュ演出（使用可能になった瞬間ピカッと光る）
        UpdateFlash();
    }

    /// <summary>状態に応じて目標fillを設定</summary>
    private void UpdateTargetFill()
    {
        switch (player._state)
        {
            case PlayerManager.dodgeState.None:
                // 使用可能 → 満タン
                targetFill = 1f;
                break;

            case PlayerManager.dodgeState.JustDodge:
            case PlayerManager.dodgeState.dodge:
                // 回避中 → 0
                targetFill = 0f;
                break;

            case PlayerManager.dodgeState.coolTime:
                // クールタイム中 → リフレクションで dodgeTime を取得して進捗を反映
                targetFill = GetCooldownProgress();
                break;
        }
    }

    /// <summary>クールタイムの進捗（0〜1）を取得</summary>
    private float GetCooldownProgress()
    {
        // PlayerManagerの private float dodgeTime / private float coolTime を取り出す
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

    /// <summary>状態に応じた色をセット</summary>
    private void UpdateColor()
    {
        Color baseColor;
        switch (player._state)
        {
            case PlayerManager.dodgeState.None:
                baseColor = readyColor;
                break;
            case PlayerManager.dodgeState.JustDodge:
            case PlayerManager.dodgeState.dodge:
                baseColor = dodgingColor;
                break;
            case PlayerManager.dodgeState.coolTime:
                baseColor = cooldownColor;
                break;
            default:
                baseColor = readyColor;
                break;
        }

        // フラッシュ加算
        if (flashAmount > 0f)
        {
            baseColor += new Color(flashAmount, flashAmount, flashAmount, 0);
        }

        gaugeImage.color = baseColor;
    }

    /// <summary>使用可能になった瞬間に光らせる</summary>
    private void UpdateFlash()
    {
        bool isReadyNow = player._state == PlayerManager.dodgeState.None;

        // クールタイムから復帰した瞬間にフラッシュ発動
        if (flashOnReady && isReadyNow && !wasReady)
        {
            flashAmount = flashIntensity;
        }

        // フラッシュをだんだん減衰
        flashAmount = Mathf.Max(0f, flashAmount - Time.deltaTime * flashFadeSpeed);

        wasReady = isReadyNow;
    }
}
