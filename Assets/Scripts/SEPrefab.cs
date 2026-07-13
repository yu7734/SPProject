using UnityEngine;

public class SEPrefab : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayingSE();
    }

    private void PlayingSE()
    {
        //çƒê∂íÜÇ»ÇÁreturnÇ≈ï‘Ç∑
        if (audioSource.isPlaying) return;
        Destroy(this.gameObject);
    }
}
