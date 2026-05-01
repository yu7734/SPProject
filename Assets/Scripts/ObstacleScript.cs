using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    GameObject Camera;//プレハブ化するのでプライベート
    void Start()
    {
        Camera = GameObject.Find("Main Camera");//名前指定なので変えると動かなくなる
    }
    void Update()
    {
        transform.parent = GameObject.Find("ObstacleManager").transform;//stageを変更する時にSestroy(this.gameObject)する必要あり
        float pos1 = this.transform.position.z;//Cameraの存在する方へ書き換えねばならない
        float pos2 = Camera.transform.position.z;//上記と同様
        transform.Translate(new Vector3(0f, 0f, -1f) * 10f * Time.deltaTime);//上記と同様
        if (pos1 <= pos2) Destroy(this.gameObject);//Cameraの後ろに行ったらこれのobjectを破壊する
    }
}
