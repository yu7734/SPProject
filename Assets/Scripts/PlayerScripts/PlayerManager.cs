using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

//プレイヤーに受けるダメージ
public interface IPlayerDamage
{
    public void Damage(int value);
    //public void Death();
}


public class PlayerManager : MonoBehaviour
{
    //プレイヤーのスピード
    [SerializeField] private float playerSpeed;
    public Vector2 moveInput = Vector2.zero;

    //プレイヤー
    [Tooltip("自機オブジェクトの位置")] private GameObject playerChildObject;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Tooltip("弾の速度")] private float bulletSpeed;
    
    //発射する位置
    [SerializeField] private Transform shotPoint;

    [SerializeField, Header("プレイヤーの体力")] public int playerHP;
    [SerializeField, Header("プレイヤーの最大体力")] public int MaxPlayerHP;


    [SerializeField] private UIManager ui;

    [SerializeField] private Transform playerModel;
    float dodgetime = 0;
    //ジャスト回避のフラグ
    public bool bJustDodge = false;
    float justDodgeTime = 0;
    public float dodgeCoolTime = 0;
    [SerializeField, Header("回避のクールタイム")] 
    private float coolTime;

    //回避のステートマシン
    public enum dodgeState
    {
        None,
        dodge,
        coolTime,
    }

    public dodgeState _state = dodgeState.None;

    private void Awake()
    {
        //bulletPrefab = GetComponent<Rigidbody>();
        //Rigidbody bulletRigid = bulletPrefab.GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerChildObject = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            //何もない状態
            case dodgeState.None:
                break;

            //回避状態
            case dodgeState.dodge:
                Dodge();
                break;

            //クールタイム状態
            case dodgeState.coolTime:
                DodgeCoolTime();
                break;
        }
    }

    private void FixedUpdate()
    {
        PlayerController();
    }

    private void PlayerController()
    {
        //移動
        var move = new Vector3(moveInput.x, moveInput.y, 0) * playerSpeed * Time.deltaTime;
        transform.Translate(move);
    }

    //移動ボタン
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //発射ボタン
    public void OnShot(InputAction.CallbackContext context)
    {
        if (context.performed && !ui.bSelect)
        {
            //Vector3 playerShotDirection = playerChildObject.rotation;
            //弾を呼び出す
            Instantiate(bulletPrefab, shotPoint.transform.position, playerChildObject.transform.rotation);

        }
    }

    //回避ボタンが押されたら
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed && _state == dodgeState.None)
        {
            //自機を回転
            playerModel.DORotate(new Vector3(0f, 0, 360), 1f, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuart);
            bJustDodge = true;
            _state = dodgeState.dodge;
        }
    }

    private void Dodge()
    {
        //ジャスト回避のカウント開始
        justDodgeTime += Time.deltaTime;
        if (justDodgeTime >= 0.3f)
            bJustDodge = false;

        //回避のカウント開始
        dodgetime += Time.deltaTime;
        if (dodgetime >= 1f)
        {
            dodgetime = 0;
            justDodgeTime = 0;
            _state = dodgeState.coolTime;
        }
    }

    //クールタイム
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

