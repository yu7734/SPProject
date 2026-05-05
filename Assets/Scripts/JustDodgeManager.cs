using UnityEngine;

public class JustDodgeManager : MonoBehaviour
{
    private float justDodgeTime = 0;

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
        if (other.gameObject.CompareTag("Enemy") && (player._state == PlayerManager.dodgeState.JustDodge))
        {

        }
    }

    public void JustDodge()
    {
        justDodgeTime += Time.deltaTime;
        if (justDodgeTime >= 0.1f)
        {
            player._state = PlayerManager.dodgeState.dodge;
        }
    }
}
