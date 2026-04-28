using UnityEngine;

public class StageScript : MonoBehaviour
{
    GameObject Camera;//プレハブ化するのでプライベート
    void Start()
    {
        Camera = GameObject.Find("Main Camera");//名前指定なので変えると動かなくなる
        transform.parent = GameObject.Find("ScrollManager").transform;//上記と同様
    }
    void Update()
    {
        float pos1=this.transform.position.z;
        float pos2=Camera.transform.position.z;
        transform.Translate(new Vector3(0f,0f,-1f) * 10f*Time.deltaTime);
        if (pos1 <= pos2) Destroy(this.gameObject);//Cameraの後ろに行ったらこれのobjectを破壊する
    }
}
