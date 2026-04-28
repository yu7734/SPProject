using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public int Stage=1;//stagemanager“™‚Ìscript‚ÅŠÇ—‚·‚éê‡‚Í‚»‚¿‚ç‚ğQÆ‚·‚é
    public GameObject[] Stage1;//Inspector‚ÅStage‚ÌPrefab‚ğ’Ç‰Á‚·‚é
    public GameObject[] Stage2;
    public GameObject[] Stage3;
    void Start()
    {
        
    }
    void Update()
    {
        switch(Stage){
            default:
            case 1:
                for(int i = 0; i < Stage1.Length; ++i)//¡‚Í0~4‚ğ‡‚Éo‚µ‚Ä‚é
                {
                    if(!GameObject.Find("Stage1_"+i)&&!GameObject.Find("Stage1_"+i+"(Clone)"))Instantiate(Stage1[i],new Vector3(i,0f,0f),Quaternion.identity);
                }
            break;
        }
    }
}
