using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 「最後に操作されたデバイス」を監視して、Imageのスプライトを
/// キーボード&マウス版 / ゲームパッド版 に自動で切り替えるコンポーネント。
///
/// 使い方:
///   1. 操作説明の画像（Image）があるオブジェクトに AddComponent
///   2. Keyboard Mouse Sprite と Gamepad Sprite に各バージョンの画像を割り当て
///   3. パネル表示中にパッドを触ればパッド版、キーやマウスを触ればKB版に勝手に切り替わる
///
/// 判定は「接続されているか」ではなく「最後にどちらを操作したか」。
/// （パッドを挿したままマウスで遊ぶ人がいるため）
/// 最後に使ったデバイスはstaticに記憶されるので、パネルを開き直しても引き継がれる。
/// </summary>
public class DeviceSpriteSwitcher : MonoBehaviour
{
    [SerializeField, Tooltip("切り替える対象のImage。未指定なら自分のImageを使う")]
    private Image targetImage;
    [SerializeField, Tooltip("キーボード&マウス操作時に表示する画像")]
    private Sprite keyboardMouseSprite;
    [SerializeField, Tooltip("ゲームパッド操作時に表示する画像")]
    private Sprite gamepadSprite;
    [SerializeField, Tooltip("スティックをどれだけ倒したら「パッドを触った」とみなすか")]
    private float stickThreshold = 0.3f;

    /// <summary>最後に使われたのがゲームパッドか（シーンをまたいで記憶）</summary>
    private static bool lastUsedGamepad = false;
    private static bool initialized = false;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        // 初回だけ「パッドが刺さっていればパッド優先」で初期化
        if (!initialized)
        {
#if ENABLE_INPUT_SYSTEM
            lastUsedGamepad = Gamepad.current != null;
#endif
            initialized = true;
        }
    }

    private void OnEnable()
    {
        Apply();
    }

    private void Update()
    {
        bool before = lastUsedGamepad;

#if ENABLE_INPUT_SYSTEM
        // --- ゲームパッドを触った？ ---
        var pad = Gamepad.current;
        if (pad != null)
        {
            if (pad.leftStick.ReadValue().sqrMagnitude > stickThreshold * stickThreshold ||
                pad.rightStick.ReadValue().sqrMagnitude > stickThreshold * stickThreshold)
            {
                lastUsedGamepad = true;
            }
            else
            {
                foreach (var control in pad.allControls)
                {
                    if (control is UnityEngine.InputSystem.Controls.ButtonControl b && b.wasPressedThisFrame)
                    {
                        lastUsedGamepad = true;
                        break;
                    }
                }
            }
        }

        // --- キーボード / マウスを触った？ ---
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            lastUsedGamepad = false;
        }
        var mouse = Mouse.current;
        if (mouse != null &&
            (mouse.leftButton.wasPressedThisFrame ||
             mouse.rightButton.wasPressedThisFrame ||
             mouse.delta.ReadValue().sqrMagnitude > 4f))
        {
            lastUsedGamepad = false;
        }
#endif

        // 変化したときだけ画像を差し替える
        if (before != lastUsedGamepad)
        {
            Apply();
        }
    }

    private void Apply()
    {
        if (targetImage == null) return;
        var sprite = lastUsedGamepad ? gamepadSprite : keyboardMouseSprite;
        if (sprite != null)
        {
            targetImage.sprite = sprite;
        }
    }
}
