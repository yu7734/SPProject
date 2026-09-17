using UnityEngine;

public class BossAttackScript : MonoBehaviour
{
    enum BossPattern 
    { 
        Idol,
        Cannon=1,
        Laser=2,
        Gatling=3
    }
    enum Aiming 
    { 
        ON,
        OFF
    }
    [SerializeField] GameObject CannonBallet, LaserBallet, GatlingBallet;
    [SerializeField] GameObject[] GatlingShotPoints;
    [SerializeField] GameObject[] CannonShotPoints;
    LineRenderer lineRenderer;
    Transform player;
    float timer = 0f;
    float timer1 = 0f;
    BossPattern pattern=BossPattern.Idol;
    [SerializeField, Tooltip("パターン遷移時間")] float transitionTime = 5;

    [Header("Laserパターン"), SerializeField, Tooltip("狙う時間")] float aimingTime = 3f;
    [SerializeField, Tooltip("撃ち終わるまでの時間")] float stopTime = 3f;
    [SerializeField, Tooltip("最初の色")] Color startColor = Color.blue; //青
    [SerializeField, Tooltip("変化する色")] Color endColor = Color.red; // 赤
    [Header("Gatlingパターン"), SerializeField, Tooltip("ブレ")] Vector2 Renge = new(10f, 10f);
    [SerializeField, Tooltip("撃ち続ける時間")] float continuationTime = 3f;
    [SerializeField,Tooltip("弾と弾の間隔")] float Interval = 0.2f;
    [SerializeField,Tooltip("自機狙いか")]Aiming aiming = Aiming.ON;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.Find(GameObjectName.Player).transform;
    }

    private void Update()
    {
        switch (pattern) 
        { 
            case BossPattern.Idol:
                timer += Time.deltaTime;
                if (timer >= transitionTime) 
                {
                    pattern = (BossPattern)Random.Range(1, 4);
                    timer = 0;
                }
                break;
            case BossPattern.Cannon:
                BossShot(CannonBallet, CannonShotPoints);
                pattern=BossPattern.Idol;
                break;
            case BossPattern.Laser:
                timer += Time.deltaTime;
                if (timer < aimingTime) AimingRenderer(lineRenderer);
                else if (timer > stopTime + aimingTime)
                {
                    timer = 0;
                    pattern = BossPattern.Idol;
                }
                else
                {
                    lineRenderer.enabled = false;
                    BossShot(LaserBallet, CannonShotPoints);
                }
                break;
            case BossPattern.Gatling:
                timer += Time.deltaTime;
                timer1 += Time.deltaTime;
                if (timer > Interval)
                {
                    BossRandomShot(GatlingBallet, GatlingShotPoints);
                    timer = 0;
                }
                if (timer1 > continuationTime)
                {
                    timer = 0;
                    timer1 = 0;
                    pattern =BossPattern.Idol;
                }
                break;
        }
    }

    void BossShot(GameObject Ballet,GameObject[] ShotPoints) 
    {
        for (int i = 0; i < ShotPoints.Length; i++)
        {
            Instantiate(Ballet, ShotPoints[i].transform.position, ShotPoints[i].transform.rotation);
        }
    }
    void BossRandomShot(GameObject Ballet, GameObject[] ShotPoints)
    {
        for (int i = 0; i < ShotPoints.Length; i++)
        {
            float x = Random.Range(-Renge.x, Renge.x);
            float y = Random.Range(-Renge.y, Renge.y);
            switch (aiming)
            {
                case Aiming.ON:
                    ShotPoints[i].transform.LookAt(player);
                    break;
                case Aiming.OFF:
                    ShotPoints[i].transform.rotation = ShotPoints[i].transform.parent.rotation;
                    break;
            }
            GameObject GetBallet=Instantiate(Ballet, ShotPoints[i].transform.position, Quaternion.Euler(x + ShotPoints[i].transform.eulerAngles.x, y + ShotPoints[i].transform.eulerAngles.y, 0f));
            Destroy(GetBallet, 3f);
        }
    }

    void AimingRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.enabled = true;
        Vector3 targetPos = player.position;
        targetPos.z -= 10;
        Vector3 RendererPos = new(transform.position.x, transform.position.y+3, transform.position.z);
        lineRenderer.SetPosition(0, RendererPos);
        lineRenderer.SetPosition(1, targetPos);

        float elapsedAimingTime = timer;// - stopTime;
        float ratio = Mathf.Clamp01(elapsedAimingTime / aimingTime);        // 照準が始まってからの経過時間を計算 (0.0 ～ 1.0 に正規化)

        Color currentColor = Color.Lerp(startColor, endColor, ratio);       // 時間の割合（ratio）に応じて色を補間

        lineRenderer.startColor = currentColor;                             // LineRendererに色を適用
        lineRenderer.endColor = currentColor;

    }
}
