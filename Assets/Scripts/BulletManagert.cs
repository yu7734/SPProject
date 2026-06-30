using UnityEngine;

public class BulletManagert : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] public static int bulletPower = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //RigidBodyを取得し、プレイヤーの向いている方向に弾を飛ばす
        rb = GetComponent<Rigidbody>(); 
        rb.linearVelocity = this.transform.forward * bulletSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(this.tag)) return;
        IEnemyDamage damage = other.gameObject.GetComponent<IEnemyDamage>();

        if (damage != null)
        {
            damage.EnemyDamaged(bulletPower);
            Destroy(gameObject);
        }
    }

    //public void EnemyDamage(EnemyManager enemy)
    //{
    //    enemy.EnemyDamaged(bulletPower);
    //}
}
