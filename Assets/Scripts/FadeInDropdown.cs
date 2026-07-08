using System.Collections;
using UnityEngine;

/// <summary>
/// 自分がアクティブになった時に「上から降ってきてフェードイン」するアニメーションを行うコンポーネント。
///
/// 特徴:
///   - CanvasGroup を使ってフェード（無ければ自動で AddComponent される）
///   - アニメ中は blocksRaycasts=false / interactable=false でクリック不可
///   - Time.timeScale = 0 でも動く（unscaledDeltaTime を使用）
///   - 派手すぎない dropDistance と duration を Inspector で調整可能
///
/// 使い方:
///   1. アニメさせたい UI パネル（例: SerectItemImage）や個別ボタンにこのスクリプトを AddComponent
///   2. Inspector で Duration / Drop Distance を好みの値に
///   3. パネルが SetActive(true) されるたびに自動でアニメする
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class FadeInDropdown : MonoBehaviour
{
    [Header("=== アニメ設定 ===")]
    [SerializeField, Tooltip("フェードインにかかる秒数（unscaled）")]
    private float duration = 0.3f;

    [SerializeField, Tooltip("どれだけ上から降ってくるか（ピクセル/ワールド単位、対象のスケールに依存）")]
    private float dropDistance = 60f;

    [SerializeField, Tooltip("アニメーションのイージング（滑らかさ）")]
    private EaseType easeType = EaseType.OutCubic;

    public enum EaseType
    {
        Linear,
        OutCubic,   // 最初速く、最後ゆっくり（一般的なUIでよく使う）
        OutQuad,    // ゆるく減速
        OutBack,    // 少しオーバーシュートしてから戻る（跳ねる感じ）
    }

    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 finalAnchoredPos;
    private bool finalPosCaptured = false;
    private Coroutine currentAnim;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();

        // Awake の時点での位置を「本来の位置」として1回だけ記憶する。
        // Prefab やシーン配置時の値なので、以降のアニメで位置がズレていても常にここへ戻る。
        if (rect != null)
        {
            finalAnchoredPos = rect.anchoredPosition;
            finalPosCaptured = true;
        }
    }

    private void OnEnable()
    {
        // すでにアニメ中なら止めて最初からやり直す
        if (currentAnim != null)
        {
            StopCoroutine(currentAnim);
        }
        currentAnim = StartCoroutine(PlayDropdown());
    }

    private IEnumerator PlayDropdown()
    {
        // RectTransform が無いオブジェクトには非対応（3Dオブジェクト用途は想定外）
        if (rect == null)
        {
            yield break;
        }

        // Awake で保存した本来の位置を使う。取れていなければ現在位置をフォールバックとして使う。
        if (!finalPosCaptured)
        {
            finalAnchoredPos = rect.anchoredPosition;
            finalPosCaptured = true;
        }
        Vector2 startPos = finalAnchoredPos + new Vector2(0f, dropDistance);

        // 開始状態: 上に移動 + 透明 + クリック不可
        rect.anchoredPosition = startPos;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        // duration が 0 以下なら即完了
        if (duration <= 0f)
        {
            FinishInstantly();
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // timeScale=0 の間も進めるため unscaledDeltaTime
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = ApplyEase(t, easeType);

            rect.anchoredPosition = Vector2.LerpUnclamped(startPos, finalAnchoredPos, eased);
            canvasGroup.alpha = eased;
            yield return null;
        }

        FinishInstantly();
    }

    private void FinishInstantly()
    {
        rect.anchoredPosition = finalAnchoredPos;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        currentAnim = null;
    }

    // イージング関数
    private static float ApplyEase(float t, EaseType type)
    {
        switch (type)
        {
            case EaseType.OutCubic:
                return 1f - Mathf.Pow(1f - t, 3f);
            case EaseType.OutQuad:
                return 1f - (1f - t) * (1f - t);
            case EaseType.OutBack:
                {
                    const float c1 = 1.70158f;
                    const float c3 = c1 + 1f;
                    return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                }
            case EaseType.Linear:
            default:
                return t;
        }
    }

    // 外部から手動で再生したい時用
    public void Replay()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(PlayDropdown());
    }

    // レイアウト変更などで本来の位置が変わった時に、現在位置を新しい「本来の位置」として再取得する
    public void RecaptureFinalPosition()
    {
        if (rect == null) return;
        finalAnchoredPos = rect.anchoredPosition;
        finalPosCaptured = true;
    }
}
