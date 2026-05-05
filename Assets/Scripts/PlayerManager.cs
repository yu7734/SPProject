using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UIElements;
using TMPro.EditorUtilities;

public class PlayerManager : MonoBehaviour
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
    private GameObject enemy;

    [SerializeField] private UIManager ui;
    [SerializeField] private JustDodgeManager justDodgeManager;

    //経験値
    public int experiencePoint = 0;

    private Animator animator;

    float time = 0;
    [SerializeField, Header("クールタイム")] private float coolTime;

    //回避の状態ステートマシン
    public enum dodgeState{
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
        animator = GetComponent<Animator>();
        enemy = GameObject.FindWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case dodgeState.None:
                break;

            case dodgeState.JustDodge:
                justDodgeManager.JustDodge();
                break;

            case dodgeState.dodge:
                Dodge();
                break;

            case dodgeState.coolTime:
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
        if (context.performed )
        {
            //回転アニメーション
            transform.DORotate(new Vector3(0f, 0, 360), 1f, RotateMode.WorldAxisAdd);
            _state = dodgeState.JustDodge;
        }
    }

    private void Dodge()
    {
        time = 0;
        time += Time.deltaTime;
        if (time >= 0.9f)
            _state = dodgeState.coolTime;
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
        //敵に触れたら
        if (other.gameObject.CompareTag("Enemy") && (_state == dodgeState.None || _state == dodgeState.coolTime))
        {
            enemy.GetComponent<EnemyManager2>().PlayerDamage(this);
        }
    }

    //プレイヤーに受けるダメージ
    public void Damage(int damage)
    {
        playerHP -= Mathf.Max(0, damage);
    }

    void DodgeCoolTime()
    {
        float dodgeTime = 0;
        dodgeTime += Time.deltaTime;
        if (dodgeTime > coolTime)
            _state = dodgeState.None;

    }
}
