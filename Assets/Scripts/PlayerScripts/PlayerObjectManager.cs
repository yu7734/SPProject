using UnityEngine;

public class PlayerObjectManager : MonoBehaviour, IPlayerDamage
{
    [SerializeField, Tooltip("PlayerManagerを取得")] private PlayerManager player;
    [SerializeField, Tooltip("CameraShake取得")] public CameraShake cameraShake;

    //ダメージ処理
    public void Damage(int value)
    {
        //回避していないかクールタイムか
        if (player._state == PlayerManager.dodgeState.None || player._state == PlayerManager.dodgeState.coolTime)
        {
            Debug.Log("hit");
            //カメラを揺らす
            cameraShake.CameraShaker();
            //HP減少
            player.playerHP -= Mathf.Max(0, value);
        }
    }
}
