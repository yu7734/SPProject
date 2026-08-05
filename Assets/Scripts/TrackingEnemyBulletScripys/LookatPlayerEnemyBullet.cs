using UnityEngine;

public class LookatPlayerEnemyBullet : MonoBehaviour
{
    void Awake()
    {
        GameObject Player = GameObject.FindWithTag(Tags.Player);
        if (Player != null) gameObject.transform.LookAt(Player.transform);
    }
}