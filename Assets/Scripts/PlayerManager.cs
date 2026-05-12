using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

//インターフェイスでダメージ処理
public interface IPlayerDamage
{
    public void Damage(int value);
    //public void Death();
}


public class PlayerManager : MonoBehaviour, IPlayerDamage
{
    Rigidbody rb;
    //プレイヤーのスピード
    [SerializeField] private float playerSpeed;
    private Vector2 moveInput = Vector2.zero;

    //プレイヤーの弾
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shotPoint;

    [SerializeField, Header("体力")] public int playerHP;
    [SerializeField, Header("最大体力")] public int MaxPlayerHP;
    public GameObject[] enemy;

    [SerializeField] public CameraShake cameraShake;

    [SerializeField] private UIManager ui;
    [SerializeField] private JustDodgeManager justDodgeManager;

    float dodgetime = 0;
    float justDodgeTime = 0;
    float dodgeCoolTime = 0;
    [SerializeField, Header("クールタイム")] private float coolTime;

    //回避の状態ステートマシン
    public enum dodgeState
    {
        None,
        JustDodge,
        dodge,
        coolTime,
    }

    public dodgeState _state = dodgeState.None;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {

            case dodgeState.None:
                //Debug.Log(_state);
                break;

            case dodgeState.JustDodge:
                //Debug.Log(_state);
                JustDodge();
                break;

            case dodgeState.dodge:
                //Debug.Log(_state);
                Dodge();
                break;

            case dodgeState.coolTime:
                DodgeCoolTime();
                break;
        }

        //if (bDodge)
        //{
        //    time += Time.deltaTime;
        //    if (time >= coolTime)
        //    {
        //        time = coolTime;
        //        bDodge = false;
        //    }
        //}
    }

    private void FixedUpdate()
    {
        PlayerController();
    }

    private void PlayerController()
    {
        //移動処理
        var move = new Vector3(moveInput.x, moveInput.y, 0) * playerSpeed * Time.deltaTime;
        transform.Translate(move);
    }

    //プレイヤーの移動
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //発射ボタン
    public void OnShot(InputAction.CallbackContext context)
    {
        if (context.performed && !ui.bSelect)
        {
            //弾を生成
            Instantiate(bulletPrefab, shotPoint.transform.position, Quaternion.identity);
        }
    }

    //回避動作
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed && _state == dodgeState.None)
        {
            //回転アニメーション
            transform.DORotate(new Vector3(0f, 0, 360), 1f, RotateMode.WorldAxisAdd);
            _state = dodgeState.JustDodge;
        }
    }

    private void Dodge()
    {
        dodgetime += Time.deltaTime;
        if (dodgetime >= 1f)
        {
            dodgetime = 0;
            _state = dodgeState.coolTime;
        }
    }

    public void JustDodge()
    {
        justDodgeTime += Time.deltaTime;
        if (justDodgeTime >= 0.1f)
        {
            justDodgeTime = 0;
            _state = dodgeState.dodge;
        }
    }

    //private void OnTriggerEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy") && !bDodge)
    //    {
    //        enemy.GetComponent<EnemyManager>().PlayerDamage(this);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (enemy == null) return;
        //敵に触れたら
        //if (other.gameObject.CompareTag("Enemy") && (_state == dodgeState.None || _state == dodgeState.coolTime))
        //{
        //    Debug.Log("ヒット");
        //    //カメラが振動する
        //    cameraShake.CameraShaker();
        //    //ダメージを受ける

        //    //other.GetComponent<EnemyManager>().PlayerDamage(this);
        //}
    }

    //プレイヤーに受けるダメージ
    public void Damage(int value)
    {
        if (_state == dodgeState.None || _state == dodgeState.coolTime)
        {
            Debug.Log("ヒット");
            //カメラが振動する
            cameraShake.CameraShaker();

            playerHP -= value;
        }
    }

    void DodgeCoolTime()
    {
        dodgeCoolTime += Time.deltaTime;

        if (dodgeCoolTime > coolTime)
        {
            dodgeCoolTime = 0;
            _state = dodgeState.None;
        }
    }
}

