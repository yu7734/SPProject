using UnityEngine;

public class StageScript : MonoBehaviour
{
    GameObject Camera;//プレハブ化するのでプライベート
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Camera = GameObject.Find("Main Camera");//名前指定なので変えると動かなくなる
        transform.parent = GameObject.Find("ScrollManager").transform;//上記と同様
    }

    // Update is called once per frame
    void Update()
    {
        float pos1=this.transform.position.z;
        float pos2=Camera.transform.position.z;
        transform.Translate(new Vector3(0f,0f,-1f) * 10f*Time.deltaTime);
        if (pos1 <= pos2) Destroy(this.gameObject);
    }
}
