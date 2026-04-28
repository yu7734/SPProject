using UnityEngine;

public class StageScript : MonoBehaviour
{
    GameObject Camera;//プレハブ化するのでプライベート
    GameObject ScrollManager;//プレハブに直接objのscriptの参照を割り当てられないので
    ScrollManager ScrollManagerScript;//objとscriptを格納変数を作って本Scriptで参照させる
    void Start()
    {
        Camera = GameObject.Find("Main Camera");//名前指定なので変えると動かなくなる
        ScrollManager = GameObject.Find("ScrollManager");//上記と同様
        ScrollManagerScript = ScrollManager.GetComponent<ScrollManager>();//ScrollManager(obj)についたScrollMannager(Script)のStage変数を取得、他にStage変数があるならば参照するobjを変えなければならない
    }
    void Update()
    {
        transform.parent = GameObject.Find("Scroll"+ScrollManagerScript.Stage).transform;//stageに応じて親を変える
        float pos1=this.transform.position.z;//Cameraの存在する方へ書き換えねばならない
        float pos2=Camera.transform.position.z;//上記と同様
        transform.Translate(new Vector3(0f,0f,-1f) * 10f*Time.deltaTime);//上記と同様
        if (pos1 <= pos2) Destroy(this.gameObject);//Cameraの後ろに行ったらこれのobjectを破壊する
    }
}
