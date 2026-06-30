using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// ポーズ画面の制御を行うクラス。
/// Escキーでポーズの開閉、Resume / Title / Quit のボタン処理を担当する。
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("ポーズ画面パネル")]
    [Tooltip("ポーズメニューのパネル（Inspectorからドラッグ＆ドロップ）")]
    [SerializeField] private GameObject pausePanel;

    [Header("操作説明パネル")]
    [Tooltip("操作説明（How To Play）のパネル。ポーズ中に開閉する")]
    [SerializeField] private GameObject howToPlayPanel;

    [Header("タイトルシーン名")]
    [Tooltip("Titleボタンで遷移するシーンの名前")]
    [SerializeField] private string titleSceneName = "Title";

    [Header("Resume後のクリック無視時間（秒）")]
    [Tooltip("Resume ボタンを押したクリックが射撃に化けないように、この時間だけプレイヤー入力を遮断する")]
    [SerializeField] private float inputIgnoreDuration = 0.15f;

    [Header("アイテム選択UIの参照")]
    [Tooltip("アイテム選択中はResumeしても時間を止めたままにするために参照する。未設定でも動作するが、設定推奨。")]
    [SerializeField] private UIManager uiManager;

    // 現在ポーズ中かどうか
    public bool IsPaused { get; private set; } = false;

    // 操作説明を開いた直後のフレーム入力を無視するためのフラグ
    private bool ignoreHowToPlayInputThisFrame = false;

    // ポーズ時に Rigidbody の速度を退避しておく（Resume時に復元する）
    private struct RigidbodyState
    {
        public Rigidbody rb;
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }
    private readonly List<RigidbodyState> savedRigidbodies = new List<RigidbodyState>();

    private void Start()
    {
        // 起動時はポーズパネルと操作説明パネルを非表示にし、ゲーム時間を通常通りにする
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        IsPaused = false;
    }

    private void Update()
    {
        // 操作説明パネルが開いているときは、任意のキーで閉じる（Escでポーズ解除は無効化）
        if (howToPlayPanel != null && howToPlayPanel.activeSelf)
        {
            // ボタンクリックの入力で即閉じてしまうのを防ぐ
            if (ignoreHowToPlayInputThisFrame)
            {
                ignoreHowToPlayInputThisFrame = false;
                return;
            }

            if (IsAnyKeyPressed())
            {
                OnCloseHowToPlayButton();
            }
            return; // 操作説明表示中は通常のEsc処理をスキップ
        }

        // Escキーが押されたらポーズ状態をトグル
        if (IsEscapePressed())
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Escキーが押されたかを判定（New Input System / 旧 Input Manager の両対応）
    private bool IsEscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    // キーボード/マウス/ゲームパッドのいずれかのキーが押されたかを判定
    private bool IsAnyKeyPressed()
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

    /// <summary>
    /// ゲームをポーズする
    /// </summary>
    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        // プレイヤー入力を一時停止（ポーズ中の誤操作・誤射撃を防ぐ）
        SetPlayerInputActive(false);

        // 現在動いている全Rigidbodyの速度を退避してから停止させる
        SaveAndFreezeAllRigidbodies();

        // ゲーム内の時間を停止（敵・弾・アニメーションなどすべて停止）
        Time.timeScale = 0f;
        IsPaused = true;
    }

    /// <summary>
    /// ポーズを解除してゲームを再開する。
    /// 「Resume」ボタンからも呼べる。
    /// </summary>
    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // アイテム選択中（bSelect == true）にポーズしていた場合、
        // ここで時間を動かしてしまうと「アイテム選択画面が出たまま時間が進む」バグになる。
        // そのため、アイテム選択中は timeScale を 0 のまま維持し、選択が終わるまで止めておく。
        if (uiManager != null && uiManager.bSelect)
        {
            Time.timeScale = 0f;
        }
        else
        {
            // 通常のポーズ解除：ゲーム内の時間を通常速度に戻す
            Time.timeScale = 1f;
        }

        // 退避していた速度を元に戻す
        RestoreAllRigidbodies();

        IsPaused = false;

        // Resume ボタンを押したクリックがそのまま射撃に化けないように、
        // 少し時間が経ってからプレイヤー入力を再度有効化する
        StartCoroutine(ReenablePlayerInputAfterDelay(inputIgnoreDuration));
    }

    /// <summary>
    /// タイトル画面に戻る。
    /// 「Title」ボタンから呼ばれる。
    /// </summary>
    public void OnTitleButton()
    {
        // シーン遷移前に必ず timeScale を戻す（戻し忘れると遷移先で全停止状態になる）
        Time.timeScale = 1f;
        IsPaused = false;
        savedRigidbodies.Clear();

        // 重要：InputActionAsset の Disable 状態はシーン遷移後も残ってしまうため、
        // ここで明示的に Enable に戻しておく。
        // （これをしないと、Title→Game に戻った時に自機操作が効かなくなる）
        SetPlayerInputActive(true);

        SceneManager.LoadScene(titleSceneName);
    }

    /// <summary>
    /// ゲームを終了する。
    /// 「Quit」ボタンから呼ばれる。
    /// </summary>
    public void OnQuitButton()
    {
        // エディタ実行中はプレイ停止、ビルド後はアプリ終了
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 操作説明パネルを開く。
    /// 「How To Play」ボタンから呼ばれる。
    /// </summary>
    public void OnHowToPlayButton()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
            // ボタンを押した入力で即閉じてしまわないように、今フレームの入力は無視する
            ignoreHowToPlayInputThisFrame = true;
        }
    }

    /// <summary>
    /// 操作説明パネルを閉じる。
    /// 操作説明パネル内の「閉じる」ボタンから呼ばれる。
    /// </summary>
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

    // シーン遷移時などに timeScale や InputActionAsset の状態が戻らないままになる事故を防ぐ
    private void OnDisable()
    {
        Time.timeScale = 1f;

        // ポーズ中にシーン遷移などで PauseManager が無効化されたときに、
        // InputActionAsset が Disable のまま残らないように戻しておく。
        // （Title ボタンを経由しないケースでの保険）
        SetPlayerInputActive(true);
    }

    // シーン上のすべての Rigidbody の速度を保存し、いったん 0 にして止める
    private void SaveAndFreezeAllRigidbodies()
    {
        savedRigidbodies.Clear();

        // シーン内のすべての Rigidbody を取得（非アクティブは除外）
        Rigidbody[] rigidbodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb == null || rb.isKinematic) continue;

            // 現在の速度を退避
            savedRigidbodies.Add(new RigidbodyState
            {
                rb = rb,
                velocity = rb.linearVelocity,
                angularVelocity = rb.angularVelocity
            });

            // いったん停止（スリープによるおかしな挙動を防ぐ）
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // 保存した速度を Rigidbody に戻す
    private void RestoreAllRigidbodies()
    {
        foreach (RigidbodyState state in savedRigidbodies)
        {
            if (state.rb == null) continue;
            // Destroy 済みでないことを確認しつつ復元
            state.rb.linearVelocity = state.velocity;
            state.rb.angularVelocity = state.angularVelocity;
            // 念のため起こしておく
            state.rb.WakeUp();
        }
        savedRigidbodies.Clear();
    }

    // シーン内のすべての PlayerInput を有効化 / 無効化する
    private void SetPlayerInputActive(bool active)
    {
#if ENABLE_INPUT_SYSTEM
        PlayerInput[] inputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (PlayerInput pi in inputs)
        {
            if (pi == null) continue;

            // PlayerInput 自体の有効/無効
            if (active)
            {
                pi.ActivateInput();
            }
            else
            {
                pi.DeactivateInput();
            }

            // さらに Actions（InputActionAsset）全体も明示的に Disable/Enable する。
            // これで Send Messages / Invoke Unity Events どのモードでも確実に入力を遮断できる。
            if (pi.actions != null)
            {
                if (active)
                {
                    pi.actions.Enable();
                }
                else
                {
                    pi.actions.Disable();
                }
            }
        }
#endif
    }

    // 少し待ってからプレイヤー入力を再有効化（Resume クリックの伝播を防ぐ）
    private IEnumerator ReenablePlayerInputAfterDelay(float delay)
    {
        // timeScale に左右されないように Realtime で待つ（timeScale=1 でも安全）
        yield return new WaitForSecondsRealtime(delay);
        SetPlayerInputActive(true);
    }
}
