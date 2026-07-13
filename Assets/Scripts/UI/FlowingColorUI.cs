using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UIの色を「右から左」へ流してループさせる演出スクリプト。
/// 指定した2色（colorA / colorB）の帯が滑らかに流れる。
///
/// 対応UI（同じGameObjectに付いているものを自動判別）：
///   ・TextMeshProUGUI … 文字ごとに色をずらして帯が流れる
///   ・Image           … 単色が colorA ⇄ colorB を行き来して流れる
///
/// 使い方：
///   1. 色を流したいUI（TextやImage）に、このスクリプトをアタッチ
///   2. colorA / colorB に好きな2色を設定
///   3. speed で流れる速さ、waveTightness で帯の細かさを調整
/// </summary>
public class FlowingColorUI : MonoBehaviour
{
    [Header("===== 色設定 =====")]
    [SerializeField, Tooltip("流れる色その1")]
    private Color colorA = new Color(0.2f, 1f, 1f, 1f);   // 水色

    [SerializeField, Tooltip("流れる色その2")]
    private Color colorB = new Color(0.6f, 0.3f, 1f, 1f); // 紫

    [Header("===== 流れ方 =====")]
    [SerializeField, Tooltip("流れる速さ（大きいほど速い）")]
    private float speed = 1.5f;

    [SerializeField, Tooltip("帯の細かさ。大きいほど色の波が細かく並ぶ（Textのみ影響）")]
    private float waveTightness = 0.35f;

    [SerializeField, Tooltip("不透明度はUI元の値を保つ（OFFなら色のアルファを使う）")]
    private bool keepOriginalAlpha = true;

    // 内部参照
    private TextMeshProUGUI tmp;
    private Graphic graphic;     // Image など
    private float phase = 0f;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null)
            graphic = GetComponent<Graphic>(); // Image, RawImage 等
    }

    void Update()
    {
        // 右から左へ流すので位相を増やしていく
        phase += Time.unscaledDeltaTime * speed;

        if (tmp != null)
            UpdateText();
        else if (graphic != null)
            UpdateGraphic();
    }

    /// <summary>
    /// TextMeshPro：文字ごとに位相をずらして「帯が左へ流れる」見た目を作る
    /// </summary>
    private void UpdateText()
    {
        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;
        int charCount = textInfo.characterCount;
        if (charCount == 0) return;

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            // 右の文字ほど位相を進める → 時間で位相を引くと帯が右→左へ動く
            float t = Mathf.Sin((i * waveTightness * Mathf.PI) - phase) * 0.5f + 0.5f;
            Color32 col = Color.Lerp(colorA, colorB, t);

            if (keepOriginalAlpha)
                col.a = tmp.color.a < 1f ? (byte)(tmp.color.a * 255) : (byte)255;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            var colors = textInfo.meshInfo[matIndex].colors32;
            colors[vertIndex + 0] = col;
            colors[vertIndex + 1] = col;
            colors[vertIndex + 2] = col;
            colors[vertIndex + 3] = col;
        }

        // 反映
        for (int m = 0; m < textInfo.meshInfo.Length; m++)
        {
            var meshInfo = textInfo.meshInfo[m];
            meshInfo.mesh.colors32 = meshInfo.colors32;
            tmp.UpdateGeometry(meshInfo.mesh, m);
        }
    }

    /// <summary>
    /// Image など：colorA ⇄ colorB を時間で行き来させて流れている雰囲気を出す
    /// </summary>
    private void UpdateGraphic()
    {
        float t = Mathf.Sin(phase) * 0.5f + 0.5f;
        Color col = Color.Lerp(colorA, colorB, t);

        if (keepOriginalAlpha)
            col.a = graphic.color.a;

        graphic.color = col;
    }
}
