using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class StartBossBattle : MonoBehaviour
{
    [SerializeField] private GameObject bossObject;
    private PlayableDirector playableDirector;
    private bool isBossBattle;

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
        playableDirector.Play();//ムービーを再生
    }

    public void BossBattle()
    {
        //ボスが攻撃を開始
        isBossBattle = true;
    }

    public bool GetIsBossBattle { get { return isBossBattle; } }
}
