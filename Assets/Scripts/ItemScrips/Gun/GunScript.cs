using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    public float cooltime = 2f;
    float time = 0f;
    bool shotReady = true;
    [SerializeField] private GameObject bulletPrefab;
    UIManager ui;
    [SerializeField] Vector3 Ofset = new(0f, 0f, 0f);
    PlayerInput playerInput;
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
        time += Time.deltaTime;
        if (!shotReady && time >= cooltime) shotReady = true;
    }

    public void OnShot()
    {
        if (ui.bSelect && shotReady)
        {
            Instantiate(bulletPrefab,transform.position+Ofset,Quaternion.identity);
        }
    }
}
