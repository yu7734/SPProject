using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    public bool isStart;
    public void ActivateGameObject()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }

    public void GameStart()
    {
        isStart = true;
    }
}
