using UnityEngine;

public class LaserCannon : MonoBehaviour
{
    Vector3 offset = new Vector3(0f, 0f, -1f);
    GameObject player;
    [SerializeField] GameObject BigLaser;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.Find("Player");
        transform.position = player.transform.position + offset;
    }

    // Update is called once per frame
    void Update()
    {


        if (player == null) Destroy(gameObject);
        Instantiate(BigLaser);
    }
}
