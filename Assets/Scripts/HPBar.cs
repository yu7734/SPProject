using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField, Header("プレイヤー参照")] private PlayerManager player;
    [SerializeField, Header("HPバー Slider")] private Slider hpSlider;
    [SerializeField, Header("バーの色を変えるFill Image")] private Image hpFillImage;
    [SerializeField, Header("バーの追従速度")] private float smoothSpeed = 5f;

    [SerializeField, Header("色を体力で変える")] private bool changeColor = true;
    [SerializeField] private Color highColor = Color.green;
    [SerializeField] private Color midColor = Color.yellow;
    [SerializeField] private Color lowColor = Color.red;

    void Update()
    {
        if (player == null || hpSlider == null) return;

        // 0〜1の割合に変換
        float ratio = (float)player.playerHP / Mathf.Max(1, player.MaxPlayerHP);
        ratio = Mathf.Clamp01(ratio);

        // ぬるっと追従
        hpSlider.value = Mathf.Lerp(hpSlider.value, ratio, Time.deltaTime * smoothSpeed);

        // 残量で色を変える
        if (changeColor && hpFillImage != null)
        {
            if (ratio > 0.6f)       hpFillImage.color = highColor;
            else if (ratio > 0.3f)  hpFillImage.color = midColor;
            else                    hpFillImage.color = lowColor;
        }
    }
}
