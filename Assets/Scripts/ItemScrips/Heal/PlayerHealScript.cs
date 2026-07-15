using UnityEngine;

public class PlayerHealScript : MonoBehaviour
{
    public int Healing = 20;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.TryGetComponent<IPlayerHeal>(out var Heal)) Heal.Heal(Healing);
            Destroy(gameObject);
        }
    }
}
