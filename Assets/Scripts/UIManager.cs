using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private GameObject selectItemImage;
    [SerializeField] private PlayerManager player;
    //[SerializeField] private BulletManagert bullet;
    [SerializeField] private int experiencePoint;

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

    //体力のUI
    private void HPUI()
    {
        HPText.text = "HP : " + player.playerHP + " / " + player.MaxPlayerHP;
    }

    //経験値UI
    private void ExperienceUI()
    {
        experienceText.text = "EX : " + experiencePoint + " / 100";
    }

    //経験値増加
    public void Experience(int point)
    {
        experiencePoint += point;
    }

    //アイテム選択シーン

    private void SelectItem()
    {
        if (experiencePoint >= 100)
        {
            bSelect = true;
            experiencePoint %= 100;
            selectItemImage.SetActive(true);
            Time.timeScale = 0;
        }
    }

    //火力アップ
    public void PowerUp()
    {
        BulletManagert.bulletPower += 5;
        Time.timeScale = 1;
        bSelect = false;
        selectItemImage.SetActive(false);
    }

    //体力アップ
    public void HPUp()
    {
        player.MaxPlayerHP += 10;
        player.playerHP += 10;
        Time.timeScale = 1;
        bSelect = false;
        selectItemImage.SetActive(false);
    }
}
