using UnityEngine;

public class PlayerObjectManager : MonoBehaviour, IPlayerDamage
{
    //PlayerManager���擾
    [SerializeField] private PlayerManager player;
    //CameraShake���擾
    [SerializeField] public CameraShake cameraShake;

    //�����ړ����ɋ@������E�ɌX����g���N
    [SerializeField] private float yawTorqueMagnitude = 20.0f;
    //�����ړ����ɋ@����㉺�ɌX����g���N
    [SerializeField] private float pithcTorqueMagnitude = 60.0f;
    //�����ړ����ɋ@�̂����E�ɌX����g���N
    [SerializeField] private float rollTorqueMagnitude = 30.0f;
    //�o�l�̂悤�Ɏp�������ɖ߂��g���N
    [SerializeField] private float restoringTorqueMagnitude = 100.0f;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //�o�l�̕����͂ł����h�ꑱ����̂�h������,angularDamping��傫�߂ɂ���
        rb.angularDamping = 20.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        InclineTorque();
    }

    //�_���[�W����
    public void Damage(int value)
    {
        //������Ă��Ȃ���Ԃ���
        if (player._state == PlayerManager.dodgeState.None || player._state == PlayerManager.dodgeState.coolTime)
        {
            Debug.Log("�q�b�g");
            //�J�������U������
            cameraShake.CameraShaker();
            //HP������
            player.playerHP -= Mathf.Max(0, value);
        }
    }

    private void InclineTorque()
    {

        //�v���C���[�̓��͂ɉ����Ďp����P�낤�Ƃ���g���N
        Vector3 rotationTorque = new Vector3(-player.moveInput.y * pithcTorqueMagnitude, player.moveInput.x * yawTorqueMagnitude, -player.moveInput.x * rollTorqueMagnitude);

        //���݂̎p���̂���ɔ�Ⴕ���傫���ŋt�����ɔP�낤�Ƃ���g���N
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Vector3 forward = transform.forward;
        Vector3 restoringTorque = new Vector3(forward.y - up.z, right.z - forward.x, up.x - right.y) * restoringTorqueMagnitude;

        //���҂Ƀg���N��������
        rb.AddTorque(rotationTorque + restoringTorque);
    }
}
