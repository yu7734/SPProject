using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    float time = 0f;
    public bool shotReady = true;
    [SerializeField] private GameObject bulletPrefab;
    UIManager ui;
    GunManagerScript manager;
    [SerializeField] Vector3 Ofset = new(0f, 0f, 1f);
    PlayerInput playerInput;
    Transform playerRot;
    void Awake()
    {
        ui = FindAnyObjectByType<UIManager>();
        playerInput = FindAnyObjectByType<PlayerInput>();
        playerRot = GameObject.FindWithTag(GameObjectName.Player).GetComponent<Transform>();
        manager = FindAnyObjectByType<GunManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playerRot.eulerAngles);
        if (!ui.bSelect&&!shotReady) time += Time.deltaTime;
        if (!shotReady && time >= manager.cooltime) shotReady = true;
        if (playerInput.actions[PlayerInputActionName.Attack].triggered && shotReady)OnShot();
    }

    public void OnShot()
    {
        if (!ui.bSelect&&shotReady)
        {
            Instantiate(bulletPrefab,transform.position+Ofset,Quaternion.Euler(playerRot.eulerAngles.x,playerRot.eulerAngles.y,0f));
            shotReady = false;
            time = 0;
        }
    }
}
