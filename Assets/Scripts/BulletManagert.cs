using UnityEngine;

public class BulletManagert : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] public static int bulletPower = 5;
    [Tooltip("ダメージ倍率")]public float bulletDamageRate = 1.0f;
    [Tooltip("倍率計算後に固定値加算")]public int bulletDamageBonus = 0;
    int bulletAttack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //RigidBody���擾���A�v���C���[�̌����Ă�������ɒe���΂�
        rb = GetComponent<Rigidbody>(); 
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
        bulletAttack = (int)(bulletPower * bulletDamageRate) + bulletDamageBonus;
        Debug.Log(bulletAttack);
        if (damage != null)
        {
            damage.EnemyDamaged(bulletAttack);
            Destroy(gameObject);
        }
    }

    //public void EnemyDamage(EnemyManager enemy)
    //{
    //    enemy.EnemyDamaged(bulletPower);
    //}
}
