using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform camera;
    [SerializeField] private Vector3 positionStrength;
    [SerializeField] private Vector3 rotationStrength;

    public void CameraShaker()
    {
        //カメラの揺れアニメーションを完了させる関数
        camera.DOComplete();
        camera.DOShakePosition(2, positionStrength);
    }
}
