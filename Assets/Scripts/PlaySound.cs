using UnityEngine;

public class PlaySound : MonoBehaviour
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

    //îzóÒÇÃóvëfÇéQè∆
    public void PlaySE(int SENumber)
    {
        audioSource[SENumber].PlayOneShot(playSE[SENumber]);
    }
}