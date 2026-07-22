using UnityEngine;

public class HealBallMoveScript : MonoBehaviour
{ 
    Vector3 playerpos;
    float smoothSpeed = 3f;
    void Awake()
    {
        playerpos=GameObject.Find("Player").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredPosition = playerpos;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
