using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("number1->射撃,number2->突撃")]
    public int enemyNumber = 1;

    [Header("射撃にかかわる変数")]
    public GameObject Bullet;
    [Tooltip("弾を撃つ間隔（秒）")]
    public float coolTime = 0.5f;
    [Tooltip("弾を撃つ力（速さ）")]
    public float enemyBulletForce = 3f;
    
    [Header("突撃にかかわる変数")]
    [Tooltip("プレイヤーを見ている時間（秒）")]
    public float lookAtTime = 1;
    [Tooltip("敵の突撃速度")]
    public float enemyRushSpeed = 3f;
    [Tooltip("敵をプレイヤー通過後に非表示にするまでの距離")]
    public float RushOffset = 10f;

    private float offset = 1.5f; // 敵の大きさによって調整
    private float timer = 0;

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
        if (player == null) return;

        if (enemyNumber == 1)   //インスペクターで制御
        {
            ShotPattern();
        }   
        if(enemyNumber == 2)
        {
            RushPattern();
        }
    }
    void ShotPattern()
    {
        transform.LookAt(player.transform); //プレイヤー側に常に向く
        timer += Time.deltaTime;
        if (timer > coolTime)   //coolTimeごとにoffsetの距離前に生成してその時点のプレイヤーの方向にenemyBulletForceの力で撃つ。３秒後に各自破壊。
        {
            Vector3 spawnPosition = transform.position + transform.forward * offset;

            GameObject newBullet = Instantiate(Bullet, spawnPosition, transform.rotation);
            Rigidbody Bulletrb = newBullet.GetComponent<Rigidbody>();

            Vector3 shotDirection = (player.position - spawnPosition).normalized;
            
            Bulletrb.AddForce(shotDirection * enemyBulletForce);
            Destroy(newBullet, 3f);
            timer = 0;
        }
    }
    void RushPattern()  //lookAtTimeになるまでプレイヤーの座標に向けて移動し、それ以降はその時向いていた方向に直線的に移動。プレイヤーZが手前に移動したとき非表示。
    {
        timer += Time.deltaTime;
        if (timer < lookAtTime)
        {
            transform.LookAt(player.transform);
        }
        transform.position += transform.forward * enemyRushSpeed * Time.deltaTime;

        if (transform.position.z < player.position.z + -RushOffset)
        {
            gameObject.SetActive(false);
        }
    }
}
