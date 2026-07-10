using UnityEngine;

public class AvoidShootScript : MonoBehaviour
{
    [SerializeField]GameObject AboidLaser;
    float timer=0;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            Instantiate(AboidLaser,transform.position,Quaternion.identity);
            timer = 0;
        }
    }
}
