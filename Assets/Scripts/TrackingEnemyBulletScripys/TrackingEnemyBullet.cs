using UnityEngine;

enum Pattern 
{
    Patten_one, Patten_two
}
public class TrackingEnemyBullet : MonoBehaviour
{
    GameObject Camera;
    GameObject player;
    float timer = 0;
    bool Attackactive = true;
    [SerializeField,Header("1,一瞬の追尾\n2,プレイヤーの目の前まで追尾")] Pattern pattern;
    [SerializeField,Header("追尾前の速度")] float primary_speed = 5;
    [SerializeField,Header("追尾時の速度")] float secondary_speed = 30;
    [SerializeField,Header("追尾するまでの時間")] float interval = 1;
    [SerializeField,Header("追尾をやめる距離")] float distance = 5;

    void Awake()
    {
        Camera = GameObject.Find("Main Camera");
        player = GameObject.Find("Player");
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > interval && Attackactive)
        {
            //対象の位置の方向を向く
            transform.LookAt(player.transform);
            primary_speed = secondary_speed;

            switch (pattern) 
            { 
                case Pattern.Patten_one:
                    Attackactive = false;
                    break;
                case Pattern.Patten_two:
                    if (gameObject.transform.position.z < player.transform.position.z + distance) Attackactive = false;
                    break;
            }
        }

        //自分自身の位置から相対的に移動する
        transform.Translate(Vector3.forward * primary_speed * Time.deltaTime);

        float pos1 = transform.position.z;//Cameraの存在する方へ書き換えねばならない
        float pos2 = Camera.transform.position.z;//上記と同様
        if (pos1 <= pos2) Destroy(gameObject);//Cameraの後ろに行ったらこれのobjectを破壊する

    }
}
