using System;
using UnityEngine;

public class RushEnemy : EnemyManager
{
    [Header("突撃にかかわる変数")]
    [Tooltip("最初の停止時間（秒）")]
    public float stopTime = 1;
    [Tooltip("プレイヤーを見ている時間（秒）")]
    public float lookAtTime = 2;
    [Tooltip("敵の突撃速度")]
    public float enemyRushSpeed = 3f;
    [Tooltip("敵をプレイヤー通過後に非表示にするまでの距離")]
    public float RushOffset = 10f;

    [NonSerialized] public float rushTimer = 0;
    public bool isRush = true;

    private void Update()
    {
        if (player == null) return;
        RushPattern();
    }
    public void RushPattern()  //lookAtTimeになるまでプレイヤーの座標に向けて移動し、それ以降はその時向いていた方向に直線的に移動。プレイヤーZが手前に移動したとき非表示。
    {
        rushTimer += Time.deltaTime;
        if (rushTimer < stopTime) return;
        if (rushTimer < lookAtTime)
        {
            transform.LookAt(player.transform);
        }
        transform.position += transform.forward * enemyRushSpeed * Time.deltaTime;

        if (transform.position.z < player.position.z + -RushOffset)
        {
            gameObject.SetActive(false);
        }
    }
}
