using UnityEngine;

public class EnemyClearScript : MonoBehaviour
{
    [SerializeField] private GameClearManager gameClearManager;
    private Renderer enemyRenderer;

    private void Awake()
    {
        enemyRenderer = GetComponent<Renderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
