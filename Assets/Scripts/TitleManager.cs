using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TitleManager : MonoBehaviour
{
    [Header("操作方法パネル")]
    [Tooltip("操作方法を表示するパネル（Inspectorからドラッグ＆ドロップ）")]
    [SerializeField] private GameObject howToPlayPanel;

    // パネルを開いた瞬間のキー押下を無視するためのフラグ
    private bool ignoreInputThisFrame = false;

    private void Start()
    {
        // 起動時は操作方法パネルを非表示にしておく
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // 操作方法パネルが表示されていないときは何もしない
        if (howToPlayPanel == null || !howToPlayPanel.activeSelf)
        {
            return;
        }

        // 開いた直後のフレームの入力は無視（ボタンを押した入力で即閉じてしまうのを防ぐ）
        if (ignoreInputThisFrame)
        {
            ignoreInputThisFrame = false;
            return;
        }

        // 任意のキー / ボタンが押されたらパネルを閉じる
        if (IsAnyKeyPressed())
        {
            OnCloseHowToPlayButton();
        }
    }

    // キーボード・マウス・ゲームパッドのいずれかが押されたかを判定
    private bool IsAnyKeyPressed()
    {
#if ENABLE_INPUT_SYSTEM
        // New Input System
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
        // 旧 Input Manager
        return Input.anyKeyDown;
#endif
    }

    // スタートボタンを押したときに呼ばれる
    public void OnStartButton()
    {
        // ゲームシーンに遷移（シーン名は実際のファイル名に合わせる）
        SceneManager.LoadScene("Game");
    }

    // 「操作方法」ボタンを押したときに呼ばれる
    public void OnHowToPlayButton()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
            // ボタンを押した入力で即閉じてしまわないように、今フレームの入力は無視する
            ignoreInputThisFrame = true;
        }
    }

    // 操作方法パネルの「閉じる」ボタンで呼ばれる
    public void OnCloseHowToPlayButton()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        // 閉じるときに専用の効果音を鳴らす（ButtonSoundManager がシーンにあれば）
        if (ButtonSoundManager.Instance != null)
        {
            ButtonSoundManager.Instance.PlayClose();
        }
    }

    // 終了ボタンを押したときに呼ばれる
    public void OnQuitButton()
    {
        // エディタ実行中はプレイ停止、ビルド後はアプリ終了
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
