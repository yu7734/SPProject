using UnityEngine;

[RequireComponent(typeof(EnemyManager))]
[RequireComponent(typeof(LineRenderer))]
public class AimingLaserEnemy : EnemyAttackBase
{
    [SerializeField, Header("照準レーザーにかかわる変数")]
    private GameObject Laser;
    [SerializeField, Tooltip("初期停止時間")]
    private float stopTime;
    [SerializeField, Tooltip("照準時間")]
    private float aimingTime;
    [SerializeField, Tooltip("レーザーを撃つ時間")]
    private float laserTimeLimit;
    [SerializeField, Tooltip("弾を撃つ力（速さ）")]
    private float enemyBulletForce;
    [SerializeField, Tooltip("弾を撃つ間隔")]
    private float laserInterval;
    [SerializeField, Tooltip("弾を最初に生成する場所")]
    private float offset;

    [Header("レーザーの色設定")]
    [SerializeField, Tooltip("最初の色")]
    private Color startColor = Color.blue; //青
    [SerializeField, Tooltip("最後の色")]
    private Color endColor = Color.red; // 赤

    private float timer;
    private float laserTimer;
    float chargeTime;
    private LineRenderer lineRenderer;

    public override void OnReset()
    {
        base.OnReset();

        timer = 0;
        laserTimer = 0;
        chargeTime = 0;
        if (lineRenderer == null) { 
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = startColor;
        }
    }
    private void Start()
    {
        if(!lineRenderer)lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        
    }
    // Update is called once per frame
    private void Update()
    {
        Aiming();

        //Debug.Log(laserTimer);
    }
    public void Aiming()
    {
        timer += Time.deltaTime;

        if (timer < stopTime)
        {
            transform.LookAt(player.transform);
            return;
        }
        LaserPatturn();

    }
    public void LaserPatturn()
    {
        if (timer < stopTime + aimingTime && timer > stopTime)
        {
            transform.LookAt(player.transform);
            AimingRenderer();            
            return;
        }

        lineRenderer.enabled = false;
        laserTimer += Time.deltaTime;

        chargeTime += Time.deltaTime;
        if (chargeTime < laserInterval) return;

        chargeTime -= laserInterval;
        Vector3 spawnPosition = transform.position + transform.forward * offset;

        GameObject newBullet = Instantiate(Laser, spawnPosition, transform.rotation);
        Debug.Log("レーザー敵が弾を生成: " + newBullet.name);
        Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();
        Bulletrb.AddForce(transform.forward * enemyBulletForce);
        Destroy(newBullet, 3f);

        if (laserTimer > laserTimeLimit)
        {
            timer = 0;
            laserTimer = 0;
            chargeTime = 0;
            return;
        }
    }
    void AimingRenderer()
    {
        lineRenderer.enabled = true;
        Vector3 targetPos = player.position;
        targetPos.z -= 10;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPos);

        // 照準が始まってからの経過時間を計算 (0.0 ～ 1.0 に正規化)
        float elapsedAimingTime = timer - stopTime;
        float ratio = Mathf.Clamp01(elapsedAimingTime / aimingTime);

        // 時間の割合（ratio）に応じて色を補間
        Color currentColor = Color.Lerp(startColor, endColor, ratio);

        // LineRendererに色を適用
        lineRenderer.startColor = currentColor;
        lineRenderer.endColor = currentColor;

    }
}

