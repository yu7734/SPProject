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
    void Awake()
    {
        ui = FindAnyObjectByType<UIManager>();
        playerInput = FindAnyObjectByType<PlayerInput>();
        manager = FindAnyObjectByType<GunManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ui.bSelect&&!shotReady) time += Time.deltaTime;
        if (!shotReady && time >= manager.cooltime) shotReady = true;
        if (playerInput.actions["Attack"].triggered && shotReady)OnShot();
    }

    public void OnShot()
    {
        if (!ui.bSelect&&shotReady)
        {
            Instantiate(bulletPrefab,transform.position+Ofset,Quaternion.identity);
            shotReady = false;
            time = 0;
        }
    }
}
