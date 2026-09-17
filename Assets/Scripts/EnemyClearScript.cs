using UnityEngine;

public class EnemyClearScript : MonoBehaviour
{
    private GameClearTimer gameClearTimer;
    private Material enemyRenderer;
    private float clearTimer;
    private float limitClearTime = 1;

    private void Awake()
    {
        enemyRenderer = GetComponent<Renderer>().material;
        gameClearTimer = FindAnyObjectByType<GameClearTimer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clearTimer = limitClearTime;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyClear();
    }

    private void EnemyClear()
    {
        if (gameClearTimer.GetRemaningTime <= 0)
        {
            clearTimer -= Time.deltaTime;
            float enemyAlpha = clearTimer / limitClearTime;
            enemyRenderer.SetFloat("_Alpha", enemyAlpha);
            if (clearTimer >= 0) return;
            clearTimer = 0;
            Destroy(this.gameObject);
        }
    }
}
