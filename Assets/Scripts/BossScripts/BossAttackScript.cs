using UnityEngine;

public class BossAttackScript : MonoBehaviour
{
    [SerializeField] GameObject CannonBallet, LaserBallet, GatlingBallet;
    [SerializeField] GameObject[] GatlingShotPoints;
    [SerializeField] GameObject[] CannonShotPoints;
    [SerializeField] bool test;

    void Awake()
    {

    }

    private void Update()
    {
        if (test)
        {
            CannonShot();
            test = false;
        }
    }
    void CannonShot() 
    {
        for (int i = 0; i < CannonShotPoints.Length; i++)
        {
            Instantiate(CannonBallet, CannonShotPoints[i].transform.position, CannonShotPoints[i].transform.rotation);
            Instantiate(CannonBallet, CannonShotPoints[i].transform.position, CannonShotPoints[i].transform.rotation);
        }
    }

    void LaserShot() 
    {
        float timer1 = Time.deltaTime;
        float timer2 = Time.deltaTime;
        float LaserTimer = 0;
        float RushTimer = 0;
        if(timer2 <= LaserTimer)
            if (timer1 >= RushTimer)
                Instantiate(LaserBallet);
    }

    void GumiShot() 
    {
        int count = 3;
        float timer1 = Time.deltaTime;
        float timer2 = Time.deltaTime;
        float LaserTimer = 0;
        float RushTimer = 0;
        if (timer2 <= LaserTimer)
            if (timer1 >= RushTimer)
                for (int i = 0; i < count; ++i) 
                    Instantiate(GatlingBallet);
    }
}
