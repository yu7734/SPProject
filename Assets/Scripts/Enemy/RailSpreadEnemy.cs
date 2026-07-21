using System;
using UnityEngine;

public class RailSpreadEnemy : EnemyAttackBase
{
    [SerializeField, Header("射撃にかかわる変数")]
    private GameObject bullet;

    /// <summary> 弾を撃つ間隔(秒) </summary>
    [SerializeField, Tooltip("弾を撃つ間隔（秒）")]
    private float coolTime;

    /// <summary> 弾を撃つ力(速さ)  </summary>
    [SerializeField, Tooltip("弾を撃つ力。この値が大きいほど弾が早く飛ぶ")]
    private float bulletForce;

    /// <summary> 弾の生成位置のオフセット </summary>
    [SerializeField, Tooltip("敵の位置からの弾の発射位置までの距離のズレ")]
    private float spawnOffset;

    /// <summary> 内側の半径（これより内側には撃たない） </summary>
    [Header("プレイヤーの付近への広がり(ドーナツ型)")]
    [SerializeField,Tooltip("プレイヤーの周りの内側の半径(弾の広がる範囲)")]
    private float minSpreadRange;

    /// <summary> 外側の半径（これより外側には撃たない） </summary>
    [SerializeField,Tooltip("プレイヤーの周りの外側の半径(弾の広がる範囲)")]
    private float maxSpreadRange;

    /// <summary> 射撃専用のタイマー </summary>
    private float shotTimer = 0;

    public override void OnReset()
    {
        base.OnReset();

        shotTimer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        ShotPattern();
    }
    /// <summary> 自機外し射撃にかかわる関数 </summary>
    public void ShotPattern()
    {
        shotTimer += Time.deltaTime;
        if (shotTimer > coolTime)  
        {
            Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;
                
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized;                                      //半径1の円の外周上のランダムな点
            float randomDistance = UnityEngine.Random.Range(minSpreadRange, maxSpreadRange);                            //minSpreadRangeからmaxSpreadRangeまでのランダムな位置
            Vector3 targetPosition = player.position + new Vector3(randomCircle.x, 0, randomCircle.y)*randomDistance;   //プレイヤーの位置を基準に範囲内のランダムな着弾点を割り出したもの

            Vector3 direction = (targetPosition - spawnPosition).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            GameObject newBullet = Instantiate(bullet, spawnPosition, rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

            Vector3 shotDirection = (targetPosition - spawnPosition).normalized;
            Bulletrb.AddForce(-shotDirection * -bulletForce);

            Destroy(newBullet, 3f);                                                                                     //３秒後に各自破壊
            shotTimer = 0;
        }
    }
}
