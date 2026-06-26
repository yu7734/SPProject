using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ボタンにカーソルを当てた／選択したとき、少し拡大しつつ色を変える演出。
/// ボタン本体（Image があるオブジェクト）にアタッチして使用する。
///
/// 色の演出は2種類から選べる（colorMode）：
///   ・Brighten     … 元の色を明るくして「光った」ように見せる（従来動作）
///   ・FlowGradient … 専用の2色グラデーションが流れる（選択中だけ別の色味になる）
/// どちらのモードでも拡大は常に併存する。非選択時は元の色に戻る。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    public enum ColorMode
    {
        Brighten,      // 元の色を明るくする（従来）
        FlowGradient,  // 専用の2色グラデーションを流す
    }

    [Header("拡大")]
    [Tooltip("カーソルを当てた／選択したときの拡大倍率")]
    [SerializeField] private float hoverScale = 1.1f;

    [Tooltip("クリック中（押下中）の拡大倍率（少し縮ませて押下感を出す）")]
    [SerializeField] private float pressedScale = 1.05f;

    [Tooltip("スケール変化にかける時間（秒）")]
    [SerializeField] private float scaleDuration = 0.15f;

    [Header("色の演出")]
    [Tooltip("色変更を有効にするか。OFFなら色は一切変えない（Sprite Swap併用時など）")]
    [SerializeField] private bool enableColorChange = true;

    [Tooltip("色の演出モード。Brighten=明るく光る / FlowGradient=専用2色のグラデーションが流れる")]
    [SerializeField] private ColorMode colorMode = ColorMode.Brighten;

    [Tooltip("色変化（フェードイン/アウト）にかける時間（秒）")]
    [SerializeField] private float colorDuration = 0.15f;

    [Header("Brighten モード設定")]
    [Tooltip("カーソルを当てたときに掛ける色（HDR 対応。Intensityを上げるとBloomで光る）")]
    [ColorUsage(true, true)]
    [SerializeField] private Color hoverColor = new Color(1.4f, 1.4f, 1.4f, 1f);

    [Header("FlowGradient モード設定")]
    [Tooltip("選択中に流れるグラデーションの色その1")]
    [SerializeField] private Color flowColorA = new Color(0.2f, 1f, 1f, 1f);   // 水色

    [Tooltip("選択中に流れるグラデーションの色その2")]
    [SerializeField] private Color flowColorB = new Color(0.6f, 0.3f, 1f, 1f); // 紫

    [Tooltip("グラデーションが流れる速さ（大きいほど速い）")]
    [SerializeField] private float flowSpeed = 1.5f;

    [Tooltip("ON=選択していなくても常にグラデーションを流す / OFF=選択中だけ流す")]
    [SerializeField] private bool flowAlways = false;

    [Header("対象")]
    [Tooltip("色を変える対象の Image。未指定なら自分の Image を使う")]
    [SerializeField] private Image targetImage;

    [Header("マウスとキー選択の同期")]
    [Tooltip("マウスホバー時にも自動で Selected（キー選択）を切り替える")]
    [SerializeField] private bool syncSelectionOnHover = true;

    [Header("効果音")]
    [Tooltip("選択（カーソル移動/ホバー）したときに音を鳴らす")]
    [SerializeField] private bool playHoverSound = true;
    [Tooltip("クリック（押下）したときに音を鳴らす")]
    [SerializeField] private bool playClickSound = true;

    [Header("選択中だけ表示するオブジェクト")]
    [Tooltip("選択中の時だけ表示するGameObject。複数登録可")]
    [SerializeField] private GameObject[] selectedOnlyObjects;

    private Vector3 originalScale;
    private Color originalColor;
    private RectTransform rectTransform;

    private bool isHovering = false;
    private bool isPressed = false;

    private float currentScaleT = 0f;
    private float currentColorT = 0f;  // 色演出の進行度（0=元の色, 1=演出色）
    private float flowPhase = 0f;      // グラデーションの位相

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
        if (targetImage != null)
        {
            originalColor = targetImage.color;
        }
    }

    private void OnEnable()
    {
        isHovering = false;
        isPressed = false;
        currentScaleT = 0f;
        currentColorT = 0f;
        flowPhase = 0f;
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale;
        }
        if (enableColorChange && targetImage != null)
        {
            targetImage.color = originalColor;
        }

        UpdateSelectedOnlyObjects(false);
    }

    private void UpdateSelectedOnlyObjects(bool show)
    {
        if (selectedOnlyObjects == null) return;

        foreach (GameObject obj in selectedOnlyObjects)
        {
            if (obj != null && obj.activeSelf != show)
            {
                obj.SetActive(show);
            }
        }
    }

    private void Update()
    {
        bool selected = isHovering || isPressed;
        float targetT = selected ? 1f : 0f;
        float dt = Time.unscaledDeltaTime;

        // 拡大の補間
        if (scaleDuration > 0f)
            currentScaleT = Mathf.MoveTowards(currentScaleT, targetT, dt / scaleDuration);
        else
            currentScaleT = targetT;

        // 色演出の進行度の補間
        if (colorDuration > 0f)
            currentColorT = Mathf.MoveTowards(currentColorT, targetT, dt / colorDuration);
        else
            currentColorT = targetT;

        // 拡大の適用（常に有効）
        if (rectTransform != null)
        {
            float scaleGoal = isPressed ? pressedScale : hoverScale;
            float scale = Mathf.LerpUnclamped(1f, scaleGoal, currentScaleT);
            rectTransform.localScale = originalScale * scale;
        }

        // 色の適用（1か所に集約して競合を防ぐ）
        if (enableColorChange && targetImage != null)
        {
            if (colorMode == ColorMode.FlowGradient)
                ApplyFlowGradient(dt, selected);
            else
                ApplyBrighten();
        }
    }

    /// <summary>
    /// Brighten：元の色 ⇄ originalColor*hoverColor を currentColorT で補間（従来動作）
    /// </summary>
    private void ApplyBrighten()
    {
        Color blended = Color.LerpUnclamped(originalColor, originalColor * hoverColor, currentColorT);
        blended.a = originalColor.a;
        targetImage.color = blended;
    }

    /// <summary>
    /// FlowGradient：選択中に flowColorA ⇄ flowColorB のグラデーションを流す。
    /// 非選択時は元の色に戻る（currentColorT でなめらかにブレンド）。
    /// </summary>
    private void ApplyFlowGradient(float dt, bool selected)
    {
        // 位相は流すべきとき（選択中 or flowAlways）だけ進める
        if (flowAlways || selected)
        {
            flowPhase += dt * flowSpeed;
        }

        // 今この瞬間のグラデーション色
        float t = Mathf.Sin(flowPhase) * 0.5f + 0.5f;
        Color flow = Color.Lerp(flowColorA, flowColorB, t);

        // 非選択 → 元の色、選択中 → グラデーション色 を currentColorT でブレンド
        // （flowAlways=ON のときは常にグラデーション色）
        float blendT = flowAlways ? 1f : currentColorT;
        Color result = Color.LerpUnclamped(originalColor, flow, blendT);
        result.a = originalColor.a;
        targetImage.color = result;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        UpdateSelectedOnlyObjects(true);

        if (syncSelectionOnHover && EventSystem.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject != gameObject)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isPressed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;

        if (playClickSound && ButtonSoundManager.Instance != null)
        {
            ButtonSoundManager.Instance.PlayClick();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        isHovering = true;
        UpdateSelectedOnlyObjects(true);

        if (playHoverSound && ButtonSoundManager.Instance != null)
        {
            ButtonSoundManager.Instance.PlayHover();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isHovering = false;
        isPressed = false;
        UpdateSelectedOnlyObjects(false);
    }
}
