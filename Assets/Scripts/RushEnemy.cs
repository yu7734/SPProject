using System;
using UnityEngine;

public class RushEnemy : EnemyAttackBase
{
    [Serializable]
    public class ShotSetting
    {
        [Header("射撃にかかわる変数")]
        public GameObject Bullet;
        [Tooltip("自機狙いかどうか")]
        public bool isTracking = false;
        [Tooltip("弾を撃つ間隔（秒）")]
        public float coolTime = 0.5f;
        [Tooltip("弾を撃つ力（速さ）")]
        public float enemyBulletForce = 3f;

        [Tooltip("弾を最初に生成する場所")]
        public float offset = 1.5f;
    }

    [Header("突撃にかかわる変数")]
    [SerializeField,Tooltip("最初の停止時間（秒）")]
    private float stopTime = 1;
    [SerializeField, Tooltip("プレイヤーを見ている時間（秒）")]
    private float lookAtTime = 2;
    [SerializeField,Tooltip("射撃するかどうか")]
    private bool canShot = false;

    private float rushTimer = 0;
    [NonSerialized] public float shotTimer = 0;

    [SerializeField] private ShotSetting shot = new();

    private void Update()
    {

        if (player == null) return;
        RushPattern();
        if(canShot)ShotPattern();
    }
    public override void OnReset()
    {
        base.OnReset();
        rushTimer = 0;
        if (canShot) shotTimer = 0;
    }
    public void RushPattern()  //lookAtTimeになるまでプレイヤーの座標に向けて移動し、それ以降はその時向いていた方向に直線的に移動。プレイヤーZが手前に移動したとき非表示。
    {
        rushTimer += Time.deltaTime;
        if (rushTimer < stopTime) return;
        if (rushTimer < lookAtTime)
        {
            transform.LookAt(player.transform);
        }
        transform.position += transform.forward * enemyMoveSpeed * Time.deltaTime;

    }
    public void ShotPattern() {
        shotTimer += Time.deltaTime;
        if (shotTimer > shot.coolTime)   //coolTimeごとにoffsetの距離前に生成してその時点のプレイヤーの方向にenemyBulletForceの力で撃つ。３秒後に各自破壊。
        {
            Vector3 spawnPosition = transform.position + transform.forward * shot.offset;

            GameObject newBullet = Instantiate(shot.Bullet, spawnPosition, transform.rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

            if (shot.isTracking)
            {
                Vector3 shotDirection = (player.position - spawnPosition).normalized;
                Bulletrb.AddForce(-shotDirection * -shot.enemyBulletForce);
            }
            else
            {
                Bulletrb.AddForce(-transform.forward * shot.enemyBulletForce);
            }

            Destroy(newBullet, 3f);
            shotTimer = 0;
        }
    }
}
