using UnityEngine;
using UnityEngine.InputSystem;

public class LaserCannon : MonoBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0f, 0f, 0f);
    [SerializeField] GameObject[] BigLaser;
    public float ShotInterval = 1f;
    public int LaserNum = 0;
    UIManager ui;
    float time;

    void Awake()
    {
        GameObject GameManager = GameObject.Find("GameManager");
        ui = GameManager.GetComponent<UIManager>();
    }

    void Update()
    {
        if (!ui.bSelect) time += Time.deltaTime;
        if (time >= ShotInterval)
        {
            OnShot();
            time = 0;
        }
    }

    public void OnShot()
    {
        if (!ui.bSelect)
        {
            Instantiate(BigLaser[LaserNum], transform.position + offset, Quaternion.identity);
        }
    }
}
