using System;
using UnityEngine;

public class TrackBulletEnemy : EnemyManager
{
    [SerializeField,Header("射撃にかかわる変数")]
    private GameObject Bullet;
    [SerializeField, Tooltip("弾を撃つ間隔（秒）")]
    private float coolTime = 0.5f;
    [SerializeField, Tooltip("弾を撃つ力（速さ）")]
    private float enemyBulletForce = 3f;
    [SerializeField, Tooltip("弾を最初に生成する場所")]
    private float offset = 1.5f;

    [NonSerialized] public float trackShotTimer = 0;
    private void Update()
    {
        if (player == null) return; 
        TrackShotPattern();
    }
    public void TrackShotPattern()
    {
        //if (!isRush) transform.LookAt(player.transform); //プレイヤー側に常に向く
        trackShotTimer += Time.deltaTime;
        if (trackShotTimer > coolTime)   //coolTimeごとにoffsetの距離前に生成してその時点のプレイヤーの方向にenemyBulletForceの力で撃つ。３秒後に各自破壊。
        {
            Vector3 spawnPosition = transform.position + transform.forward * offset;

            GameObject newBullet = Instantiate(Bullet, spawnPosition, transform.rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

            Vector3 shotDirection = (player.position - spawnPosition).normalized;

            Bulletrb.AddForce(shotDirection * enemyBulletForce);
            Destroy(newBullet, 3f);
            trackShotTimer = 0;
        }
    }
}
