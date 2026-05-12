using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform camera;
    [SerializeField] private Vector3 positionStrength;
    [SerializeField] private Vector3 rotationStrength;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraShaker()
    {
        //カメラの揺れアニメーションを完了させる関数
        camera.DOComplete();
        camera.DOShakePosition(1, positionStrength);
    }
}
