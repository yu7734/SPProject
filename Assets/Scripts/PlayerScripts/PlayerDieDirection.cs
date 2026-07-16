using UnityEngine;

public class PlayerDieDirection : MonoBehaviour
{
    private PlayerManager playerManager;
    [SerializeField, Tooltip("BGMを参照")] 
    private AudioSource audio;
    [SerializeField, Tooltip("プレイヤーが爆発するエフェクト")]
    private GameObject playerExprosion;
    [SerializeField, Tooltip("爆発するSE")]
    private AudioClip[] exprosionSE;
    //サウンド管理システムを通してSEを鳴らす
    [SerializeField] private SoundManager sound;


    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerDie();
    }

    void PlayerDie()
    {
        if (playerManager.playerHP <= 0)
        {
            //BGMを止めプレイヤーを爆発させる
            audio.Stop();
            Instantiate(playerExprosion, this.transform.position, Quaternion.identity);
            sound.Play(exprosionSE[0]);
            sound.Play(exprosionSE[1]);
            gameObject.SetActive(false);
        }
    }
}
