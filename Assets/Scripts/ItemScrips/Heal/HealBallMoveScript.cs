using UnityEngine;

public class HealBallMoveScript : MonoBehaviour
{ 
    Vector3 playerpos;
    [SerializeField,Tooltip("‹ß‚Ã‚­‘¬“x")]float smoothSpeed = 3f;
    float timer = 0f;
    [SerializeField,Tooltip("oŒ»‚µ‚Ä‚©‚ç‰½•bŒã‚ÉÁ‚¦‚é‚©")]float extinction = 4f;
    void Awake()
    {
        playerpos=GameObject.Find(Tags.Player).transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, playerpos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        timer += Time.deltaTime;
        if(timer>extinction)Destroy(gameObject);
    }
}
