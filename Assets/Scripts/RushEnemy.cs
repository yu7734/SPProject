using System;
using UnityEngine;

public class RushEnemy : EnemyAttackBase
{

    [Header("突撃にかかわる変数")]
    [SerializeField, Tooltip("最初の停止時間（秒）")]
    private float stopTime;
    [SerializeField, Tooltip("プレイヤーを見ている時間（秒）")]
    private float lookAtTime;
    [SerializeField, Tooltip("射撃するかどうか")]
    private bool canShot = false;

    private float rushTimer;
    [NonSerialized] public float shotTimer;

    private void Update()
    {

        if (player == null) return;
        RushPattern(); 
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
}
