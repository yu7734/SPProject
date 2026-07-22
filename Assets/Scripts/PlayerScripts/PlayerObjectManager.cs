using UnityEngine;

public class PlayerObjectManager : MonoBehaviour, IPlayerDamage, IPlayerHeal
{
    [SerializeField, Tooltip("PlayerManagerを取得")] private PlayerManager player;
    [SerializeField, Tooltip("CameraShake取得")] public CameraShake cameraShake;

    [SerializeField] private SoundManager soundManager;
    [SerializeField] AudioClip playerHitedSE;

    //ダメージ処理
    public void Damage(int value)
    {
        //回避していないかクールタイムか
        if (player._state == PlayerManager.dodgeState.None || player._state == PlayerManager.dodgeState.coolTime)
        {
            Debug.Log("hit");
            //カメラを揺らす
            cameraShake.CameraShaker();

            //SEを鳴らす
            soundManager.Play(playerHitedSE);
            //HP減少
            player.playerHP -= Mathf.Max(0, value);
        }
    }

    //回復処理
    public void Heal(int value)
    {
        Debug.Log("Heal");
        //ここにカメラ効果とSE
        //HP回復
        player.playerHP += Mathf.Max(0, value);
        if (player.MaxPlayerHP < player.playerHP) player.playerHP = player.MaxPlayerHP;
    }
}
