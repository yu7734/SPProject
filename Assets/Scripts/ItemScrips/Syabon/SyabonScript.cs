using UnityEngine;
using UnityEngine.InputSystem;

public class SyabonScript : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    UIManager ui;
    [SerializeField] Vector3 Ofset = new(0f, 0f, 0f);
    public float x_Range = 10f, y_Range = 10f;
    public float ShotInterval = 0.1f;
    public float ShotCooltime = 1f;
    public float ShotDuration = 3f;
    float time1 = 0, time2 = 0;
    float x = 0, y = 0;
    
    void Awake()
    {
        GameObject GameManager = GameObject.Find("GameManager");
        ui = GameManager.GetComponent<UIManager>();
    }

    void Update()
    {
        if (!ui.bSelect) time1 += Time.deltaTime;
        if (time1 > ShotCooltime) time2 += Time.deltaTime;
        if (time2 >= ShotInterval) OnShot();
        if (time1 > ShotDuration + ShotCooltime) time1 = 0;
    }

    public void OnShot()
    {
        if (!ui.bSelect)
        {
            x = Random.Range(-x_Range, x_Range);
            y = Random.Range(-y_Range, y_Range);
            Instantiate(bulletPrefab, transform.position + Ofset, Quaternion.Euler(x,y,0f));
            time2 = 0;
        }
    }
}
