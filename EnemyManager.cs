using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [NonSerialized] public Transform player;   //プレイヤーの座標
    Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject playerObject = GameObject.FindWithTag("Player"); //プレイヤーはPlayerタグを使用する想定

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }
}
