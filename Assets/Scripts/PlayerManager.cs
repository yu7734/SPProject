using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

//インターフェイスでダメージ処理
public interface IPlayerDamage
{
    public void Damage(int value);
    //public void Death();
}


public class PlayerManager : MonoBehaviour
{
    Rigidbody rb;
    //プレイヤーのスピード
    [SerializeField] private float playerSpeed;
    public Vector2 moveInput = Vector2.zero;

    //プレイヤーの弾
    [SerializeField] private GameObject bulletPrefab;
    //弾が発射する所
    [SerializeField] private Transform shotPoint;

    [SerializeField, Header("体力")] public int playerHP;
    [SerializeField, Header("最大体力")] public int MaxPlayerHP;
    public GameObject[] enemy;

    //[SerializeField] public CameraShake cameraShake;

    [SerializeField] private UIManager ui;
    [SerializeField] private JustDodgeManager justDodgeManager;

    [SerializeField] private Transform playerModel;
    float dodgetime = 0;
    //ジャスト回避状態か
    public bool bJustDodge = false;
    float justDodgeTime = 0;
    public float dodgeCoolTime = 0;
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
        Debug.Log(bJustDodge);
        switch (_state)
        {

            //何もしていない状態
            case dodgeState.None:
                //Debug.Log(_state);
                break;

            //ジャスト回避状態
            case dodgeState.JustDodge:
                //Debug.Log(_state);
                JustDodge();
                break;

            //普通の回避状態
            case dodgeState.dodge:
                //Debug.Log(_state);
                Dodge();
                break;

            //回避が出来ないクールタイム状態
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
            playerModel.DORotate(new Vector3(0f, 0, 360), 1f, RotateMode.WorldAxisAdd);
            bJustDodge = true;
            _state = dodgeState.dodge;
        }
    }

    private void Dodge()
    {
        //ジャスト回避状態
        justDodgeTime += Time.deltaTime;
        if (justDodgeTime >= 0.3f)
            bJustDodge = false;

        //普通の回避状態
        dodgetime += Time.deltaTime;
        if (dodgetime >= 1f)
        {
            dodgetime = 0;
            justDodgeTime = 0;
            _state = dodgeState.coolTime;
        }
    }

    public void JustDodge()
    {
        justDodgeTime += Time.deltaTime;
        if (justDodgeTime >= 1f)
        {
            justDodgeTime = 0;
            _state = dodgeState.dodge;
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

