using UnityEngine;

public class EnemyManager2 : MonoBehaviour
{
    public PlayerManager player;
    //ダメージ量
    [SerializeField] private int attackPower;
    //敵の移動スピード
    [SerializeField] private float enemySpeed;
    //敵のHP
    [SerializeField] private int enemyHP;
    [SerializeField] private GameObject playerBullet;

    [SerializeField] private UIManager ui;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnemyDie();
    }

    private void FixedUpdate()
    {
        //敵が前に進み続ける
        transform.position -= Vector3.forward * enemySpeed * Time.deltaTime;
    }

    //プレイヤーに与えるダメージ量
    public void PlayerDamage(PlayerManager player)
    {
        player.Damage(attackPower);
    }

    //敵が受けるダメージ量
    public void EnemyDamaged(int damage)
    {
        enemyHP -= Mathf.Max(0, damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーの弾に触れたら
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            //ダメージを受ける
            playerBullet.GetComponent<BulletManagert>().EnemyDamage(this);

            Debug.Log("hit");
            //Destroy(playerBullet);
        }
    }

    //敵が死んだら
    private void EnemyDie()
    {
        if (enemyHP <= 0)
        {
            ui.Experience(10);
            Destroy(gameObject);
        }
    }
}
