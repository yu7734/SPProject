using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public int Stage=1;//stagemanager等のscriptで管理する場合はそちらを参照する
    public GameObject[] Stage1;//InspectorでStageのPrefabを追加する
    public GameObject[] Stage2;//Stageを変化させるときにgameObject.SetActive()でScrollobjectのtrue/false変えたほうがいい
    public GameObject[] Stage3;
    void Start()
    {
        
    }
    void Update()
    {
        switch(Stage){
            default:
            case 1:
                for(int i = 0; i < Stage1.Length; ++i)//今は0~4を順に出してる
                {
                    if(!GameObject.Find("Stage1_"+i)&&!GameObject.Find("Stage1_"+i+"(Clone)"))Instantiate(Stage1[i],new Vector3(i,0f,0f),Quaternion.identity);
                }
            break;

            case 2:
                GameObject[] tagobj1 = GameObject.FindGameObjectsWithTag("Stage2");//Stage2tagのついたobjectをtagobj1に配列化
                int j=Random.Range(0,Stage2.Length);
                if (tagobj1.Length < Stage2.Length) Instantiate(Stage2[j],new Vector3(j,0f,0f),Quaternion.identity);//tagobj1とStage2に割り当てられたPrefabの数を比較、ランダムに呼び出し
            break;
        }
    }
}
