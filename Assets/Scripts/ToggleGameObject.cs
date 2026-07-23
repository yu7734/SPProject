using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    private bool isStart = false;
    public void ActivateGameObject()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }

    public bool GetSetIsStart {
        get { return isStart; } 
        set { isStart = value; } 
    }

    public void GameStart() { GetSetIsStart = true; }
}
