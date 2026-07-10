using UnityEngine;

public class GunManagerScript : MonoBehaviour
{
    [SerializeField] GameObject[] Gun;
    [SerializeField] GameObject GunBullet;
    BulletManagert bulletManagert;
    public int GunCount=0;
    [SerializeField] bool Serect=false;

    void Awake()
    {
        bulletManagert = GunBullet.GetComponent<BulletManagert>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Serect) 
        {
            SerectGun();
            Serect = false;
        }
    }

    public void SerectGun() 
    {
        switch (GunCount) 
        {
            case 0:
                Gun[0].SetActive(true);
                break;
            case 1:
                Gun[1].SetActive(true);
                break;
            case 2:
                Gun[2].SetActive(true);
                break;
            case 3:
                Gun[3].SetActive(true);
                break;
            default:
                break;
        }
        GunCount++;
    }
}
