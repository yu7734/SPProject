using UnityEngine;

public class newFanelManager : MonoBehaviour
{
    const int MaxLoop=20;
    GameObject Player;
    [SerializeField] GameObject FanelPrefab;
    GameObject[] Fanel;
    [SerializeField] GameObject FanelBullet;
    BulletManagert bulletManagert;
    Vector3 PlayerPos;
    [HideInInspector] public Vector3[] vector3s= new Vector3[MaxLoop];
    int loop = 0;
    [HideInInspector] public int FanelCount = 0;
    [SerializeField] Vector3 offset = new(0f,0f,-1.25f);
    [SerializeField] bool Select = false;

    const int FanelPos1 = 14,FanelPos2 = 9,FanelPos3=4,FanelPos4=0;
    void Awake() 
    {
        Player = GameObject.Find("Player");
        bulletManagert = FanelBullet.GetComponent<BulletManagert>();
        bulletManagert.bulletDamageRate = bulletManagert.BASE_bulletDamageRate;
        for (int i=0;i<MaxLoop;i++)vector3s[i] = Player.transform.position;
    }
    
    void FixedUpdate()
    {
        if (PlayerPos != Player.transform.position)
        {
            PlayerPos = Player.transform.position;
            vector3s[loop] = Player.transform.position;
            ++loop;
            if (loop >= MaxLoop) loop = 0;
        }
        
        switch (FanelCount) 
        {
            case 4:
                Fanel[3].transform.position = vector3s[(FanelPos4 + loop) % MaxLoop] + offset;
                goto case 3;
            case 3:
                Fanel[2].transform.position = vector3s[(FanelPos3 + loop) % MaxLoop] + offset;
                goto case 2;
            case 2:
                Fanel[1].transform.position = vector3s[(FanelPos2 + loop) % MaxLoop] + offset;
                goto case 1;
            case 1:
                Fanel[0].transform.position = vector3s[(FanelPos1 + loop) % MaxLoop] + offset;
                break;
        }
        
        if (Select) 
        {
            SerectFanel();
            Select = false;
        }
    }

    public void SerectFanel() 
    {
        switch (FanelCount)
        {
            case 0:
                Instantiate(FanelPrefab, vector3s[(FanelPos1 + loop) % MaxLoop], Quaternion.identity);
                break;
            case 1:
                Instantiate(FanelPrefab, vector3s[(FanelPos2 + loop) % MaxLoop], Quaternion.identity);
                break;
            case 2:
                Instantiate(FanelPrefab, vector3s[(FanelPos3 + loop) % MaxLoop], Quaternion.identity);
                break;
            case 3:
                Instantiate(FanelPrefab, vector3s[(FanelPos4 + loop) % MaxLoop], Quaternion.identity);
                break;
            default:
                break;
        }
        Fanel = GameObject.FindGameObjectsWithTag("Fanel");
        ++FanelCount;
    }
}
