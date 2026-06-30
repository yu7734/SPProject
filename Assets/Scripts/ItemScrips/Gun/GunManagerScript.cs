using UnityEngine;

public class GunManagerScript : MonoBehaviour
{
    [SerializeField] GameObject[] Gun;
    public int GunCount=0;

    // Update is called once per frame
    void Update()
    {
        if (!Gun[0].activeSelf && GunCount >= 1) Gun[0].SetActive(true);
        if (!Gun[1].activeSelf && GunCount >= 2) Gun[1].SetActive(true);
        if (!Gun[2].activeSelf && GunCount >= 3) Gun[2].SetActive(true);
        if (!Gun[3].activeSelf && GunCount >= 4) Gun[3].SetActive(true);
    }
}
