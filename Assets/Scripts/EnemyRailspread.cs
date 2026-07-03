using System;
using UnityEngine;

public class EnemyRailspread : EnemyAttackBase
{
    [SerializeField, Header("射撃にかかわる変数")]
    private GameObject Bullet;
    [SerializeField, Tooltip("弾を撃つ間隔（秒）")]
    private float coolTime = 0.5f;
    [SerializeField, Tooltip("弾を撃つ力（速さ）")]
    private float enemyBulletForce = 3f;

    [SerializeField, Tooltip("弾を最初に生成する場所")]
    private float offset = 1.5f;
    [Header("プレイヤーの付近(ドーナツ型)")]
    [SerializeField]
    private float rangeMin = 1f;//内側の半径（これより内側には撃たない）
    [SerializeField]
    private float rangeMax = 3f;//外側の半径（これより外側には撃たない）

    [SerializeField, Tooltip("")]

    [NonSerialized] public float shotTimer = 0;

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        ShotPattern();
    }
    public void ShotPattern()
    {
        shotTimer += Time.deltaTime;
        if (shotTimer > coolTime)   //coolTimeごとにoffsetの距離前に生成してenemyBulletForceの力で撃つ。３秒後に各自破壊。
        {
            Vector3 spawnPosition = transform.position + transform.forward * offset;

            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized;
            float randomDistance = UnityEngine.Random.Range(rangeMin, rangeMax);
            Vector3 targetPosition = player.position +new Vector3(randomCircle.x, 0, randomCircle.y)*randomDistance;

            Vector3 direction = (targetPosition - spawnPosition).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            GameObject newBullet = Instantiate(Bullet, spawnPosition, rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

            Vector3 shotDirection = (targetPosition - spawnPosition).normalized;
            Bulletrb.AddForce(-shotDirection * -enemyBulletForce);

            Destroy(newBullet, 3f);
            shotTimer = 0;
        }
    }
}
