using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 positionStrength = new Vector3(1, 1, 0);

    public void CameraShaker()
    {
        Debug.Log("カメラシェイク");
        //カメラの揺れアニメーションを完了させる関数
        cameraTransform.transform.DOComplete();
        cameraTransform.DOShakePosition(0.5f, positionStrength);
    }
}
