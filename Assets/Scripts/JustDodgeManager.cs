using Unity.VisualScripting;
using UnityEngine;

public class JustDodgeManager : MonoBehaviour
{
    [SerializeField] private UIManager ui;

    [SerializeField] private PlayerManager player;
    private float slowTime = 0;
    private bool bslow = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ƒXƒ[‰‰o
        if (bslow)
        {
            Time.timeScale = 0.2f;
            slowTime += Time.deltaTime;
            if (slowTime >= 1f)
            {
                bslow = false;
                slowTime = 0f;
                Time.timeScale = 1f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && (player.bJustDodge == true))
        {
            player.bJustDodge = false;
            Debug.Log("Just");
            ui.Experience(10);
        }
    }
}
