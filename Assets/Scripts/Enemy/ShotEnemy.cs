using UnityEngine;

public class ShotEnemy : EnemyAttackBase
{
    [SerializeField, Header("射撃にかかわる変数")]
    private GameObject bullet;

    /// <summary> trueなら自機狙い </summary>
    [SerializeField, Tooltip("自機狙いかどうか")]
    private bool trackingPlayer;

    /// <summary> 弾を撃つ間隔(秒) </summary>
    [SerializeField, Tooltip("弾を撃つ間隔（秒）")]
    private float coolTime;

    /// <summary> 弾を撃つ力(速さ)  </summary>
    [SerializeField, Tooltip("弾を撃つ力。この値が大きいほど弾が早く飛ぶ")]
    private float bulletForce;

    /// <summary> 弾の生成位置のオフセット </summary>
    [SerializeField, Tooltip("敵の位置からの弾の発射位置までの距離のズレ")]
    private float spawnOffset;

    /// <summary> 射撃専用のタイマー </summary>
    private float shotTimer = 0;

    public override void OnReset()
    {
        base.OnReset();

        shotTimer = 0;
    }
    private void Update()
    {
        if (player == null) return;
        transform.position -= Vector3.forward *
            (Mathf.Min(approachMoveSpeed.maxSpeed, approachMoveSpeed.speed + (Time.timeSinceLevelLoad/approachMoveSpeed.timeUntilIncrease * approachMoveSpeed.intervalIncrease)) * 
            Time.deltaTime);

        if ((transform.position.x > rangeX.min && transform.position.x <= rangeX.max) &&        //Xmin < transform.position.x > Xmax
            (transform.position.y > rangeY.min && transform.position.y <= rangeY.max) &&        //Ymin < transform.position.y > Ymax
             transform.position.z > player.position.z + fireStopDistance)                         
            ShotPattern();                                                                      //全て当てはまっていれば弾を撃つ
    }
    /// <summary> 一般的な射撃にかかわる関数 </summary>
    public void ShotPattern()
    {
        shotTimer += Time.deltaTime;
        if (shotTimer > coolTime)   
        {
            Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;

            Vector3 direction = (player.position - spawnPosition).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            GameObject newBullet = Instantiate(bullet, spawnPosition, rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

            if (trackingPlayer)
            {
                Vector3 shotDirection = (player.position - spawnPosition).normalized;
                Bulletrb.AddForce(-shotDirection * -bulletForce);                          //その時点のプレイヤーの方向に撃つ。
            }
            else
            {
                Bulletrb.AddForce(-transform.forward * bulletForce);
            }

            Destroy(newBullet, 3f);                                                             //３秒後に各自破壊
            shotTimer = 0;
        }
    }
}
