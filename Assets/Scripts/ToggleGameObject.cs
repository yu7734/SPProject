using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    public void ActivateGameObject()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }
}
