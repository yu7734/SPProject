using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField, Header("UIManager参照")] private UIManager uiManager;
    [SerializeField, Header("EXPバー Slider")] private Slider expSlider;
    [SerializeField, Header("バーの追従速度")] private float smoothSpeed = 8f;
    [SerializeField, Header("レベルアップに必要な経験値")] private int maxExperience = 100;

    void Update()
    {
        if (uiManager == null || expSlider == null) return;

        // UIManagerから直接経験値を取得
        int currentExp = uiManager.experiencePoint;

        float ratio = (float)currentExp / Mathf.Max(1, maxExperience);
        ratio = Mathf.Clamp01(ratio);

        // ゲーム停止中（アイテム選択中）でも動くように unscaledDeltaTime を使用
        expSlider.value = Mathf.Lerp(expSlider.value, ratio, Time.unscaledDeltaTime * smoothSpeed);
    }
}
