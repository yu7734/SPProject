using UnityEngine;

public class HealBallMoveScript : MonoBehaviour
{ 
    Vector3 playerpos=new(0f,0f,-6.17f);
    [SerializeField,Tooltip("‹ß‚Ã‚­‘¬“x")]float smoothSpeed = 3f;
    float timer = 0f;
    [SerializeField,Tooltip("oŒ»‚µ‚Ä‚©‚ç‰½•bŒã‚ÉÁ‚¦‚é‚©")]float extinction = 4f;
    void Awake()
    {
        if(GameObject.Find(Tags.Player)!=null)playerpos=GameObject.Find(Tags.Player).transform.position;
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
