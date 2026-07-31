using UnityEngine;
using UnityEngine.Rendering;

public class HealBallSpawnTimerScript : MonoBehaviour
{
    [SerializeField]GameObject HealBall;
    float timer = 0;
    [SerializeField]float SpawnTime = 60;
    ToggleGameObject toggle;
    void Awake()
    {
        if(HealBall == null)enabled = false;
        toggle = FindAnyObjectByType<ToggleGameObject>();
    }
    void Update()
    {
        if(toggle.GetSetIsStart)timer += Time.deltaTime;
        if (SpawnTime <= timer) 
        {
            Instantiate(HealBall,transform.position,Quaternion.identity);
            timer -= SpawnTime;
        }
    }
}
