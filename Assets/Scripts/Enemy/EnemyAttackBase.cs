using System;
using UnityEngine;

public class EnemyAttackBase : MonoBehaviour
{
    [Serializable]
    public struct ShotRange
    {
        [SerializeField, Tooltip("弾を撃つ範囲(最小)")]
        public float min;

        [SerializeField, Tooltip("弾を撃つ範囲(最大)")]
        public float max;
    }

    [Serializable]
    public struct MoveSpeed
    {
        [SerializeField, Tooltip("プレイヤー側に移動する速度")]
        public float speed;

        [SerializeField, Tooltip("移動速度の増加までの時間")]
        public float timeUntilIncrease;

        [SerializeField, Tooltip("移動速度の増加間隔")]
        public float intervalIncrease;

        [SerializeField, Tooltip("移動速度の最大値")]
        public float maxSpeed;
    }

    /// <summary> 射撃範囲 </summary>
    [Header("敵が弾を撃つ範囲")]
    [SerializeField, Tooltip("X座標")]
    protected ShotRange rangeX;

    /// <summary> 射撃範囲 </summary>
    [SerializeField, Tooltip("Y座標")]
    protected ShotRange rangeY;

    /// <summary> 近づいたときに弾を撃たなくなる距離 </summary>
    [SerializeField, Tooltip("プレイヤーの手前で弾を撃たなくする距離")]
    protected float fireStopDistance;

    /// <summary> 敵が衝突した場合のダメージ </summary>
    public int collisionDamage;

    /// <summary> 敵の最大HP(時間経過で増加) </summary>
    public int maxEnemyHP;

    [NonSerialized] public Transform player;

    /// <summary> 敵の移動速度(プレイヤー方向に移動する敵にのみ影響する) </summary>
    [SerializeField, Tooltip("敵が向かってくる速さ")]
    protected MoveSpeed approachMoveSpeed;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary> プールが再利用されるたびに実行 </summary>
    public virtual void OnReset()
    {
        GameObject playerObject = GameObject.FindWithTag("Player"); 
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }
}
