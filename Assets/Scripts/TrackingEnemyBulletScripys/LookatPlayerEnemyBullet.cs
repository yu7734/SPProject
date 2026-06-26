using UnityEngine;

public class LookatPlayerEnemyBullet : MonoBehaviour
{
    void Awake()
    {
        gameObject.transform.LookAt(GameObject.FindWithTag("Player").transform);
    }
}