using UnityEngine;

public class PlaySEManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSource;
    [SerializeField] private AudioClip[] playSE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        audioSource = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //”z—ñ—v‘f‚ğQÆ‚µ‚ÄSE‚ğ–Â‚ç‚·
    public void PlaySE(int SENumber)
    {
        audioSource[SENumber].PlayOneShot(playSE[SENumber]);
    }
}
