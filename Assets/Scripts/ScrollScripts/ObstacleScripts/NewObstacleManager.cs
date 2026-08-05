using UnityEngine;

public class NewObstacleManager : MonoBehaviour
{
    [SerializeField] int Stage = 1;//stagemanager等のscriptで管理する場合はそちらを参照する
    public int limit = 5;//一度に存在できるObstacleの上限
    public GameObject[] Obstacle1;//InspectorでObstacleのPrefabを追加する
    public GameObject[] Obstacle2;//Stageを変化させるときにDestroy()で今存在しているObstacleを一掃したほうがいい
    public GameObject[] Obstacle3;
    float timer=0f;
    float frequency = 1f;
    void Start()
    {
        
    }
    void Update()
    {
        switch (Stage)
        {
            default:
            case 1:
                GameObject[] tagobj = GameObject.FindGameObjectsWithTag(Tags.Obstacle);//Obstacleのついたobjectをtagobjに配列化
                timer += Time.deltaTime;
                if (timer >= frequency)//frequency秒毎にObstacleを生成する
                {
                    int i = Random.Range(0, Obstacle1.Length);
                    int x = Random.Range(0, 5);
                    int y = Random.Range(0, 5);
                    int z = Random.Range(0, 5);
                    if (tagobj.Length < limit) Instantiate(Obstacle1[i], new Vector3(x, y, z + transform.position.z), Quaternion.identity,transform);//tagobjとlimitを比較、ランダムに呼び出し
                    timer -= frequency;
                }
            break;
        }
    }
}
