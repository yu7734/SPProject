using System;
using UnityEngine;

public interface IEnemyDamage
{
    public void EnemyDamaged(int damage);
}

public class EnemyManager : MonoBehaviour, IEnemyDamage
{
    //ダメージ量
    [SerializeField] private int attackPower;

    //敵のHP
    [SerializeField] private int enemyHP;
    [SerializeField,Tooltip("nameでGameManagerを探しているため、埋める必要はない。")] 
    private UIManager ui;
    [SerializeField] private GameObject playerBullet;

    [SerializeField] private GameObject exprosion;
    [NonSerialized] public Transform player;

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObject = GameObject.FindWithTag("Player"); //プレイヤーはPlayerタグを使用する想定
        ui = GameObject.Find("GameManager").GetComponent<UIManager>();
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (player == null) return;


        if (transform.position.z <= -9.7f)
        {
            Destroy(this.gameObject);
        }

        EnemyDie();
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーにダメージを与える
        IPlayerDamage damage = other.gameObject.GetComponent<IPlayerDamage>();
        if (damage != null)
        {
            damage.Damage(attackPower);
            Debug.Log("hit");
        }
        //プレイヤーの弾に触れたら
        //if (other.gameObject.CompareTag("PlayerBullet"))
        //{
        //    //ダメージを受ける
        //    //playerBullet.GetComponent<BulletManagert>().EnemyDamage(this);

        //    Debug.Log("hit");
        //    //Destroy(playerBullet);
        //}
    }

    //プレイヤーに与えるダメージ量
    //public void PlayerDamage(PlayerManager player)
    //{
    //    player.Damage(attackPower);
    //}

    //敵が受けるダメージ量
    public void EnemyDamaged(int damage)
    {
        enemyHP -= Mathf.Max(0, damage);
    }

    private void EnemyDie()
    {
        if (enemyHP <= 0)
        {
            ui.Experience(10);
            Instantiate(exprosion, this.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}