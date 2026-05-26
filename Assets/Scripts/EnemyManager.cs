using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public interface IEnemyDamage
{
    public void EnemyDamaged(int damage);
}

public class EnemyManager : MonoBehaviour, IEnemyDamage
{
    //敵のHP
    
    private int enemyHP;
    [SerializeField,Tooltip("nameでGameManagerを探しているため、埋める必要はない。")] 
    private UIManager ui;
    [SerializeField] private GameObject playerBullet;

    [SerializeField] private GameObject exprosion;
    [NonSerialized] public Transform player;
    
    EnemyAttackBase enemyAttackBase;
    Rigidbody rb;
    public IObjectPool<GameObject> MyPool { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        enemyAttackBase = GetComponent<EnemyAttackBase>();
    }
    public void OnReset()
    {
        enemyHP = enemyAttackBase.maxEnemyHP;
        if (ui == null)
        {
            ui = GameObject.Find("GameManager").GetComponent<UIManager>();
        }
        
    }
        
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
            MyPool.Release(this.gameObject);
            return;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーにダメージを与える
        IPlayerDamage damage = other.gameObject.GetComponent<IPlayerDamage>();
        if (damage != null)
        {
            if (enemyAttackBase == null)
            {
                enemyAttackBase = GetComponent<EnemyAttackBase>();
            }

            if (enemyAttackBase != null)
            {
                damage.Damage(enemyAttackBase.attackPower);
            }
            
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
        if (enemyHP <= 0)
        {
            EnemyDie();
        }
            
    }

    private void EnemyDie()
    {
        Instantiate(exprosion, this.transform.position, Quaternion.identity);
        if (ui == null)
        {
            ui = GameObject.Find("GameManager").GetComponent<UIManager>();
        }

        if (ui != null)
        {
            ui.Experience(10);
        }
        
        if(MyPool != null)
        {
            MyPool.Release(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}