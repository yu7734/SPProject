using UnityEngine;

public class SyabonManagerScript : MonoBehaviour
{
    [SerializeField] GameObject[] Syabon;
    [SerializeField] GameObject SyabonBullet;
    BulletManagert BulletManagert;
    public int SyabonCount = 0;
    [SerializeField] bool Serect=false; 
    void Awake()
    {
        BulletManagert = SyabonBullet.GetComponent<BulletManagert>();
    }

    void Update() 
    {
        if (Serect) 
        {
            SerectSyabon();
            Serect = false;
        }
    }
    public void SerectSyabon() 
    {
        switch (SyabonCount) 
        { 
            case 0:
                Syabon[0].SetActive(true);
                break;
            case 2:
                Syabon[1].SetActive(true);
                break;
            default:
            case 1:
            case 3:
                BulletManagert.bulletDamageRate += 0.2f;
                break;
        }
        SyabonCount++;
    }
}
