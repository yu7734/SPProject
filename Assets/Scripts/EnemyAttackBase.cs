using System;
using UnityEngine;

public class EnemyAttackBase : MonoBehaviour
{
    public int attackPower;
    public int maxEnemyHP;
    [NonSerialized] public Transform player;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //予備
        GameObject playerObject = GameObject.FindWithTag("Player"); //プレイヤーはPlayerタグを使用する想定

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }
    /// <summary>
    /// 初期設定。FindWithTagや値のリセット。
    /// </summary>
    public virtual void OnReset()
    {
        GameObject playerObject = GameObject.FindWithTag("Player"); //プレイヤーはPlayerタグを使用する想定

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

}
