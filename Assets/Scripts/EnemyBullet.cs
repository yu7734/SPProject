using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField, Tooltip("弾の攻撃力。敵本体のダメージとは別")]
    private int attackPower = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            IPlayerDamage damage = other.GetComponent<IPlayerDamage>();
            if (damage != null)
            {
                damage.Damage(attackPower);
            }
        }

    }
}
