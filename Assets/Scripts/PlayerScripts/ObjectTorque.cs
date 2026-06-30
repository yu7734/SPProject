using UnityEngine;

public class ObjectTorque : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerManagerを取得")] 
    private PlayerManager player;

    [Header("オブジェクトの傾き")]
    [SerializeField, Tooltip(" 水平移動時に機首を左右に向けるトルク")] 
    private float yawTorqueMagnitude = 20.0f;
    [SerializeField, Tooltip("垂直移動時に機首を上下に向けるトルク")] 
    private float pithcTorqueMagnitude = 60.0f;
    [SerializeField, Tooltip("垂直移動時に機首を上下に向けるトルク")] 
    private float rollTorqueMagnitude = 30.0f;
    [SerializeField, Tooltip("バネのように姿勢を元に戻すトルク")] 
    private float restoringTorqueMagnitude = 100.0f;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //常にブレないように,angularDampingを大きめに設定
        rb.angularDamping = 20.0f;
    }

    void FixedUpdate()
    {
        InclineTorque();
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
