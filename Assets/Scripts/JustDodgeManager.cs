using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class JustDodgeManager : MonoBehaviour
{
    [SerializeField] private UIManager ui;

    [SerializeField] private PlayerManager player;
    private float slowTime = 0;
    private bool bslow = false;

    [Header("===== ジャスト回避テキスト演出 =====")]
    [SerializeField, Tooltip("ジャスト回避成功時に表示するテキスト")] private TextMeshProUGUI justText;
    [SerializeField, Tooltip("表示する文字列")] private string justString = "JUST!";
    [SerializeField, Tooltip("テキストが表示される時間（秒）")] private float justTextDuration = 0.8f;
    [SerializeField, Tooltip("フェードアウトにかける時間（秒）")] private float justTextFadeDuration = 0.4f;
    [SerializeField, Tooltip("表示時にぴょこっと拡大する倍率")] private float justTextPopScale = 1.3f;

    [Header("===== 表示位置 =====")]
    [SerializeField, Tooltip("プレイヤーの位置から見て、画面上どれだけ上にずらすか（ピクセル）")] private Vector2 justTextScreenOffset = new Vector2(0f, 80f);
    [SerializeField, Tooltip("表示中、徐々に上に浮き上がる移動量（ピクセル）")] private float justTextFloatUp = 40f;
    [SerializeField, Tooltip("メインカメラ。未設定なら Camera.main を使う")] private Camera mainCamera;

    [Header("===== EXP+10 ポップアップ演出 =====")]
    [SerializeField, Tooltip("EXP獲得時に出すポップアップテキスト（JUST!とは別に用意）")] private TextMeshProUGUI expPopupText;
    [SerializeField, Tooltip("獲得EXP表示のフォーマット（{0}が獲得量）")] private string expPopupFormat = "+{0} EXP";
    [SerializeField, Tooltip("EXPポップアップの表示時間（秒）")] private float expPopupDuration = 1.0f;
    [SerializeField, Tooltip("EXPポップアップのフェードアウト時間（秒）")] private float expPopupFadeDuration = 0.5f;
    [SerializeField, Tooltip("EXPポップアップの出現位置オフセット（JUST!の下に出すなら -50 など）")] private Vector2 expPopupScreenOffset = new Vector2(0f, 30f);
    [SerializeField, Tooltip("EXPポップアップがフワッと上に浮き上がる距離（ピクセル）")] private float expPopupFloatUp = 60f;
    [SerializeField, Tooltip("EXPポップアップの登場時ポップサイズ倍率")] private float expPopupPopScale = 1.2f;

    // 演出用の内部状態
    private float justTextTimer = 0f;
    private bool isShowingJustText = false;
    private Color justTextOriginalColor;
    private Vector3 justTextOriginalScale;
    // ジャスト発動時のプレイヤー位置（その場で固定したい場合に使う）
    private Vector3 justTextWorldAnchor;

    // EXPポップアップ用の内部状態
    private float expPopupTimer = 0f;
    private bool isShowingExpPopup = false;
    private Color expPopupOriginalColor;
    private Vector3 expPopupOriginalScale;
    private int lastGainedExp = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // テキストの初期状態を保存して、最初は非表示にしておく
        if (justText != null)
        {
            justTextOriginalColor = justText.color;
            justTextOriginalScale = justText.rectTransform.localScale;

            // 起動時は完全に透明にしておく
            Color c = justTextOriginalColor;
            c.a = 0f;
            justText.color = c;

            // GameObject ごと無効化（Raycastや他UIの邪魔をしないように）
            justText.gameObject.SetActive(false);
        }

        // EXPポップアップも同様に初期化
        if (expPopupText != null)
        {
            expPopupOriginalColor = expPopupText.color;
            expPopupOriginalScale = expPopupText.rectTransform.localScale;

            Color c = expPopupOriginalColor;
            c.a = 0f;
            expPopupText.color = c;

            // GameObject ごと無効化
            expPopupText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�X���[���o
        if (bslow)
        {
            Time.timeScale = 0.2f;
            slowTime += Time.deltaTime;
            if (slowTime >= 1f)
            {
                bslow = false;
                slowTime = 0f;
                Time.timeScale = 1f;
            }
        }

        // ジャスト回避テキストの演出（フェードアウト＆少しずつ縮小）
        if (isShowingJustText && justText != null)
        {
            justTextTimer += Time.unscaledDeltaTime; // スロー中でも進むように unscaledDeltaTime

            float alpha;
            float scale;

            if (justTextTimer < justTextDuration - justTextFadeDuration)
            {
                // 表示中（フェード前）：ポップから等倍に戻す
                float t = Mathf.Clamp01(justTextTimer / 0.15f);
                scale = Mathf.Lerp(justTextPopScale, 1f, t);
                alpha = 1f;
            }
            else if (justTextTimer < justTextDuration)
            {
                // フェードアウト中
                float fadeT = (justTextTimer - (justTextDuration - justTextFadeDuration)) / justTextFadeDuration;
                alpha = Mathf.Lerp(1f, 0f, fadeT);
                scale = 1f;
            }
            else
            {
                // 終了
                alpha = 0f;
                scale = 1f;
                isShowingJustText = false;

                // GameObject ごと非表示にして他UIのRaycastを邪魔しないようにする
                justText.gameObject.SetActive(false);
            }

            // 色のアルファだけ書き換える（元の色味は維持）
            Color c = justTextOriginalColor;
            c.a = alpha;
            justText.color = c;

            // スケール反映
            justText.rectTransform.localScale = justTextOriginalScale * scale;

            // プレイヤーの位置に追従させる（少しずつ上に浮き上がる）
            // ※非表示にしてしまっているフレームでは無駄になるのでスキップ
            if (justText.gameObject.activeSelf)
            {
                UpdateJustTextPosition();
            }
        }

        // EXPポップアップの演出
        if (isShowingExpPopup && expPopupText != null)
        {
            expPopupTimer += Time.unscaledDeltaTime;

            float alpha;
            float scale;

            if (expPopupTimer < expPopupDuration - expPopupFadeDuration)
            {
                // ポップから等倍に戻す
                float t = Mathf.Clamp01(expPopupTimer / 0.15f);
                scale = Mathf.Lerp(expPopupPopScale, 1f, t);
                alpha = 1f;
            }
            else if (expPopupTimer < expPopupDuration)
            {
                // フェードアウト
                float fadeT = (expPopupTimer - (expPopupDuration - expPopupFadeDuration)) / expPopupFadeDuration;
                alpha = Mathf.Lerp(1f, 0f, fadeT);
                scale = 1f;
            }
            else
            {
                alpha = 0f;
                scale = 1f;
                isShowingExpPopup = false;

                // GameObject ごと非表示にして他UIのRaycastを邪魔しないようにする
                expPopupText.gameObject.SetActive(false);
            }

            Color c = expPopupOriginalColor;
            c.a = alpha;
            expPopupText.color = c;

            expPopupText.rectTransform.localScale = expPopupOriginalScale * scale;

            // 位置をプレイヤーに追従＋上にフロート
            // ※非表示にしてしまっているフレームでは無駄になるのでスキップ
            if (expPopupText.gameObject.activeSelf)
            {
                UpdateExpPopupPosition();
            }
        }
    }

    /// <summary>
    /// プレイヤーのワールド座標を画面座標に変換して、テキストの位置を更新する。
    /// 経過時間に応じて少しずつ上に浮き上がる演出も入れる。
    /// </summary>
    private void UpdateJustTextPosition()
    {
        if (justText == null || player == null) return;

        Camera cam = mainCamera != null ? mainCamera : Camera.main;
        if (cam == null) return;

        // プレイヤー位置を画面座標に変換
        Vector3 screenPos = cam.WorldToScreenPoint(player.transform.position);

        // カメラの後ろに回り込んだ場合は表示しない
        if (screenPos.z < 0f)
        {
            Color c = justText.color;
            c.a = 0f;
            justText.color = c;
            return;
        }

        // 経過時間に応じて少しずつ上にフロート
        float t = Mathf.Clamp01(justTextTimer / justTextDuration);
        Vector2 floatOffset = new Vector2(0f, justTextFloatUp * t);

        // 画面座標 + オフセット を最終位置として反映
        Vector2 finalScreenPos = new Vector2(screenPos.x, screenPos.y) + justTextScreenOffset + floatOffset;
        justText.rectTransform.position = finalScreenPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && (player.bJustDodge == true))
        {
            player.bJustDodge = false;
            Debug.Log("Just");

            const int gainedExp = 10;
            ui.Experience(gainedExp);

            // ジャスト回避テキストを表示
            ShowJustText();

            // EXPポップアップを表示
            ShowExpPopup(gainedExp);
        }
    }

    /// <summary>
    /// ジャスト回避成功時にテキストをぴょこっと表示してフェードアウトさせる
    /// </summary>
    private void ShowJustText()
    {
        if (justText == null) return;

        // 表示開始：GameObject を有効化
        justText.gameObject.SetActive(true);

        justText.text = justString;
        justTextTimer = 0f;
        isShowingJustText = true;

        // 発動時のプレイヤー位置を記録（必要に応じて固定したい場合用）
        if (player != null)
        {
            justTextWorldAnchor = player.transform.position;
        }

        // 初期状態：完全表示＆ポップサイズ
        Color c = justTextOriginalColor;
        c.a = 1f;
        justText.color = c;
        justText.rectTransform.localScale = justTextOriginalScale * justTextPopScale;

        // 出現位置をプレイヤー近くに合わせる
        UpdateJustTextPosition();
    }

    /// <summary>
    /// EXP獲得時にポップアップ表示する
    /// </summary>
    private void ShowExpPopup(int gainedExp)
    {
        if (expPopupText == null) return;

        // 表示開始：GameObject を有効化
        expPopupText.gameObject.SetActive(true);

        lastGainedExp = gainedExp;
        expPopupText.text = string.Format(expPopupFormat, gainedExp);
        expPopupTimer = 0f;
        isShowingExpPopup = true;

        // 初期：完全表示＆ポップサイズ
        Color c = expPopupOriginalColor;
        c.a = 1f;
        expPopupText.color = c;
        expPopupText.rectTransform.localScale = expPopupOriginalScale * expPopupPopScale;

        UpdateExpPopupPosition();
    }

    /// <summary>
    /// EXPポップアップの位置をプレイヤーの画面位置に合わせて更新
    /// </summary>
    private void UpdateExpPopupPosition()
    {
        if (expPopupText == null || player == null) return;

        Camera cam = mainCamera != null ? mainCamera : Camera.main;
        if (cam == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(player.transform.position);

        if (screenPos.z < 0f)
        {
            Color c = expPopupText.color;
            c.a = 0f;
            expPopupText.color = c;
            return;
        }

        // 時間経過で上にフロート
        float t = Mathf.Clamp01(expPopupTimer / expPopupDuration);
        Vector2 floatOffset = new Vector2(0f, expPopupFloatUp * t);

        Vector2 finalScreenPos = new Vector2(screenPos.x, screenPos.y) + expPopupScreenOffset + floatOffset;
        expPopupText.rectTransform.position = finalScreenPos;
    }

    //public void JustDodge()
    //{
    //    justDodgeTime += Time.deltaTime;
    //    if (justDodgeTime >= 0.1f)
    //    {
    //        justDodgeTime = 0;
    //        player._state = PlayerManager.dodgeState.dodge;
    //    }
    //}
}
