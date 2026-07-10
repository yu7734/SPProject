using UnityEngine;

public class BulletManagert : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] public static int bulletPower = 5;
    private PlaySEManager playSEManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playSEManager = GetComponent<PlaySEManager>();

    }
    void Start()
    {
        rb.linearVelocity = this.transform.forward * bulletSpeed * Time.fixedDeltaTime;
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


        //if (other.CompareTag("Enemy"))
            playSEManager.PlaySE(0);

        if (damage != null)
        {
            damage.EnemyDamaged(bulletPower);
            //audioSource.PlayOneShot(audioClip);
            Destroy(gameObject);
        }
    }

    //public void EnemyDamage(EnemyManager enemy)
    //{
    //    enemy.EnemyDamaged(bulletPower);
    //}
}
