using System;
using UnityEngine;
using UnityEngine.Pool;

public interface IEnemyDamage
{
    public void EnemyDamaged(int damage);
}

public class EnemyManager : MonoBehaviour, IEnemyDamage
{
    /// <summary> 敵のHP </summary>
    private int enemyHP;
    [SerializeField,Tooltip("一定時間毎のHPの増加量")]
    private int hpGrowthRate;
    [SerializeField, Tooltip("HP増加までの時間")]
    private float timeUntilIncrease;
    [SerializeField, Tooltip("HPの増加量の最大値(制限時間や増加量により変更する必要あり)")]
    private int maxHpGrowth;
    [SerializeField]
    private UIManager ui;

    [SerializeField,Tooltip("敵に攻撃をヒットさせた時の演出")]
    private GameObject hit;
    [SerializeField,Tooltip("敵を撃破したときの演出")]
    private GameObject exprosion;
    [NonSerialized] public Transform player;

    [SerializeField, Tooltip("敵撃破時に落ちる回復")] private GameObject Healball;
    [SerializeField, Tooltip("回復の落ちる確率")] private float DropPercentage = 30f;
    System.Random random=new System.Random();
    float DropHealball;

    EnemyAttackBase enemyAttackBase;
    Rigidbody rb;
    /// <summary> 敵を倒したときの入手経験値 /// </summary>    
    private int exp = 10;
    /// <summary> すでにオブジェクトプールに戻っているかチェック(二重の演出防止) </summary>
    private bool isReleased = false;
    /// <summary> EnemySpawnerのオブジェクトプール </summary>
    public IObjectPool<GameObject> MyPool { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        DropHealball = random.Next(100)+1;
        enemyAttackBase = GetComponent<EnemyAttackBase>();
    }
    /// <summary> プールが再利用されるたびに実行 </summary>
    public void OnReset()
    {
        enemyHP = enemyAttackBase.maxEnemyHP+Mathf.Min(maxHpGrowth,(int)(Time.timeSinceLevelLoad/timeUntilIncrease) * hpGrowthRate);
        isReleased = false;
        if (ui == null)
        {
            ui = GameObject.Find(GameObjectName.GameManager).GetComponent<UIManager>();
        }

    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObject = GameObject.FindWithTag(Tags.Player); //プレイヤーはPlayerタグを使用する想定
        ui = GameObject.Find(GameObjectName.GameManager).GetComponent<UIManager>();
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isReleased) return;

        if (transform.position.z <= -9.7f)
        {
            isReleased = true;
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
                damage.Damage(enemyAttackBase.collisionDamage);
                EnemyDie(true);
            }

            //Debug.Log("hit");
        }
    }
    /// <summary> 敵がダメージを受ける処理 </summary>
    ///<param name="damage">敵が受けるダメージ量</param>
    public void EnemyDamaged(int damage)
    {
        if (isReleased) return;
        enemyHP -= Mathf.Max(0, damage);
        if (enemyHP <= 0)
        {
            if (Healball != null&&DropHealball<=DropPercentage) Instantiate(Healball, transform.position, Quaternion.identity);
            EnemyDie(false);
        }
        else
        {
            Instantiate(hit, this.transform.position, Quaternion.identity);
        }

    }
    /// <summary> 敵が倒されたときの処理 </summary>
    /// <param name="isCollision">敵が衝突で倒されたかどうか。
    /// 衝突ならtrue,弾で倒したならfalse </param>
    private void EnemyDie(bool isCollision)
    {
        if (isReleased) return;
        isReleased = true;
        Instantiate(exprosion, this.transform.position, Quaternion.identity);
        if (ui == null)
        {
            ui = GameObject.Find(GameObjectName.GameManager).GetComponent<UIManager>();
        }

        if (ui != null&&!isCollision)
        {
            ui.Experience(exp);
        }

        // 撃墜数のカウント（弾で倒したときのみ。体当たりや画面外への離脱は数えない）
        if (!isCollision && KillCountManager.Instance != null)
        {
            KillCountManager.Instance.AddKill();
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

