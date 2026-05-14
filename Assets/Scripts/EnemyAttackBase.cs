using System;
using UnityEngine;

public class EnemyAttackBase : MonoBehaviour
{
    [NonSerialized] public Transform player;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObject = GameObject.FindWithTag("Player"); //プレイヤーはPlayerタグを使用する想定

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }
}
