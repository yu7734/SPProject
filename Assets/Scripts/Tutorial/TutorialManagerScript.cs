using UnityEngine;
using TMPro;

enum Step { step_one, step_two,step_tree}
public class TutorialManagerScript : MonoBehaviour
{
    Step step = Step.step_one;
    [SerializeField] GameObject Avoid, TutorialTagetSpawnerManager;
    [SerializeField] PlayerManager PlayerManager;
    [SerializeField] UIManager UIManager;
    [SerializeField] TextMeshProUGUI StepText, DescriptionText;

    // Update is called once per frame
    void Update()
    {
        NextStep();
        switch (step)
        {
            case Step.step_one:
                if (!GameObject.FindGameObjectWithTag("Enemy")) 
                {
                    Avoid.SetActive(true);
                    step = Step.step_two;
                }
                break;
            case Step.step_two:
                if (UIManager.bSelect)
                {
                    TutorialTagetSpawnerManager.SetActive(true);
                    step = Step.step_tree;
                }
                break;
            default:
            case Step.step_tree:
                if(UIManager.experiencePoint!=0)UIManager.Experience(-UIManager.experiencePoint);
                break;
        }
        if(PlayerManager.playerHP <=20) PlayerManager.playerHP += 10;
    }

    void NextStep() 
    {
        switch (step) 
        {
            case Step.step_one:
                StepText.text = "Step 1";
                DescriptionText.text = "Break All Target\nWASD:Move\nEnter:Shot";
                break;
            case Step.step_two:
                StepText.text = "Step 2";
                DescriptionText.text = "Just Dodge Enemy Bullet\nSpace:Dodge";
                break;
            default:
            case Step.step_tree:
                StepText.text = "Step 3";
                DescriptionText.text = "Exit Tutorial";
                break;
        }
    }
}
