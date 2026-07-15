using System;
using UnityEngine;

public class EnemyAttackBase : MonoBehaviour
{
    [Serializable]
    public struct ShotRange
    {
        public float min;
        public float max;
    }
    [Header("敵が弾を撃つ範囲")]

    [SerializeField, Tooltip("弾を撃つ範囲。X座標のminからmaxまで")]
    protected ShotRange rangeX;
    [SerializeField, Tooltip("弾を撃つ範囲。Y座標のminからmaxまで")]
    protected ShotRange rangeY;
    [SerializeField, Tooltip("プレイヤーの手前で弾を撃たなくする")]
    protected float playerDistance;

    public int attackPower;
    public int maxEnemyHP;
    [NonSerialized] public Transform player;
    [SerializeField, Tooltip("敵が向かってくる速さ")]
    protected float enemyMoveSpeed = 3f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnReset()
    {
        GameObject playerObject = GameObject.FindWithTag("Player"); //プレイヤーはPlayerタグを使用する想定

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }
}
