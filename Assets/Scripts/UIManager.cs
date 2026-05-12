using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private GameObject selectItemImage;
    [SerializeField] private PlayerManager player;
    //[SerializeField] private BulletManagert bullet;
    [SerializeField] private int _experiencePoint;

    // 外部からの読み取り用プロパティ（HUDPanel / EXPBar からアクセスされる）
    public int experiencePoint => _experiencePoint;

    public bool bSelect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectItemImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HPUI();
        ExperienceUI();
        SelectItem();
    }

    //�̗͂�UI
    private void HPUI()
    {
        HPText.text = "HP : " + player.playerHP + " / " + player.MaxPlayerHP;
    }

    //�o���lUI
    private void ExperienceUI()
    {
        experienceText.text = "EX : " + _experiencePoint + " / 100";
    }

    //�o���l����
    public void Experience(int point)
    {
        _experiencePoint += point;
    }

    //�A�C�e���I���V�[��

    private void SelectItem()
    {
        if (_experiencePoint >= 100)
        {
            bSelect = true;
            _experiencePoint %= 100;
            selectItemImage.SetActive(true);
            Time.timeScale = 0;
        }
    }

    //�ἨA�b�v
    public void PowerUp()
    {
        BulletManagert.bulletPower += 5;
        Time.timeScale = 1;
        bSelect = false;
        selectItemImage.SetActive(false);
    }

    //�̗̓A�b�v
    public void HPUp()
    {
        player.MaxPlayerHP += 10;
        player.playerHP += 10;
        Time.timeScale = 1;
        bSelect = false;
        selectItemImage.SetActive(false);
    }
}
