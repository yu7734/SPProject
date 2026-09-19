using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Tooltip("ボスのゲームオブジェクト")] 
    public GameObject bossObject;

    public static BossManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    /// <summary> 時間制限が終わった直後、もしくはボス登場演出が終わった直後に呼び出す </summary>
    public void ActiveBoss()
    {

        bossObject.SetActive(true);
    }
}
