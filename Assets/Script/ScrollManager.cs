using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public int Stage=1;//stagemanager“™‚Ìscript‚ÅŠÇ—‚·‚éê‡‚Í‚»‚¿‚ç‚ğQÆ‚·‚é
    public GameObject[] Stage1;
    public GameObject[] Stage2;
    public GameObject[] Stage3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(Stage){
            default:
            case 1:
                for(int i = 0; i < Stage1.Length; ++i)
                {
                    if(!GameObject.Find("Stage1_"+i)&&!GameObject.Find("Stage1_"+i+"(Clone)"))Instantiate(Stage1[i],new Vector3(i,0f,0f),Quaternion.identity);
                }
            break;
        }
    }
}
