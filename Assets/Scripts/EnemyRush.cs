using System;
using UnityEngine;

[RequireComponent(typeof(EnemyManager))]
public class EnemyRush : EnemyAttackBase
{
    [Header("突撃にかかわる変数")]
    [SerializeField,Tooltip("最初の停止時間（秒）")]
    private float stopTime = 1;
    [SerializeField, Tooltip("プレイヤーを見ている時間（秒）")]
    private float lookAtTime = 2;
    [SerializeField, Tooltip("敵の突撃速度")]
    private float enemyRushSpeed = 3f;

    private float rushTimer = 0;

    private void Update()
    {

        if (player == null) return;
        RushPattern();
    }
    public override void OnReset()
    {
        base.OnReset();
        rushTimer = 0;
    }
    /// <summary>
    /// rushTimer &lt; lookAtTimeでプレイヤーの座標に向けて移動し、 &gt; lookAtTimeではその時向いていた方向に直線的に移動
    /// </summary>
    public void RushPattern()  //lookAtTimeになるまでプレイヤーの座標に向けて移動し、それ以降はその時向いていた方向に直線的に移動。プレイヤーZが手前に移動したとき非表示。
    {
        rushTimer += Time.deltaTime;
        if (rushTimer < stopTime) return;
        if (rushTimer < lookAtTime)
        {
            transform.LookAt(player.transform);
        }
        transform.position += transform.forward * enemyRushSpeed * Time.deltaTime;

    }
}
