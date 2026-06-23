using UnityEngine;
using UnityEngine.InputSystem;

public class ItemLaserShotScript : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    UIManager ui;
    [SerializeField] Vector3 Ofset = new (0f, 0f, 0f);
    PlayerInput playerInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameObject GameManager = GameObject.Find("GameManager");
        ui = GameManager.GetComponent<UIManager>();
        GameObject Player = GameObject.Find("Player");
        playerInput = Player.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.actions["Attack"].triggered) OnShot();
    }

    public void OnShot()
    {
        if (!ui.bSelect)
        {
            Instantiate(bulletPrefab, transform.position+Ofset, Quaternion.identity);
        }
    }
}
