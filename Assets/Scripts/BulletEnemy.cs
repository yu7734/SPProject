using System;
using UnityEngine;

public class BulletEnemy : EnemyAttackBase
{
    [SerializeField, Header("射撃にかかわる変数")]
    private GameObject Bullet;
    [SerializeField, Tooltip("自機狙いかどうか")]
    private bool isTracking = false;
    [SerializeField, Tooltip("弾を撃つ間隔（秒）")]
    private float coolTime = 0.5f;
    [SerializeField, Tooltip("弾を撃つ力（速さ）")]
    private float enemyBulletForce = 3f;
    
    [SerializeField, Tooltip("弾を最初に生成する場所")]
    private float offset = 1.5f;

    [NonSerialized] public float shotTimer = 0;
    private void Update()
    {
        if (player == null) return;
        ShotPattern();
    }
    public void ShotPattern()
    {
        shotTimer += Time.deltaTime;
        if (shotTimer > coolTime)   //coolTimeごとにoffsetの距離前に生成してその時点のプレイヤーの方向にenemyBulletForceの力で撃つ。３秒後に各自破壊。
        {
            if(isTracking) transform.LookAt(player.transform);
            Vector3 spawnPosition = transform.position + transform.forward * offset;

            GameObject newBullet = Instantiate(Bullet, spawnPosition, transform.rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

            if (isTracking)
            {
                Vector3 shotDirection = (player.position - spawnPosition).normalized;
                Bulletrb.AddForce(-shotDirection * -enemyBulletForce);
            }
            else
            {
                Bulletrb.AddForce(-transform.forward * enemyBulletForce);
            }

            Destroy(newBullet, 3f);
            shotTimer = 0;
        }
    }
}
