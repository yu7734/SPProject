using UnityEngine;

public class BulletManagert : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] public static int bulletPower = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //RigidBody‚ğæ“¾‚µA‘O•û‚É”ò‚Î‚·
        rb = GetComponent<Rigidbody>(); 
        rb.AddForce(Vector3.forward * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 2f);
    }

    public void EnemyDamage(EnemyManager2 enemy)
    {
        enemy.EnemyDamaged(bulletPower);
    }
}
