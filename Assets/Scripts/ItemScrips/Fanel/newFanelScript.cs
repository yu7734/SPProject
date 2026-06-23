using System.Linq;
using UnityEngine;

public enum Tracking { Off, On }
public class newFanelScript : MonoBehaviour
{
    [SerializeField] string SearchTag = "Enemy";
    FanelManager fanelManager;
    public Tracking Tracking = Tracking.Off;
    GameObject player;
    GameObject Enemy;
    [HideInInspector] public Vector3 offset = new Vector3(0f, 0f, -1.25f); // プレイヤーが動いていない場合の位置
    float smoothSpeed = 3f; // 追従の速さ
    Quaternion setup = Quaternion.identity;

    void Awake()
    {
        player = GameObject.Find("Player");//自機のオブジェクト名
        transform.position = player.transform.position + offset;//出現した時にプレイヤーの真後ろに生成
        setup = transform.rotation;
        GameObject fanelManagerObj = GameObject.Find("FanelManager");
        fanelManager = fanelManagerObj.GetComponent<FanelManager>();
    }

    void Start()
    {
        ++fanelManager.Fanelcount;
    }
    void Update()
    {
        if (player == null) Destroy(gameObject);

        Vector3 desiredPosition = player.transform.position + player.transform.rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        if (Tracking == Tracking.On) Enemy = GameObject.FindGameObjectsWithTag(SearchTag).OrderBy((GameObject e) => { float distance = float.MaxValue; if (player.transform.position.z < e.transform.position.z) distance = Vector3.Distance(player.transform.position, e.transform.position); return distance; }).FirstOrDefault();
        else { Enemy = null; transform.rotation = setup; }
        if (Enemy != null) transform.LookAt(Enemy.transform.position);
    }
}
