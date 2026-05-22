using UnityEngine;

public class BossAttackScript : MonoBehaviour
{
    GameObject Camera;
    GameObject player;
    float timer = 0;
    bool Attackactive=true;
    float speed = 5;

    void Start()
    {
        Camera = GameObject.Find("Main Camera");
    }
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 1 && Attackactive)
        {
            player = GameObject.Find("Player");
            //対象の位置の方向を向く
            transform.LookAt(player.transform);
            speed = 30;
            Attackactive = false;
        }

        //自分自身の位置から相対的に移動する
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        float pos1 = this.transform.position.z;//Cameraの存在する方へ書き換えねばならない
        float pos2 = Camera.transform.position.z;//上記と同様
        //transform.Translate(new Vector3(0f, 0f, -1f) * 10f * Time.deltaTime);//上記と同様
        if (pos1 <= pos2) Destroy(this.gameObject);//Cameraの後ろに行ったらこれのobjectを破壊する

    }
}