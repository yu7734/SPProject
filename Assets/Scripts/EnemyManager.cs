using UnityEngine;

public interface IEnemyDamage
{
    public void EnemyDamaged(int damage);
}

public class EnemyManager : MonoBehaviour, IEnemyDamage
{
    [Header("行動種類(複数選択可)")]
    [Tooltip("通常弾")]
    public bool isShot = false;
    [Tooltip("自機狙いの弾")]
    public bool isTrackShot = false;
    [Tooltip("自機狙い突進")]
    public bool isRush = false;

    [Header("射撃にかかわる変数")]
    public GameObject Bullet;
    [Tooltip("弾を撃つ間隔（秒）")]
    public float coolTime = 0.5f;
    [Tooltip("弾を撃つ力（速さ）")]
    public float enemyBulletForce = 3f;

    [Header("突撃にかかわる変数")]
    [Tooltip("最初の停止時間（秒）")]
    public float stopTime = 1;
    [Tooltip("プレイヤーを見ている時間（秒）")]
    public float lookAtTime = 2;
    [Tooltip("敵の突撃速度")]
    public float enemyRushSpeed = 3f;
    [Tooltip("敵をプレイヤー通過後に非表示にするまでの距離")]
    public float RushOffset = 10f;

    //ダメージ量
    [SerializeField] private int attackPower;

    //敵のHP
    [SerializeField] private int enemyHP;
    [SerializeField] private UIManager ui;
    [SerializeField] private GameObject playerBullet;

    [SerializeField] private GameObject exprosion;

    private float offset = 1.5f; // 敵の大きさによって調整
    private float trackShotTimer = 0;
    private float shotTimer = 0;
    private float rushTimer = 0;

    Transform player;   //プレイヤーの座標
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObject = GameObject.FindWithTag("Player"); //プレイヤーはPlayerタグを使用する想定

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (player == null) return;

        if (isTrackShot)   //インスペクターで制御
        {
            TrackShotPattern();
        }
        if (isShot)
        {
            ShotPattern();
        }
        if (isRush)
        {
            RushPattern();
        }

        if (transform.position.z <= -9.7f)
        {
            Destroy(this.gameObject);
        }

        EnemyDie();
    }
    void TrackShotPattern()
    {
        if (!isRush) transform.LookAt(player.transform); //プレイヤー側に常に向く
        trackShotTimer += Time.deltaTime;
        if (trackShotTimer > coolTime)   //coolTimeごとにoffsetの距離前に生成してその時点のプレイヤーの方向にenemyBulletForceの力で撃つ。３秒後に各自破壊。
        {
            Vector3 spawnPosition = transform.position + transform.forward * offset;

            GameObject newBullet = Instantiate(Bullet, spawnPosition, transform.rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

            Vector3 shotDirection = (player.position - spawnPosition).normalized;

            Bulletrb.AddForce(shotDirection * enemyBulletForce);
            Destroy(newBullet, 3f);
            trackShotTimer = 0;
        }
    }
    void ShotPattern()
    {

        shotTimer += Time.deltaTime;
        if (shotTimer > coolTime)   //coolTimeごとにoffsetの距離前に生成してその時点のプレイヤーの方向にenemyBulletForceの力で撃つ。３秒後に各自破壊。
        {
            Vector3 spawnPosition = transform.position + transform.forward * offset;

            GameObject newBullet = Instantiate(Bullet, spawnPosition, transform.rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();
            Bulletrb.AddForce(transform.forward * enemyBulletForce);
            Destroy(newBullet, 3f);
            shotTimer = 0;
        }
    }
    void RushPattern()  //lookAtTimeになるまでプレイヤーの座標に向けて移動し、それ以降はその時向いていた方向に直線的に移動。プレイヤーZが手前に移動したとき非表示。
    {
        rushTimer += Time.deltaTime;
        if (rushTimer < stopTime) return;
        if (rushTimer < lookAtTime)
        {
            transform.LookAt(player.transform);
        }
        transform.position += transform.forward * enemyRushSpeed * Time.deltaTime;

        if (transform.position.z < player.position.z + -RushOffset)
        {
            gameObject.SetActive(false);
        }
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