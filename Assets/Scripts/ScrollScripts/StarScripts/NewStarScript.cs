using UnityEngine;

public class NewStarScript : MonoBehaviour
{
    [SerializeField,Header("生成する星の大きさや色を変更する場合はここを変更")]GameObject Star;
    GameObject Camera;//プレハブ化するのでプライベート
    [SerializeField] float Distance = 11f;
    [SerializeField] float Speed = 50f;
    void Start()
    {
        Camera = GameObject.Find("Main Camera");//名前指定なので変えると動かなくなる
        Instantiate(Star, transform.position+transform.up*Distance, Quaternion.identity, transform);//stageを変更する時にSestroy(this.gameObject)する必要あり
    }
    void Update()
    {
        float pos1 = transform.position.z;//Cameraの存在する方へ書き換えねばならない
        float pos2 = Camera.transform.position.z;//上記と同様
        transform.Translate(new Vector3(0f, 0f, -1f) * Speed * Time.deltaTime);//上記と同様
        if (pos1 <= pos2) Destroy(gameObject);//Cameraの後ろに行ったらこれのobjectを破壊する
    }
}
