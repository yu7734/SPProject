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
    [SerializeField]
    private UIManager ui;
    [SerializeField] private GameObject playerBullet;

    [SerializeField] private GameObject exprosion;
    [NonSerialized] public Transform player;

    EnemyAttackBase enemyAttackBase;
    Rigidbody rb;
    private bool isReleased = false;
    public IObjectPool<GameObject> MyPool { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        enemyAttackBase = GetComponent<EnemyAttackBase>();
    }
    public void OnReset()
    {
        enemyHP = enemyAttackBase.maxEnemyHP;
        isReleased = false;
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
        if (isReleased) return;

        if (transform.position.z <= -9.7f)
        {
            MyPool.Release(this.gameObject);
            return;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (isReleased) return;
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
                EnemyDie();
            }

            Debug.Log("hit");
        }
    }
    //敵が受けるダメージ量
    public void EnemyDamaged(int damage)
    {
        if (isReleased) return;
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

        if (MyPool != null)
        {
            MyPool.Release(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

