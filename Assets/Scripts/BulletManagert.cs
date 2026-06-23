using UnityEngine;

public class BulletManagert : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] public static int bulletPower = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //RigidBodyを取得し、前方に飛ばす
        rb = GetComponent<Rigidbody>(); 
        rb.AddForce(Vector3.forward * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(this.tag)) return;
        IEnemyDamage damage = other.gameObject.GetComponentInParent<IEnemyDamage>();

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
