using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyManager))]
public class EnemyAimingLaser : EnemyAttackBase
{
    [SerializeField,Header("照準レーザーにかかわる変数")]
    private GameObject Laser;
    [SerializeField, Tooltip("初期停止時間")]
    private float stopTime;
    [SerializeField, Tooltip("照準時間")]
    private float aimingTime;
    [SerializeField, Tooltip("レーザーを撃つ時間")]
    private float laserTimeLimit;
    [SerializeField, Tooltip("弾を撃つ力（速さ）")]
    private float enemyBulletForce;
    [SerializeField, Tooltip("弾を最初に生成する場所")]
    private float offset;

    private float timer;
    private float laserTimer;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }
    // Update is called once per frame
    private void Update()
    {
        Aiming();
        
        Debug.Log(laserTimer);
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

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.position);

            return;
        }
        float chargeTime = 0;
        chargeTime += Time.deltaTime;
        if (chargeTime < 1) return;

        laserTimer += Time.deltaTime;
        lineRenderer.enabled = false;
        Vector3 spawnPosition = transform.position + transform.forward * offset;

        GameObject newBullet = Instantiate(Laser, spawnPosition, transform.rotation);
        Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

        Bulletrb.AddForce(-transform.forward * -enemyBulletForce);
        Destroy(newBullet, 3f);

        if (laserTimer > laserTimeLimit)
        {
            timer = 0;
            laserTimer = 0;
        }
    }
    
}
