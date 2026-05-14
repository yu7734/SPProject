using UnityEngine;

public class PlayerObjectManager : MonoBehaviour, IPlayerDamage
{
    //PlayerManagerを取得
    [SerializeField] private PlayerManager player;
    //CameraShakeを取得
    [SerializeField] public CameraShake cameraShake;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ダメージ処理
    public void Damage(int value)
    {
        //回避していない状態だと
        if (player._state == PlayerManager.dodgeState.None || player._state == PlayerManager.dodgeState.coolTime)
        {
            Debug.Log("ヒット");
            //カメラが振動する
            cameraShake.CameraShaker();
            //HPが減る
            player.playerHP -= Mathf.Max(0, value);
        }
    }
}
