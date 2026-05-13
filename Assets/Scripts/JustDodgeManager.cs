using UnityEngine;

public class JustDodgeManager : MonoBehaviour
{
    [SerializeField] private UIManager ui;

    [SerializeField] private PlayerManager player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && (player._state == PlayerManager.dodgeState.JustDodge))
        {
            Debug.Log("Just");
            ui.Experience(10);
        }
    }

    //public void JustDodge()
    //{
    //    justDodgeTime += Time.deltaTime;
    //    if (justDodgeTime >= 0.1f)
    //    {
    //        justDodgeTime = 0;
    //        player._state = PlayerManager.dodgeState.dodge;
    //    }
    //}
}
