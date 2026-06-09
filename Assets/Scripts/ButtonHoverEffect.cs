using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ボタンにカーソルを当てたとき、少し拡大しつつ色を明るくして「光った」ように見せる演出。
/// ボタン本体（Image があるオブジェクト）にアタッチして使用する。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("拡大")]
    [Tooltip("カーソルを当てたときの拡大倍率")]
    [SerializeField] private float hoverScale = 1.1f;

    [Tooltip("クリック中（押下中）の拡大倍率（少し縮ませて押下感を出す）")]
    [SerializeField] private float pressedScale = 1.05f;

    [Tooltip("スケール変化にかける時間（秒）")]
    [SerializeField] private float scaleDuration = 0.15f;

    [Header("発光（色を明るくする）")]
    [Tooltip("色変更を有効にする。Sprite Swap と併用したいときは OFF にする（Sprite変更を邪魔しない）")]
    [SerializeField] private bool enableColorChange = true;

    [Tooltip("カーソルを当てたときに掛ける色（HDR 対応。Intensity を上げると Bloom で光る）")]
    [ColorUsage(true, true)] // (showAlpha, hdr) → HDR ピッカーを有効化
    [SerializeField] private Color hoverColor = new Color(1.4f, 1.4f, 1.4f, 1f);

    [Tooltip("色変化にかける時間（秒）")]
    [SerializeField] private float colorDuration = 0.15f;

    [Header("対象")]
    [Tooltip("色を変える対象の Image。未指定なら自分の Image を使う")]
    [SerializeField] private Image targetImage;

    [Header("マウスとキー選択の同期")]
    [Tooltip("マウスホバー時にも自動で Selected（キー選択）を切り替える。複数のボタンが同時に光るのを防ぐ")]
    [SerializeField] private bool syncSelectionOnHover = true;

    [Header("効果音")]
    [Tooltip("選択（カーソル移動/ホバー）したときに ButtonSoundManager を呼んで音を鳴らす")]
    [SerializeField] private bool playHoverSound = true;
    [Tooltip("クリック（押下）したときに ButtonSoundManager を呼んで音を鳴らす")]
    [SerializeField] private bool playClickSound = true;

    // 元のスケール・元の色を保存しておく
    private Vector3 originalScale;
    private Color originalColor;
    private RectTransform rectTransform;

    // ホバー中かどうか
    private bool isHovering = false;
    // 押下中かどうか
    private bool isPressed = false;

    // アニメーション用の進行度
    private float currentScaleT = 0f;
    private float currentColorT = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        // ターゲットImageが未指定なら自分から取得
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
        // 有効化されたタイミングで状態をリセット（ポーズ復帰時などに変な状態にならないように）
        isHovering = false;
        isPressed = false;
        currentScaleT = 0f;
        currentColorT = 0f;
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale;
        }
        // 色変更を有効にしている場合だけ、初期色に戻す
        if (enableColorChange && targetImage != null)
        {
            targetImage.color = originalColor;
        }
    }

    private void Update()
    {
        // 目標値（ホバー中/押下中ならアニメーションを進める、そうでないなら戻す）
        float targetT = (isHovering || isPressed) ? 1f : 0f;

        // timeScale=0（ポーズ中）でも動くように unscaledDeltaTime を使う
        float dt = Time.unscaledDeltaTime;

        // スケールの補間
        if (scaleDuration > 0f)
        {
            currentScaleT = Mathf.MoveTowards(currentScaleT, targetT, dt / scaleDuration);
        }
        else
        {
            currentScaleT = targetT;
        }

        // 色の補間
        if (colorDuration > 0f)
        {
            currentColorT = Mathf.MoveTowards(currentColorT, targetT, dt / colorDuration);
        }
        else
        {
            currentColorT = targetT;
        }

        // 適用
        if (rectTransform != null)
        {
            // 押下中は pressedScale、それ以外（ホバー中）は hoverScale を目標にする
            float scaleGoal = isPressed ? pressedScale : hoverScale;
            float scale = Mathf.LerpUnclamped(1f, scaleGoal, currentScaleT);
            rectTransform.localScale = originalScale * scale;
        }

        // 色変更が有効な場合だけ Color を書き換える
        // ※OFFにすると Sprite Swap などの他の色変更機能と競合しなくなる
        if (enableColorChange && targetImage != null)
        {
            // 色を originalColor と hoverColor の間で補間
            // hoverColor は乗算的に使う（1より大きければ明るく光る）
            Color blended = Color.LerpUnclamped(originalColor, originalColor * hoverColor, currentColorT);
            // アルファは元のまま保持
            blended.a = originalColor.a;
            targetImage.color = blended;
        }
    }

    // ===== マウス（ポインター）イベント =====
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        // マウスホバー時にも EventSystem の Selected を自分にして、
        // キー選択中の他のボタンと「選択中扱い」が重複しないようにする。
        // これで Selected Sprite の青枠が常に1つのボタンにだけ出るようになる。
        if (syncSelectionOnHover && EventSystem.current != null)
        {
            // 既に自分が選択中なら呼ばない（無駄な OnSelect/OnDeselect 発火を避ける）
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

        // クリック音を鳴らす
        if (playClickSound && ButtonSoundManager.Instance != null)
        {
            ButtonSoundManager.Instance.PlayClick();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    // ===== キーボード/ゲームパッドでのナビゲーション選択イベント =====
    public void OnSelect(BaseEventData eventData)
    {
        // 方向キーやゲームパッドで選択されたときもホバー扱い
        isHovering = true;

        // 選択音を鳴らす
        // ※ syncSelectionOnHover=true ならマウスホバー時にも EventSystem.SetSelectedGameObject
        //    が呼ばれて OnSelect が発火するので、マウス・キー両方この1か所で音が鳴る
        if (playHoverSound && ButtonSoundManager.Instance != null)
        {
            ButtonSoundManager.Instance.PlayHover();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isHovering = false;
        isPressed = false;
    }
}
