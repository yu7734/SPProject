using UnityEngine;
using UnityEngine.UIElements;

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
        ui = FindAnyObjectByType<UIManager>();
        for (int i = 0; i < BigLaser.Length; i++) 
        {
            for (int j = 0; j < BigLaser[i].transform.childCount; j++) 
            { 
                BulletManagert bulletManagert = BigLaser[i].transform.GetChild(j).gameObject.GetComponent<BulletManagert>();
                bulletManagert.bulletDamageRate = bulletManagert.BASE_bulletDamageRate;
            }
        }
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
            // 安全チェック：BigLaser配列が空、またはLaserNumが範囲外なら何もしない
            if (BigLaser == null || BigLaser.Length == 0)
            {
                Debug.LogWarning("LaserCannon: BigLaser配列がアサインされていません。Inspectorで LaserSphere1/2/3 を設定してください。");
                return;
            }
            int index = Mathf.Clamp(LaserNum, 0, BigLaser.Length - 1);
            if (BigLaser[index] == null) return;
            Instantiate(BigLaser[index], transform.position + offset, transform.rotation);
        }
    }
}
