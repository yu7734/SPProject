using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

enum Step { step_one, step_two,step_tree}
public enum Language { JP,ENG}
public class TutorialManagerScript : MonoBehaviour
{
    Step step = Step.step_one;
    public Language language = Language.JP;
    bool TutorialTextActive = true;
    float Timer = 0f;
    [SerializeField] GameObject Avoid, TutorialTagetSpawnerManager;
    [SerializeField] PlayerManager PlayerManager;
    [SerializeField] UIManager UIManager;
    [SerializeField] TextMeshProUGUI StepText, DescriptionText,PanelText;
    [SerializeField] Image TutorialPanel;
    [SerializeField] GameObject ShotImagePanel,DodgeImagePanel,MoveImagePanel,PauseImagePanel;
    [SerializeField] TMP_FontAsset JP_Murecho, ENG_Orbiron;

    string[] StepTextString = new string[3];
    string[] DescriptionTextString = new string[3];
    string[] InstructionTextString = new string[10];
    private void Awake()
    {
        switch (language) 
        {
            default:
            case Language.ENG:
                StepTextString[0] = "Step 1";
                StepTextString[1] = "Step 2";
                StepTextString[2] = "Step 3";
                DescriptionTextString[0] = "Break All Target\nWASD/L-S:Move\nEnter/X/□:Shot";
                DescriptionTextString[1] = "Just Dodge\nEnemy Bullet\nSpace/A/×:Dodge";
                DescriptionTextString[2] = "Exit Tutorial\nESC/Start/≡:\nPause&Menu";
                InstructionTextString[0] = "Step 1\nBreak All Target\nWASD/Left-Stick:Move  Enter/X/□:Shot";
                InstructionTextString[1] = "Step 2\nIf you dodge an enemy's shot just before it hits you, you'll earn <color=#FF00FF>EXP</color>.\nKeep doing this until you level up.\nSpace/A/×:Dodge";
                InstructionTextString[2] = "Step 3\nAs you level up, you can choose one weapon from three options. if you choose the same one repeatedly, it will become stronger.\nExit the menu.  ESC/Start/≡:Pause&Menu";
                break;
            case Language.JP:
                StepTextString[0] = "Step 1";
                StepTextString[1] = "Step 2";
                StepTextString[2] = "Step 3";
                DescriptionText.font = JP_Murecho;
                DescriptionTextString[0] = "全てのターゲットを壊せ\nWASD/L-S:移動\nEnter/X/□:射撃";
                DescriptionTextString[1] = "敵の弾をギリギリで避けろ\nSpace/A/×:回避";
                DescriptionTextString[2] = "チュートリアルを抜ける\nESC/Start/≡:\nPause&Menu";
                PanelText.font = JP_Murecho;
                InstructionTextString[0] = "Step 1\n全てのターゲットを壊せ\nWASD/Left-Stick:移動  Enter/X/□:射撃";
                InstructionTextString[1] = "Step 2\n敵の弾を当たる直前で避けると<color=#FF00FF>EXP</color>がたまる\nレベルアップするまで繰り返せ\nSpace/A/×:回避";
                InstructionTextString[2] = "Step 3\nレベルが上がると武装を3択の中から1つ選べる、何度も同じ物を選んだ場合は強化されていく\nメニューから退出しろ  ESC/Start/≡:Pause&Menu";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.unscaledDeltaTime;
        ReadText();
        switch (step)
        {
            case Step.step_one:
                if (TutorialTextActive)
                {
                    NextStep();
                    TutorialText();
                }
                if (!GameObject.FindGameObjectWithTag(Tags.Enemy)) 
                {
                    Avoid.SetActive(true);
                    step = Step.step_two;
                    TutorialTextActive = true;
                }
                break;
            case Step.step_two:
                if (TutorialTextActive)
                {
                    NextStep();
                    TutorialText();
                }
                if (UIManager.bSelect)
                {
                    TutorialTagetSpawnerManager.SetActive(true);
                    step = Step.step_tree;
                    TutorialTextActive = true;
                }
                break;
            default:
            case Step.step_tree:
                if (TutorialTextActive)
                {
                    NextStep();
                    TutorialText();
                }
                if (UIManager.experiencePoint!=0)UIManager.Experience(-UIManager.experiencePoint);
                break;
        }
        if(PlayerManager.playerHP <=20) PlayerManager.playerHP += 10;
    }

    void NextStep()
    {
        switch (step)
        {
            case Step.step_one:
                StepText.text = StepTextString[0];
                DescriptionText.text = DescriptionTextString[0];
                break;
            case Step.step_two:
                StepText.text = StepTextString[1];
                DescriptionText.text = DescriptionTextString[1];
                break;
            default:
            case Step.step_tree:
                StepText.text = StepTextString[2];
                DescriptionText.text = DescriptionTextString[2];
                break;
        }
    }

    void TutorialText() 
    {
        Timer = 0f;
        Time.timeScale = 0f;
        switch (step) 
        {
            case Step.step_one:
                TutorialPanel.gameObject.SetActive(true);
                ShotImagePanel.SetActive(true);
                MoveImagePanel.SetActive(true);
                PanelText.text = InstructionTextString[0];
                break;
            case Step.step_two:
                TutorialPanel.gameObject.SetActive(true);
                DodgeImagePanel.SetActive(true);
                PanelText.text = InstructionTextString[1];
                break;
            case Step.step_tree:
                TutorialPanel.gameObject.SetActive(true);
                PauseImagePanel.SetActive(true);
                PanelText.text = InstructionTextString[2];
                break;
        }
        TutorialTextActive = false;
    }
    void ReadText() 
    {
        if (Timer > 1 && IsAnyKeyPressed()) 
        {
            TutorialPanel.gameObject.SetActive(false);
            ShotImagePanel.SetActive(false);
            MoveImagePanel.SetActive(false);
            DodgeImagePanel.SetActive(false);
            PauseImagePanel.SetActive(false);
            if(!UIManager.bSelect)Time.timeScale = 1f;
        }
    }

    bool IsAnyKeyPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return true;
        }
        if (Mouse.current != null &&
            (Mouse.current.leftButton.wasPressedThisFrame ||
             Mouse.current.rightButton.wasPressedThisFrame ||
             Mouse.current.middleButton.wasPressedThisFrame))
        {
            return true;
        }
        if (Gamepad.current != null)
        {
            foreach (var control in Gamepad.current.allControls)
            {
                if (control is UnityEngine.InputSystem.Controls.ButtonControl button &&
                    button.wasPressedThisFrame)
                {
                    return true;
                }
            }
        }
        return false;
#else
        return Input.anyKeyDown;
#endif
    }
}
