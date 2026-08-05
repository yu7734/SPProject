using UnityEngine;

public class NewScrollManager : MonoBehaviour
{
    public int Stage=1;//stagemanager等のscriptで管理する場合はそちらを参照する
    public GameObject[] StagePearent;
    public GameObject[] Stage1;//InspectorでStageのPrefabを追加する
    public GameObject[] Stage2;//Stageを変化させるときにgameObject.SetActive()でScrollobjectのtrue/false変えたほうがいい
    public GameObject[] Stage3;
    void Start()
    {
        Instantiate(StagePearent[0], transform.position, Quaternion.identity, transform);
        Instantiate(StagePearent[1], transform.position, Quaternion.identity, transform);
        Instantiate(StagePearent[2], transform.position, Quaternion.identity, transform);
        StagePearent[0]=GameObject.Find("Stage1(Clone)");
        StagePearent[1]=GameObject.Find("Stage2(Clone)");
        StagePearent[2]=GameObject.Find("Stage3(Clone)");
        for (int i = 0; i < Stage1.Length; ++i)Instantiate(Stage1[i], new Vector3(i, 0f, i), Quaternion.identity, StagePearent[0].transform);
        for (int i = 0; i < Stage2.Length; ++i)Instantiate(Stage2[i], new Vector3(i, 0f, i), Quaternion.identity, StagePearent[1].transform);
    }
    void Update()
    {
        switch(Stage){
            default:
            case 1:
                for(int i = 0; i < Stage1.Length; ++i)//今は0~4を順に出してる
                {
                    if(!GameObject.Find("Stage1_"+i)&&!GameObject.Find("Stage1_"+i+"(Clone)"))Instantiate(Stage1[i],new Vector3(i, 0f, 0f), Quaternion.identity, StagePearent[0].transform);
                }
            break;

            case 2:
                GameObject[] tagobj2 = GameObject.FindGameObjectsWithTag(Tags.Stage2);//Stage2tagのついたobjectをtagobj1に配列化
                int j=Random.Range(0,Stage2.Length);
                if (tagobj2.Length < Stage2.Length) Instantiate(Stage2[j],new Vector3(j, 0f, 0f), Quaternion.identity, StagePearent[1].transform);//tagobj1とStage2に割り当てられたPrefabの数を比較、ランダムに呼び出し
            break;
        }
    }
}
