using UnityEngine;

public class RushEnemy : EnemyAttackBase
{

    [Header("突撃にかかわる変数")]

    [SerializeField, Tooltip("初期停止時間（秒）")]
    private float stopTime;

    [SerializeField, Tooltip("プレイヤーを見ている時間（秒)")]
    private float lookAtPlayerTime;

    /// <summary> 突撃専用のタイマー </summary>
    private float rushTimer;

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
    /// <summary> 一般的な突撃にかかわる関数 </summary>
    public void RushPattern()
    {
        rushTimer += Time.deltaTime;
        if (rushTimer < stopTime) return;
        if (rushTimer < lookAtPlayerTime)
        {
            transform.LookAt(player.transform);                                                     //lookAtTimeになるまでプレイヤーの座標に向けて移動
        }
        transform.position += transform.forward *
            (Mathf.Min(approachMoveSpeed.maxSpeed, approachMoveSpeed.speed + (Time.timeSinceLevelLoad / approachMoveSpeed.timeUntilIncrease * approachMoveSpeed.intervalIncrease)) *
            Time.deltaTime);                                                                        //その時向いていた方向に移動。

        //Debug.Log(approachMoveSpeed.speed + (Time.timeSinceLevelLoad / approachMoveSpeed.timeUntilIncrease * approachMoveSpeed.intervalIncrease));

    }
}
