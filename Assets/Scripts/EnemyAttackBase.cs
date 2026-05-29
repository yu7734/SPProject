using System;
using UnityEngine;

public class EnemyAttackBase : MonoBehaviour
{
    public int attackPower;
    public int maxEnemyHP;
    [SerializeField,Tooltip("敵が向かってくる速さ")]
    protected float enemyMoveSpeed = 3f;
    [NonSerialized] public Transform player;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
