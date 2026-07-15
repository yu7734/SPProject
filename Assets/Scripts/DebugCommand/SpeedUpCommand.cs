using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedUpCommand : MonoBehaviour
{
    [SerializeField, Tooltip("ゲーム全体のスピード")] 
    private float gameSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameSpeedUp(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        //押している間はゲームのスピードが上がる
        if (context.performed)
        {
            Time.timeScale = gameSpeed;
        }
        else
        {
            Time.timeScale = 1;
        }
#endif
    }
}
