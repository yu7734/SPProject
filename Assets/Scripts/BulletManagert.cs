using UnityEngine;


public class BulletManagert : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] public static int bulletPower = 5;
    /// <summary>初期化用の変数なので攻撃力をいじる場合はインスペクターでBASEを
    /// scriptでbulletDamageRateを変更してください</summary>
    [Tooltip("ダメージ倍率")]public float BASE_bulletDamageRate = 1.0f;
    [HideInInspector] public float bulletDamageRate = 1.0f;
    [Tooltip("倍率計算後に固定値加算")]public int bulletDamageBonus = 0;
    int bulletAttack;
    //[SerializeField, Tooltip("命中した時のSEオブジェクト")] private GameObject hitSE;
    private SoundManager soundManager;
    [SerializeField, Tooltip("命中した時のSEオブジェクト")] private AudioClip hitSE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //RigidBody���擾���A�v���C���[�̌����Ă�������ɒe���΂�
        rb = GetComponent<Rigidbody>();
        soundManager = FindAnyObjectByType<SoundManager>();
        rb.linearVelocity = this.transform.forward * bulletSpeed * Time.fixedDeltaTime;
        bulletAttack = (int)(bulletPower * bulletDamageRate) + bulletDamageBonus;
        Debug.Log(bulletAttack);
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

        if (other.gameObject.CompareTag("Enemy"))
            soundManager.Play(hitSE);

        if (damage != null)
        {
            damage.EnemyDamaged(bulletAttack);
            Destroy(gameObject);
        }
    }
}
