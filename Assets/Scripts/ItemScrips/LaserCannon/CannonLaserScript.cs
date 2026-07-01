using UnityEngine;
using UnityEngine.UIElements;

public class CannonLaserScript : MonoBehaviour
{
    [SerializeField,Tooltip("1•bŠÔ‚É‰½“x‰ñ“]‚·‚é‚©")] float RotateSpeed = 360f;
    void Update()
    {
        gameObject.transform.Rotate(0, 0, RotateSpeed*Time.deltaTime);
    }
}
