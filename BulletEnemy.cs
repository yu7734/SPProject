using System;
using UnityEngine;

public class BulletEnemy : EnemyManager
{
    [Header("射撃にかかわる変数")]
    public GameObject Bullet;
    [Tooltip("弾を撃つ間隔（秒）")]
    public float coolTime = 0.5f;
    [Tooltip("弾を撃つ力（速さ）")]
    public float enemyBulletForce = 3f;
    
    [Tooltip("弾を最初に生成する場所")]
    public float offset = 1.5f;

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
            Vector3 spawnPosition = transform.position + transform.forward * offset;

            GameObject newBullet = Instantiate(Bullet, spawnPosition, transform.rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();
            Bulletrb.AddForce(transform.forward * enemyBulletForce);
            Destroy(newBullet, 3f);
            shotTimer = 0;
        }
    }
}
