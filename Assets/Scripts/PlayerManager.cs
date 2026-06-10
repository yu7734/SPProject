using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

//�C���^�[�t�F�C�X�Ń_���[�W����
public interface IPlayerDamage
{
    public void Damage(int value);
    //public void Death();
}


public class PlayerManager : MonoBehaviour
{
    Rigidbody rb;
    //�v���C���[�̃X�s�[�h
    [SerializeField] private float playerSpeed;
    public Vector2 moveInput = Vector2.zero;

    //�v���C���[�̒e
    [SerializeField] private GameObject bulletPrefab;
    //�e�����˂��鏊
    [SerializeField] private Transform shotPoint;

    [SerializeField, Header("�̗�")] public int playerHP;
    [SerializeField, Header("�ő�̗�")] public int MaxPlayerHP;
    public GameObject[] enemy;

    //[SerializeField] public CameraShake cameraShake;

    [SerializeField] private UIManager ui;
    [SerializeField] private JustDodgeManager justDodgeManager;

    [SerializeField] private Transform playerModel;
    float dodgetime = 0;
    //�W���X�g�����Ԃ�
    public bool bJustDodge = false;
    float justDodgeTime = 0;
    public float dodgeCoolTime = 0;
    [SerializeField, Header("�N�[���^�C��")] private float coolTime;

    //����̏�ԃX�e�[�g�}�V��
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

            //何もない状態
            case dodgeState.None:
                //Debug.Log(_state);
                break;

            //�W���X�g������
            case dodgeState.JustDodge:
                //Debug.Log(_state);
                JustDodge();
                break;

            //回避状態
            case dodgeState.dodge:
                //Debug.Log(_state);
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
            //�e�𐶐�
            Instantiate(bulletPrefab, shotPoint.transform.position, Quaternion.identity);
        }
    }

    //��𓮍�
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed && _state == dodgeState.None)
        {
            //��]�A�j���[�V����
            playerModel.DORotate(new Vector3(0f, 0, 360), 1f, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuart);
            bJustDodge = true;
            _state = dodgeState.dodge;
        }
    }

    private void Dodge()
    {
        //�W���X�g������
        justDodgeTime += Time.deltaTime;
        if (justDodgeTime >= 0.3f)
            bJustDodge = false;

        //���ʂ̉�����
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

    //�N�[���^�C��
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

