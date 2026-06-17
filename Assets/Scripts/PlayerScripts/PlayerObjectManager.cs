using UnityEngine;

public class PlayerObjectManager : MonoBehaviour
{
    //PlayerManager���擾
    [SerializeField] private PlayerManager player;
    //CameraShake���擾
    [SerializeField] public CameraShake cameraShake;

    // 水平移動時に機首を左右に向けるトルク
    [SerializeField] private float yawTorqueMagnitude = 20.0f;
    // 垂直移動時に機首を上下に向けるトルク
    [SerializeField] private float pithcTorqueMagnitude = 60.0f;
    // 水平移動時に機体を左右に傾けるトルク
    [SerializeField] private float rollTorqueMagnitude = 30.0f;
    // バネのように姿勢を元に戻すトルク
    [SerializeField] private float restoringTorqueMagnitude = 100.0f;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //常にブレないように,angularDampingを大きめに設定
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

    //ダメージ処理
    public void Damage(int value)
    {
        //回避していないかクールタイムか
        if (player._state == PlayerManager.dodgeState.None || player._state == PlayerManager.dodgeState.coolTime)
        {
            Debug.Log("hit");
            //カメラを揺らす
            cameraShake.CameraShaker();
            //HP減少
            player.playerHP -= Mathf.Max(0, value);
        }
    }

    private void InclineTorque()
    {

        // プレイヤーの入力に応じて姿勢をひねろうとするトルク
        Vector3 rotationTorque = new Vector3(-player.moveInput.y * pithcTorqueMagnitude, player.moveInput.x * yawTorqueMagnitude, -player.moveInput.x * rollTorqueMagnitude);

        // 現在の姿勢のずれに比例した大きさで逆方向にひねろうとするトルク
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Vector3 forward = transform.forward;
        Vector3 restoringTorque = new Vector3(forward.y - up.z, right.z - forward.x, up.x - right.y) * restoringTorqueMagnitude;

        // 機体にトルクを加える
        rb.AddTorque(rotationTorque + restoringTorque);
    }
}
