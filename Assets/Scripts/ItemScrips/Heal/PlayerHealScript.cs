using UnityEngine;

public class PlayerHealScript : MonoBehaviour
{
    public int Healing = 20;
    /// <summary> 回復量が増えるまでの時間 </summary>
    public float UntilIncreaseHealing = 30f;
    /// <summary> 一定時間毎の回復量の増加量 </summary>
    public float HealingRate = 5f;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            int TotalHealing = Healing + (int)(Time.timeSinceLevelLoad/ UntilIncreaseHealing * HealingRate);
            if (other.TryGetComponent<IPlayerHeal>(out var Heal)) Heal.Heal(TotalHealing);
            Destroy(gameObject);
        }
    }
}
