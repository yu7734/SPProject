using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float playerSpeed;
    private Vector2 moveInput = Vector2.zero;
    //private InputAction moveAction;
    //private InputAction shotAction;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shotPoint;
    //[SerializeField] private

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //moveのリファレンスを探す
        //moveAction = InputSystem.actions.FindAction("Move");
        //shotAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        PlayerController();
    }

    private void PlayerController()
    {
        //移動処理
        //var moveValue = context.ReadValue<Vector2>();
        var move = new Vector3(moveInput.x, 0, -moveInput.y) * playerSpeed * Time.deltaTime;
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
        if (context.performed)
        {
            //弾を生成
            Instantiate(bulletPrefab, shotPoint.transform.position, Quaternion.identity);
        }
    }

    //回避動作
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
    }

}
