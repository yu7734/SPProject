using UnityEngine;

public class SyabonManagerScript : MonoBehaviour
{
    [SerializeField] GameObject[] Syabon;
    [SerializeField] GameObject SyabonBullet;
    BulletManagert BulletManagert;
    [Tooltip("Œ‚‚ÂŠp“x‚Ì”ÍˆÍ")] public float x_Range = 10f, y_Range = 10f;
    [Tooltip("Œ‚‚Ä‚éó‘Ô‚Å‚ÌŒ‚‚Â•p“x")] public float ShotInterval = 0.2f;
    [Tooltip("Œ‚‚½‚È‚¢ŠÔ")] public float ShotCooltime = 1f;
    [Tooltip("Œ‚‚Ä‚éŠÔ")] public float ShotDuration = 3f;
    [Tooltip("ˆê“x‚ÉŒ‚‚ÂŒÂ”")] public int ShotFrequency = 2;
    public int SyabonCount = 0;
    [SerializeField] bool Serect=false; 
    void Awake()
    {
        BulletManagert = SyabonBullet.GetComponent<BulletManagert>();
        BulletManagert.bulletDamageRate = BulletManagert.BASE_bulletDamageRate; 
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
