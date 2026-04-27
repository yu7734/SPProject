using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private GameObject selectItemImage;
    [SerializeField] private PlayerManager player;
    [SerializeField] private int experiencePoint;
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
    }

    //体力のUI
    private void HPUI()
    {
        HPText.text = "HP : " + player.playerHP + " / 100";
    }

    //経験値UI
    private void ExperienceUI()
    {
        experienceText.text = experiencePoint + " / 100";
    }

    //経験値増加
    public void Experience(int point)
    {
        experiencePoint += point;
    }

    private void SelectItem()
    {
        if (experiencePoint >= 100)
        {
            experiencePoint %= 100;
            selectItemImage.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
