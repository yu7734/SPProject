using UnityEngine;

public class StarScrollManager : MonoBehaviour
{
    public int Stage = 1;//stagemanager等のscriptで管理する場合はそちらを参照する
    public int limit = 25;//一度に存在できるStarの上限
    public GameObject[] Star1;//InspectorでStarのPrefabを追加する
    public GameObject[] Star2;//Stageを変化させるときにDestroy()で今存在しているStarを一掃したほうがいい
    public GameObject[] Star3;
    float timer = 0f;
    float frequency = 0.1f;
    void Start()
    {

    }
    void Update()
    {
        switch (Stage)
        {
            default:
            case 1:
                GameObject[] tagobj = GameObject.FindGameObjectsWithTag("Star");//Starのついたobjectをtagobjに配列化
                timer += Time.deltaTime;
                if (timer >= frequency)//frequency秒毎にStarを生成する
                {
                    int i = Random.Range(0, Star1.Length);
                    float z = Random.Range(0, 360);
                    if (tagobj.Length < limit) Instantiate(Star1[i],this.transform.position, Quaternion.Euler(0,0,z));//tagobjとlimitを比較、ランダムに呼び出し
                    timer -= frequency;
                }
                break;
        }
    }
}
