using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 自分がアクティブになった時に、指定した GameObject（通常は Button）を
/// EventSystem に選択させる汎用コンポーネント。
///
/// 使い方：
///   1. 任意の UI パネル（Pauseメニュー、選択画面、GameOverなど）にこのスクリプトを Add Component
///   2. Inspector の targetButton に、最初にハイライトしたいボタンを Hierarchy からドラッグ
///   3. パネルが SetActive(true) されるたびに、そのボタンが自動で選択状態になる
///
/// パネルが表示され続けている間に別の入力で選択が外れても、
/// autoRestoreWhenNothingSelected を true にしておけば
/// 「何も選択されていない状態」を検出して自動的に復帰させる。
/// </summary>
public class DefaultFocus : MonoBehaviour
{
    [Header("初期選択にしたいオブジェクト（通常はButton）")]
    [SerializeField, Tooltip("このパネルが表示された時に最初にハイライトされる対象。未設定なら何もしない")]
    private GameObject targetButton;

    [Header("挙動オプション")]
    [SerializeField, Tooltip("有効化直後の1フレームだけ待ってからフォーカスをセットする（別UIから遷移して来た時の競合防止）")]
    private bool delayOneFrame = true;

    [SerializeField, Tooltip("表示中に選択が外れた場合、自動的にターゲットに戻す")]
    private bool autoRestoreWhenNothingSelected = false;

    private void OnEnable()
    {
        if (targetButton == null) return;

        if (delayOneFrame)
        {
            // 1フレーム後にセット（他システムのフォーカス操作と衝突しないため）
            StartCoroutine(SetFocusNextFrame());
        }
        else
        {
            ApplyFocus();
        }
    }

    private System.Collections.IEnumerator SetFocusNextFrame()
    {
        // timeScale=0 でも進む Realtime の1フレーム待機
        yield return null;
        ApplyFocus();
    }

    private void Update()
    {
        if (!autoRestoreWhenNothingSelected) return;
        if (targetButton == null) return;
        if (EventSystem.current == null) return;

        // 何も選択されていない状態になったら復帰
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            ApplyFocus();
        }
    }

    // 外部からもフォーカスを再セットしたい時用（任意）
    public void ApplyFocus()
    {
        if (targetButton == null) return;
        if (EventSystem.current == null) return;

        // 前フレームの選択をクリアしてから改めてセット
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(targetButton);
    }

    // 外部から差し替える用（任意）
    public void SetTarget(GameObject newTarget)
    {
        targetButton = newTarget;
        if (isActiveAndEnabled)
        {
            ApplyFocus();
        }
    }
}
