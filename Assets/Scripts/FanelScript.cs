using UnityEngine;

public class FanelScript : MonoBehaviour
{
    GameObject player;
    Vector3 offset = new Vector3(0f, 0f, -1f); // プレイヤーが動いていない場合の位置
    float smoothSpeed = 3f; // 追従の速さ

    void Start()
    {
        player = GameObject.Find("Player");//自機のオブジェクト名
        this.gameObject.transform.position = player.transform.position+offset;//出現した時にプレイヤーの真後ろに生成
    }
    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.transform.position + player.transform.rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        //transform.LookAt(Enemy);
    }
}
