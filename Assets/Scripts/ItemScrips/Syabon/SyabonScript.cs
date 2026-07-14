using UnityEngine;

public class SyabonScript : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    UIManager ui;
    SyabonManagerScript Manager;
    [SerializeField] Vector3 Ofset = new(0f, 0f, 0f);
    float time1 = 0, time2 = 0;
    float x = 0, y = 0;
    
    void Awake()
    {
        Manager = FindAnyObjectByType<SyabonManagerScript>();
        ui = FindAnyObjectByType<UIManager>();
    }

    void Update()
    {
        if (!ui.bSelect) time1 += Time.deltaTime;
        if (time1 > Manager.ShotCooltime) time2 += Time.deltaTime;
        if (time2 >= Manager.ShotInterval) OnShot(Manager.ShotFrequency);
        if (time1 > Manager.ShotDuration + Manager.ShotCooltime) time1 = 0;
    }

    public void OnShot(int N)
    {
        if (!ui.bSelect)
        {
            for(int i=0;i<N;++i)RandomShot();
            time2 = 0;
        }
    }

    void RandomShot() 
    {
        x = Random.Range(-Manager.x_Range, Manager.x_Range);
        y = Random.Range(-Manager.y_Range, Manager.y_Range);
        Instantiate(bulletPrefab, transform.position + Ofset, Quaternion.Euler(x, y, 0f));
    }
}
