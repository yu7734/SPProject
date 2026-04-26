using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerManager player;
    //ダメージ量
    [SerializeField] private int attackPower;
    [SerializeField] private float enemySpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= Vector3.forward * enemySpeed * Time.deltaTime;
    }

    //プレイヤーに与えるダメージ量
    public void PlayerDamage(PlayerManager player)
    {
        player.Damage(attackPower);
    }
}
