using Unity.VisualScripting;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    private ToggleGameObject toggle;

    private void Awake()
    {
        toggle = new ToggleGameObject();
    }
}
