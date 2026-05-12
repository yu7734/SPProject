using UnityEngine;

public class Testpos : MonoBehaviour
{
    float move = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*Debug.Log(timer);
        timer += Time.deltaTime;
        if(timer >= 1f)
        {
            int x = Random.Range(0, 5);
            int y = Random.Range(0, 5);
            int z = Random.Range(0, 5);
            this.gameObject.transform.position = new Vector3(x, y, z);
            timer -= 1;
        }*/
        transform.Translate(new Vector3(-1f, 0f, 0f) * move * Time.deltaTime);
        if (this.transform.position.x < -10f) move = -10f;
        if (this.transform.position.x > 10f) move = 10f;
    }
}
