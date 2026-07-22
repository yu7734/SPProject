using UnityEngine;

[RequireComponent(typeof(EnemyManager))]
[RequireComponent(typeof(LineRenderer))]
public class AimingLaserEnemy : EnemyAttackBase
{
    [SerializeField, Header("照準レーザーにかかわる変数")]
    private GameObject laser;
    [SerializeField, Tooltip("初期停止時間(秒)")]
    private float stopTime;

    /// <summary> 照準を画面上に表示させる時間 </summary>
    [SerializeField, Tooltip("照準時間")]
    private float aimingTime;

    /// <summary> レーザーを撃っている時間 </summary>
    [SerializeField, Tooltip("レーザーを撃つ時間")]
    private float laserShotTime;

    /// <summary> 弾を撃つ力(速さ)  </summary>
    [SerializeField, Tooltip("弾を撃つ力。この値が大きいほど弾が早く飛ぶ")]
    private float bulletForce;

    /// <summary> 弾を撃つ間隔(秒) </summary>
    [SerializeField, Tooltip("弾を撃つ間隔")]
    private float laserInterval;

    /// <summary> 弾の生成位置のオフセット </summary>
    [SerializeField, Tooltip("敵の位置からの弾の発射位置までの距離のズレ")]
    private float spawnOffset;

    [Header("レーザーの色設定")]
    [SerializeField, Tooltip("最初の色")]
    private Color startColor = Color.blue; //青
    [SerializeField, Tooltip("最後の色")]
    private Color endColor = Color.red; // 赤

    /// <summary> 通常のタイマー </summary>
    private float timer;

    /// <summary> レーザー専用のタイマー </summary>
    private float laserTimer;

    /// <summary> チャージする時間 </summary>
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
    /// <summary> 照準 </summary>
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
    /// <summary> レーザーの射撃にかかわる関数 </summary>
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
        Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;

        GameObject newBullet = Instantiate(laser, spawnPosition, transform.rotation);
        Debug.Log("レーザー敵が弾を生成: " + newBullet.name);
        Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();
        Bulletrb.AddForce(transform.forward * bulletForce);
        Destroy(newBullet, 3f);

        if (laserTimer > laserShotTime)
        {
            timer = 0;
            laserTimer = 0;
            chargeTime = 0;
            return;
        }
    }
    /// <summary> 照準の描画 </summary>
    void AimingRenderer()
    {
        lineRenderer.enabled = true;
        Vector3 targetPos = player.position;
        targetPos.z -= 10;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPos);

        float elapsedAimingTime = timer - stopTime;
        float ratio = Mathf.Clamp01(elapsedAimingTime / aimingTime);        // 照準が始まってからの経過時間を計算 (0.0 ～ 1.0 に正規化)

        Color currentColor = Color.Lerp(startColor, endColor, ratio);       // 時間の割合（ratio）に応じて色を補間

        lineRenderer.startColor = currentColor;                             // LineRendererに色を適用
        lineRenderer.endColor = currentColor;

    }
}

