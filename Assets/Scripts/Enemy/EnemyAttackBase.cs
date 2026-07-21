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

    /// <summary> 敵の移動速度 </summary>
    [SerializeField, Tooltip("敵が向かってくる速さ")]
    protected float moveSpeed;

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
