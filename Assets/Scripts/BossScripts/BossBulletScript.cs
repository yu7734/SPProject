using UnityEngine;

public class BossBulletScript : MonoBehaviour
{
    [SerializeField]float bulletSpeed=50f;
    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * bulletSpeed;
    }
}
