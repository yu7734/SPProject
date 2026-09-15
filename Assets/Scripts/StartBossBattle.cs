using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class StartBossBattle : MonoBehaviour
{
    [SerializeField] private GameObject bossObject;
    private PlayableDirector playableDirector;

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    private void OnEnable()
    {
        MoviePlay();
    }

    public void MoviePlay()
    {
        bossObject.SetActive(true);
        playableDirector.Play();//ƒ€[ƒr[‚ğÄ¶
    }
}
