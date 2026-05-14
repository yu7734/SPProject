using UnityEngine;

public class BossTimerScript : MonoBehaviour
{
    [SerializeField] GameObject[] BossObject;
    public int Stage = 1;//stagemanager“™‚Ìscript‚ÅŠÇ—‚·‚éê‡‚Í‚»‚¿‚ç‚ðŽQÆ‚·‚é
    float Timer=0f;
    float frequency = 30f;
    bool appearance = false;
    void Start()
    {
        
    }

    void Update()
    {
        Timer += Time.deltaTime;
        if( Timer >= frequency&&!appearance)
        {
            Instantiate( BossObject[Stage-1],this.transform.position,Quaternion.identity);
            appearance = true;
        }
    }
}
