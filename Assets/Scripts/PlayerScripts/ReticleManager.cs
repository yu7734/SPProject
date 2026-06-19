using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レティクル（照準）を「弾が飛んでいく先」に表示するスクリプト。
///
/// このゲームの弾は機体の正面方向（前方）にまっすぐ飛ぶので、
/// 自機の位置から前方へ aimDistance だけ進んだワールド座標を画面に投影し、
/// そこにレティクル Image を表示する。これで「ここに弾が飛ぶ」が一目で分かる。
///
/// 使い方：
///   1. Canvas の下に UI > Image を作る（例：「Reticle」）
///   2. その Image の Source Image にレティクルにしたい画像（Sprite）を入れる
///   3. このスクリプトを適当な GameObject にアタッチ
///   4. reticleImage に手順1で作った Image を割り当てる
///   5. target に自機を割り当てる（未設定なら "Player" タグを自動検索）
///   6. aimDirectionMode で弾の進む向きを選ぶ（既定 = WorldForward。弾は Vector3.forward で飛ぶため）
///
/// ※ Canvas は Render Mode = Screen Space - Overlay を想定。
/// </summary>
public class ReticleManager : MonoBehaviour
{
    public enum AimDirection
    {
        WorldForward,   // ワールドの前方（+Z）。BulletManagert が Vector3.forward で撃つのでこれが既定
        TargetForward,  // 機体の向いている方向（transform.forward）
    }

    [Header("===== レティクル本体 =====")]
    [SerializeField, Tooltip("レティクルとして表示する UI Image。ここに割り当てた Image の Source Image がレティクルになる")]
    private Image reticleImage;

    [Header("===== 照準設定 =====")]
    [SerializeField, Tooltip("照準の基準にする対象（自機）。未設定なら Player タグを自動検索する")]
    private Transform target;

    [SerializeField, Tooltip("弾が飛ぶ向き。WorldForward = 常に奥(+Z)、TargetForward = 機体の向き")]
    private AimDirection aimDirectionMode = AimDirection.WorldForward;

    [SerializeField, Tooltip("自機の前方どれだけ先にレティクルを置くか（弾道上の距離）")]
    private float aimDistance = 30f;

    [SerializeField, Tooltip("メインカメラ。未設定なら Camera.main を使う")]
    private Camera mainCamera;

    [SerializeField, Tooltip("画面上での微調整オフセット（ピクセル）")]
    private Vector2 screenOffset = Vector2.zero;

    [SerializeField, Tooltip("追従の滑らかさ。0 で即時追従、大きいほどゆっくり追いつく")]
    private float smoothTime = 0f;

    [Header("===== 表示設定 =====")]
    [SerializeField, Tooltip("起動時にレティクルを表示するか")]
    private bool showOnStart = true;

    [SerializeField, Tooltip("照準点がカメラの後ろに回り込んだら自動で隠す")]
    private bool hideWhenBehindCamera = true;

    private RectTransform reticleRect;
    private Vector2 currentVelocity;

    void Start()
    {
        if (reticleImage != null)
        {
            reticleRect = reticleImage.rectTransform;
        }

        if (target == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        SetVisible(showOnStart);
    }

    void LateUpdate()
    {
        if (reticleImage == null || reticleRect == null || target == null) return;

        Camera cam = mainCamera != null ? mainCamera : Camera.main;
        if (cam == null) return;

        // 弾が飛ぶ向きを決める
        Vector3 aimDir = (aimDirectionMode == AimDirection.WorldForward)
            ? Vector3.forward
            : target.forward;

        // 自機の前方 aimDistance 先の「弾道上のワールド座標」
        Vector3 aimWorldPos = target.position + aimDir.normalized * aimDistance;

        // それを画面座標に変換
        Vector3 screenPos = cam.WorldToScreenPoint(aimWorldPos);

        // カメラの後ろに回り込んだら隠す
        if (hideWhenBehindCamera && screenPos.z < 0f)
        {
            if (reticleImage.enabled) reticleImage.enabled = false;
            return;
        }
        else if (!reticleImage.enabled && showOnStart)
        {
            reticleImage.enabled = true;
        }

        Vector2 finalPos = new Vector2(screenPos.x, screenPos.y) + screenOffset;

        if (smoothTime > 0f)
        {
            Vector2 current = reticleRect.position;
            reticleRect.position = Vector2.SmoothDamp(current, finalPos, ref currentVelocity, smoothTime);
        }
        else
        {
            reticleRect.position = finalPos;
        }
    }

    /// <summary>
    /// 外部からレティクルの表示／非表示を切り替える
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (reticleImage != null)
        {
            reticleImage.enabled = visible;
        }
    }

    /// <summary>
    /// 外部から照準の基準対象（自機）を差し替える
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// 外部からレティクル画像（Source Image）を差し替える
    /// </summary>
    public void SetReticleSprite(Sprite sprite)
    {
        if (reticleImage != null)
        {
            reticleImage.sprite = sprite;
        }
    }
}
