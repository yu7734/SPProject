using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

/// <summary>
/// 設定（Settings）パネルの本体。音量3種と操作の反転を変更できる。
///
/// UIは「Tools > SpacePhantom > 設定パネル(Settings)を作成」を1回実行すると
/// シーン上に本物のオブジェクトとして生成される。
/// 生成後はヒエラルキーに並ぶので、位置・色・文字を普通に手で編集できるし、
/// シーンを保存すればそのまま残る（再生しないと出てこない、という事は無くなる）。
///
/// このスクリプトは生成されたUIの参照を持っていて、実行時は
///   ・現在の設定値をUIに反映する
///   ・開閉とキャンセルキーの処理をする
/// だけを担当する。
///
/// 参照が空のまま再生した場合だけ、保険として実行時にUIを自動生成する。
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("===== メニューへのボタン追加 =====")]
    [SerializeField, Tooltip("既存メニューに Settings ボタンを追加する")]
    private bool addButtonToMenu = true;

    [SerializeField, Tooltip("追加するボタンに表示する文字")]
    private string menuButtonLabel = "Settings";

    [SerializeField, Tooltip("複製元にするボタン。未設定なら下の名前でシーンから探す")]
    private Button templateButton;

    [SerializeField, Tooltip("複製元ボタンのオブジェクト名（タイトル画面の終了ボタン）")]
    private string templateButtonName = "quitButton";

    [SerializeField, Tooltip("ON=複製元(Quit)の1つ上に入れる / OFF=1つ下に入れる")]
    private bool insertAboveTemplate = true;

    [SerializeField, Tooltip("ボタンが1つ増えた分、メニュー全体の縦位置を組み直す")]
    private bool relayoutMenu = true;

    [Header("===== パネルの文言 =====")]
    [SerializeField] private string panelTitleLabel = "SETTINGS";
    [SerializeField] private string masterLabel = "Master";
    [SerializeField] private string bgmLabel = "BGM";
    [SerializeField] private string seLabel = "SE";
    [SerializeField] private string invertYLabel = "Invert Vertical";
    [SerializeField] private string invertXLabel = "Invert Horizontal";
    [SerializeField] private string resetLabel = "Reset";
    [SerializeField] private string closeLabel = "Close";
    [SerializeField] private string onLabel = "ON";
    [SerializeField] private string offLabel = "OFF";

    [Header("===== 見た目 =====")]
    [SerializeField, Tooltip("パネルの大きさ（Canvasの基準解像度 800x600 に対する値）")]
    private Vector2 windowSize = new Vector2(560f, 420f);

    [SerializeField, Tooltip("強調色（スライダーの色・選択中の色）")]
    private Color accentColor = new Color(0.2f, 1f, 1f, 1f);

    [SerializeField, Tooltip("パネルの背景色")]
    private Color windowColor = new Color(0.03f, 0.05f, 0.1f, 0.96f);

    [SerializeField, Tooltip("画面全体を暗くする幕の色")]
    private Color dimColor = new Color(0f, 0f, 0f, 0.75f);

    [SerializeField, Tooltip("文字色")]
    private Color textColor = Color.white;

    [SerializeField, Tooltip("文字のフォント。未設定なら複製元ボタンのフォントを使う")]
    private TMP_FontAsset fontOverride;

    [Header("===== 動作 =====")]
    [SerializeField, Tooltip("Escape / ゲームパッドのBボタンで閉じる")]
    private bool closeWithCancelKey = true;

    [SerializeField, Tooltip("開いている間 Time.timeScale を0にする（ゲーム中に開く場合用）")]
    private bool pauseWhileOpen = false;

    [Header("===== 生成されたUIの参照（作成ツールが自動で入れます） =====")]
    [SerializeField, Tooltip("パネル全体のルート。これをON/OFFして開閉する")]
    private GameObject panelRoot;
    [SerializeField] private Canvas panelCanvas;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private TextMeshProUGUI masterValueText;
    [SerializeField] private TextMeshProUGUI bgmValueText;
    [SerializeField] private TextMeshProUGUI seValueText;
    [SerializeField] private Button invertYButton;
    [SerializeField] private Button invertXButton;
    [SerializeField] private TextMeshProUGUI invertYStateText;
    [SerializeField] private TextMeshProUGUI invertXStateText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;
    [SerializeField, Tooltip("メニューに追加された Settings ボタン")]
    private Button menuButton;
    [SerializeField, Tooltip("パネルを開いた時に最初に選択されるUI")]
    private Selectable firstSelectable;

    // メニューを組み直す前の位置（削除ツールで元に戻すために覚えておく）
    [SerializeField, HideInInspector] private List<RectTransform> movedMenuButtons = new List<RectTransform>();
    [SerializeField, HideInInspector] private List<Vector2> movedMenuButtonOriginalPositions = new List<Vector2>();

    private TMP_FontAsset uiFont;
    private GameObject previousSelected;
    private bool ignoreCancelThisFrame = false;
    private bool buildingInEditor = false;
    private float timeScaleBeforeOpen = 1f;

    /// <summary>パネルが開いているか</summary>
    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

    /// <summary>生成済みかどうか（エディタツールから見る用）</summary>
    public bool HasUI => panelRoot != null;

    private void Start()
    {
        // 参照が空＝ツールでの生成をしていない場合の保険。実行時に組み立てる
        if (panelRoot == null)
        {
            BuildUI(false);
        }

        ApplyValuesToUI();

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOpen) return;

        // 開いた瞬間の入力で即閉じてしまうのを防ぐ
        if (ignoreCancelThisFrame)
        {
            ignoreCancelThisFrame = false;
            return;
        }

        if (closeWithCancelKey && IsCancelPressed())
        {
            Close();
        }
    }

    // ==================================================================
    // 外から呼べる操作（ボタンの OnClick に割り当て済み）
    // ==================================================================

    /// <summary>設定パネルを開く</summary>
    public void Open()
    {
        if (panelRoot == null) return;
        if (IsOpen) return;

        previousSelected = (EventSystem.current != null) ? EventSystem.current.currentSelectedGameObject : null;

        ApplyValuesToUI();
        panelRoot.SetActive(true);
        ignoreCancelThisFrame = true;

        if (pauseWhileOpen)
        {
            timeScaleBeforeOpen = Time.timeScale;
            Time.timeScale = 0f;
        }

        if (EventSystem.current != null && firstSelectable != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
        }
    }

    /// <summary>設定パネルを閉じる</summary>
    public void Close()
    {
        if (panelRoot == null) return;
        if (!IsOpen) return;

        panelRoot.SetActive(false);

        if (pauseWhileOpen)
        {
            Time.timeScale = timeScaleBeforeOpen;
        }

        if (ButtonSoundManager.Instance != null)
        {
            ButtonSoundManager.Instance.PlayClose();
        }

        // 開く前に選択していたボタンへ戻す（無ければ Settings ボタン）
        GameObject back = previousSelected;
        if (back == null && menuButton != null)
        {
            back = menuButton.gameObject;
        }
        if (EventSystem.current != null && back != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(back);
        }
    }

    /// <summary>開いていたら閉じる、閉じていたら開く</summary>
    public void Toggle()
    {
        if (IsOpen) Close();
        else Open();
    }

    /// <summary>全体音量を変更する（スライダーの OnValueChanged 用）</summary>
    public void SetMasterVolume(float value)
    {
        if (GameSettings.Instance != null) GameSettings.Instance.MasterVolume = value;
        RefreshTexts();
    }

    /// <summary>BGM音量を変更する（スライダーの OnValueChanged 用）</summary>
    public void SetBgmVolume(float value)
    {
        if (GameSettings.Instance != null) GameSettings.Instance.BgmVolume = value;
        RefreshTexts();
    }

    /// <summary>SE音量を変更する（スライダーの OnValueChanged 用）</summary>
    public void SetSeVolume(float value)
    {
        if (GameSettings.Instance != null) GameSettings.Instance.SeVolume = value;
        RefreshTexts();
    }

    /// <summary>上下反転を切り替える（ボタンの OnClick 用）</summary>
    public void ToggleInvertY()
    {
        if (GameSettings.Instance != null) GameSettings.Instance.InvertY = !GameSettings.Instance.InvertY;
        RefreshTexts();
    }

    /// <summary>左右反転を切り替える（ボタンの OnClick 用）</summary>
    public void ToggleInvertX()
    {
        if (GameSettings.Instance != null) GameSettings.Instance.InvertX = !GameSettings.Instance.InvertX;
        RefreshTexts();
    }

    /// <summary>設定を初期値に戻す（ボタンの OnClick 用）</summary>
    public void ResetSettings()
    {
        if (GameSettings.Instance != null) GameSettings.Instance.ResetToDefault();
        ApplyValuesToUI();
    }

    private bool IsCancelPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return true;
        }
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            return true;
        }
        return false;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    // ==================================================================
    // 表示の更新
    // ==================================================================

    /// <summary>設定値をUIに反映する（スライダーを動かしてもイベントは発火させない）</summary>
    public void ApplyValuesToUI()
    {
        GameSettings settings = GameSettings.Instance;

        float master = (settings != null) ? settings.MasterVolume : GameSettings.DefaultMasterVolume;
        float bgm = (settings != null) ? settings.BgmVolume : GameSettings.DefaultBgmVolume;
        float se = (settings != null) ? settings.SeVolume : GameSettings.DefaultSeVolume;

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(master);
        if (bgmSlider != null) bgmSlider.SetValueWithoutNotify(bgm);
        if (seSlider != null) seSlider.SetValueWithoutNotify(se);

        RefreshTexts();
    }

    /// <summary>数値表示と ON/OFF 表示を更新する</summary>
    private void RefreshTexts()
    {
        GameSettings settings = GameSettings.Instance;

        float master = (settings != null) ? settings.MasterVolume : GameSettings.DefaultMasterVolume;
        float bgm = (settings != null) ? settings.BgmVolume : GameSettings.DefaultBgmVolume;
        float se = (settings != null) ? settings.SeVolume : GameSettings.DefaultSeVolume;
        bool invertY = (settings != null) ? settings.InvertY : GameSettings.DefaultInvertY;
        bool invertX = (settings != null) ? settings.InvertX : GameSettings.DefaultInvertX;

        if (masterValueText != null) masterValueText.text = ToPercent(master);
        if (bgmValueText != null) bgmValueText.text = ToPercent(bgm);
        if (seValueText != null) seValueText.text = ToPercent(se);

        SetStateText(invertYStateText, invertY);
        SetStateText(invertXStateText, invertX);
    }

    private void SetStateText(TextMeshProUGUI target, bool isOn)
    {
        if (target == null) return;
        target.text = isOn ? onLabel : offLabel;
        target.color = isOn ? accentColor : new Color(0.6f, 0.6f, 0.6f, 1f);
    }

    private static string ToPercent(float value)
    {
        return Mathf.RoundToInt(value * 100f) + "%";
    }

    // ==================================================================
    // UIの生成（エディタツールと、保険の実行時生成の両方から呼ばれる）
    // ==================================================================

    /// <summary>
    /// 設定パネルのUIを生成する。
    /// editorMode が true なら Undo に対応し、ボタンのOnClickも
    /// Inspector に見える形（Persistent Listener）で登録する。
    /// </summary>
    public void BuildUI(bool editorMode)
    {
        buildingInEditor = editorMode;

        ResolveTemplateAndFont();
        BuildPanel();

        if (addButtonToMenu)
        {
            InsertMenuButton();
        }

        WireEvents();
        ApplyValuesToUI();

        buildingInEditor = false;
    }

    /// <summary>生成したUIを消して、メニューの並びも元に戻す</summary>
    public void ClearUI(bool editorMode)
    {
        buildingInEditor = editorMode;

        // メニューの位置を元に戻す
        for (int i = 0; i < movedMenuButtons.Count; ++i)
        {
            RectTransform rect = movedMenuButtons[i];
            if (rect == null) continue;
            if (i >= movedMenuButtonOriginalPositions.Count) break;

            RecordUndo(rect, "Remove Settings UI");
            rect.anchoredPosition = movedMenuButtonOriginalPositions[i];
        }
        movedMenuButtons.Clear();
        movedMenuButtonOriginalPositions.Clear();

        if (menuButton != null)
        {
            DestroyUIObject(menuButton.gameObject);
            menuButton = null;
        }

        if (panelCanvas != null)
        {
            DestroyUIObject(panelCanvas.gameObject);
        }
        else if (panelRoot != null)
        {
            DestroyUIObject(panelRoot);
        }

        panelCanvas = null;
        panelRoot = null;
        masterSlider = null;
        bgmSlider = null;
        seSlider = null;
        masterValueText = null;
        bgmValueText = null;
        seValueText = null;
        invertYButton = null;
        invertXButton = null;
        invertYStateText = null;
        invertXStateText = null;
        resetButton = null;
        closeButton = null;
        firstSelectable = null;

        buildingInEditor = false;
    }

    private void ResolveTemplateAndFont()
    {
        if (templateButton == null && !string.IsNullOrEmpty(templateButtonName))
        {
            GameObject found = GameObject.Find(templateButtonName);
            if (found != null)
            {
                templateButton = found.GetComponent<Button>();
            }
        }

        uiFont = fontOverride;
        if (uiFont == null && templateButton != null)
        {
            TextMeshProUGUI templateText = templateButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (templateText != null)
            {
                uiFont = templateText.font;
            }
        }
    }

    private void BuildPanel()
    {
        // --- 専用のCanvas（既存UIより手前に出す） ---
        GameObject canvasObj = new GameObject("SettingsCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObj.transform.SetParent(transform, false);
        canvasObj.layer = LayerMask.NameToLayer("UI");
        RegisterCreated(canvasObj);

        panelCanvas = canvasObj.GetComponent<Canvas>();
        panelCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        panelCanvas.sortingOrder = 200;

        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800f, 600f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0f; // 既存Canvasと同じ「横基準」

        // --- 画面全体を暗くする幕（後ろのボタンを押せなくする役割も兼ねる） ---
        Image dim = CreateImage("Panel", canvasObj.transform, dimColor);
        Stretch(dim.rectTransform);
        dim.raycastTarget = true;
        panelRoot = dim.gameObject;

        // --- パネル本体（枠＋中身） ---
        Image frame = CreateImage("Window", panelRoot.transform, new Color(accentColor.r, accentColor.g, accentColor.b, 0.55f));
        frame.rectTransform.anchorMin = frame.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        frame.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        frame.rectTransform.sizeDelta = windowSize;
        frame.rectTransform.anchoredPosition = Vector2.zero;

        Image inner = CreateImage("Inner", frame.transform, windowColor);
        Stretch(inner.rectTransform);
        inner.rectTransform.offsetMin = new Vector2(2f, 2f);
        inner.rectTransform.offsetMax = new Vector2(-2f, -2f);

        Transform content = inner.transform;
        float halfHeight = windowSize.y * 0.5f;

        // --- 見出し ---
        TextMeshProUGUI title = CreateText("Title", content, panelTitleLabel, 34f, TextAlignmentOptions.Center, accentColor);
        title.rectTransform.anchorMin = title.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        title.rectTransform.sizeDelta = new Vector2(windowSize.x - 60f, 44f);
        title.rectTransform.anchoredPosition = new Vector2(0f, halfHeight - 42f);

        Image line = CreateImage("TitleLine", content, new Color(accentColor.r, accentColor.g, accentColor.b, 0.5f));
        line.rectTransform.anchorMin = line.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        line.rectTransform.sizeDelta = new Vector2(windowSize.x - 60f, 2f);
        line.rectTransform.anchoredPosition = new Vector2(0f, halfHeight - 70f);
        line.raycastTarget = false;

        // --- 各行 ---
        float rowTop = halfHeight - 118f;
        const float rowStep = 52f;

        masterSlider = CreateSliderRow(content, masterLabel, rowTop, out masterValueText);
        bgmSlider = CreateSliderRow(content, bgmLabel, rowTop - rowStep, out bgmValueText);
        seSlider = CreateSliderRow(content, seLabel, rowTop - rowStep * 2f, out seValueText);
        invertYButton = CreateToggleRow(content, invertYLabel, rowTop - rowStep * 3f, out invertYStateText);
        invertXButton = CreateToggleRow(content, invertXLabel, rowTop - rowStep * 4f, out invertXStateText);

        // --- 下部のボタン ---
        float bottomY = -halfHeight + 46f;
        resetButton = CreateButton("ResetButton", content, resetLabel, new Vector2(150f, 34f),
                                   new Vector2(-90f, bottomY), out _);
        closeButton = CreateButton("CloseButton", content, closeLabel, new Vector2(150f, 34f),
                                   new Vector2(90f, bottomY), out _);

        // --- キーボード / コントローラーでの移動順 ---
        SetNavigation(masterSlider, closeButton, bgmSlider, null, null);
        SetNavigation(bgmSlider, masterSlider, seSlider, null, null);
        SetNavigation(seSlider, bgmSlider, invertYButton, null, null);
        SetNavigation(invertYButton, seSlider, invertXButton, null, null);
        SetNavigation(invertXButton, invertYButton, resetButton, null, null);
        SetNavigation(resetButton, invertXButton, masterSlider, closeButton, closeButton);
        SetNavigation(closeButton, invertXButton, masterSlider, resetButton, resetButton);

        firstSelectable = masterSlider;
    }

    // ==================================================================
    // メニューへのボタン追加
    // ==================================================================

    private void InsertMenuButton()
    {
        if (templateButton == null)
        {
            Debug.LogWarning($"[{nameof(SettingsMenu)}] 複製元のボタンが見つかりませんでした（探した名前: {templateButtonName}）。" +
                             "Inspector の Template Button に手動で割り当ててください。");
            return;
        }

        Transform parent = templateButton.transform.parent;
        GameObject clone = Instantiate(templateButton.gameObject, parent);
        clone.name = "settingsButton";
        RegisterCreated(clone);

        // 文字を差し替え（光らせる用の重ねテキストがある場合も考えて全部書き換える）
        TextMeshProUGUI[] cloneTexts = clone.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < cloneTexts.Length; ++i)
        {
            cloneTexts[i].text = menuButtonLabel;
        }

        menuButton = clone.GetComponent<Button>();

        // 複製元のクリック処理（Quitなど）が残っているので消しておく
        if (menuButton != null)
        {
            ClearClickListeners(menuButton);
        }

        // 描画順（ヒエラルキーの並び）も複製元の隣に置く
        int templateIndex = templateButton.transform.GetSiblingIndex();
        clone.transform.SetSiblingIndex(insertAboveTemplate ? templateIndex : templateIndex + 1);

        if (relayoutMenu)
        {
            RelayoutMenu(parent, clone.transform as RectTransform);
        }
        else
        {
            RectTransform cloneRect = clone.transform as RectTransform;
            RectTransform templateRect = templateButton.transform as RectTransform;
            if (cloneRect != null && templateRect != null)
            {
                cloneRect.anchoredPosition = templateRect.anchoredPosition + new Vector2(0f, insertAboveTemplate ? 50f : -50f);
            }
        }
    }

    /// <summary>
    /// 同じ親にいるボタンを縦に並べ直す。
    /// 元の並びの中心と間隔を保ったまま、ボタンが1つ増えた分だけ上下に広がる。
    /// 動かす前の位置は覚えておき、削除ツールで元に戻せるようにする。
    /// </summary>
    private void RelayoutMenu(Transform parent, RectTransform clone)
    {
        if (parent == null || clone == null) return;

        // 複製したボタン以外の「元からあるボタン」を集める
        List<RectTransform> buttons = new List<RectTransform>();
        foreach (Transform child in parent)
        {
            if (child == clone) continue;
            if (!child.gameObject.activeSelf) continue;
            if (child.GetComponent<Button>() == null) continue;

            RectTransform rect = child as RectTransform;
            if (rect != null)
            {
                buttons.Add(rect);
            }
        }

        if (buttons.Count == 0) return;

        // 上から下の順に並べる
        buttons.Sort((a, b) => b.anchoredPosition.y.CompareTo(a.anchoredPosition.y));

        // 元の位置を覚えておく（元に戻す用）
        movedMenuButtons.Clear();
        movedMenuButtonOriginalPositions.Clear();
        for (int i = 0; i < buttons.Count; ++i)
        {
            movedMenuButtons.Add(buttons[i]);
            movedMenuButtonOriginalPositions.Add(buttons[i].anchoredPosition);
        }

        // ボタン同士の間隔（平均）を求める
        float spacing = 50f;
        if (buttons.Count >= 2)
        {
            float total = 0f;
            for (int i = 0; i < buttons.Count - 1; ++i)
            {
                total += buttons[i].anchoredPosition.y - buttons[i + 1].anchoredPosition.y;
            }
            spacing = total / (buttons.Count - 1);
        }
        if (Mathf.Abs(spacing) < 1f)
        {
            spacing = 50f;
        }

        // 元の並びの中心（ここを基準に上下へ広げる）
        float center = (buttons[0].anchoredPosition.y + buttons[buttons.Count - 1].anchoredPosition.y) * 0.5f;

        // 複製元の位置に合わせて、複製ボタンを並びに差し込む
        RectTransform templateRect = templateButton.transform as RectTransform;
        int insertIndex = buttons.IndexOf(templateRect);
        if (insertIndex < 0)
        {
            insertIndex = buttons.Count;
        }
        else if (!insertAboveTemplate)
        {
            insertIndex += 1;
        }

        if (templateRect != null)
        {
            clone.anchoredPosition = new Vector2(templateRect.anchoredPosition.x, clone.anchoredPosition.y);
        }
        buttons.Insert(insertIndex, clone);

        // 中心を保ったまま等間隔に並べ直す（横位置は各ボタンのものを維持）
        float startY = center + spacing * (buttons.Count - 1) * 0.5f;
        for (int i = 0; i < buttons.Count; ++i)
        {
            RecordUndo(buttons[i], "Add Settings Button");
            Vector2 pos = buttons[i].anchoredPosition;
            pos.y = startY - spacing * i;
            buttons[i].anchoredPosition = pos;
        }
    }

    // ==================================================================
    // ボタン・スライダーの処理割り当て
    // ==================================================================

    private void WireEvents()
    {
        if (buildingInEditor)
        {
#if UNITY_EDITOR
            // Inspector の OnClick 欄に見える形で登録する（後から手で差し替えられる）
            if (masterSlider != null) UnityEventTools.AddPersistentListener(masterSlider.onValueChanged, new UnityAction<float>(SetMasterVolume));
            if (bgmSlider != null) UnityEventTools.AddPersistentListener(bgmSlider.onValueChanged, new UnityAction<float>(SetBgmVolume));
            if (seSlider != null) UnityEventTools.AddPersistentListener(seSlider.onValueChanged, new UnityAction<float>(SetSeVolume));
            if (invertYButton != null) UnityEventTools.AddPersistentListener(invertYButton.onClick, new UnityAction(ToggleInvertY));
            if (invertXButton != null) UnityEventTools.AddPersistentListener(invertXButton.onClick, new UnityAction(ToggleInvertX));
            if (resetButton != null) UnityEventTools.AddPersistentListener(resetButton.onClick, new UnityAction(ResetSettings));
            if (closeButton != null) UnityEventTools.AddPersistentListener(closeButton.onClick, new UnityAction(Close));
            if (menuButton != null) UnityEventTools.AddPersistentListener(menuButton.onClick, new UnityAction(Open));
#endif
            return;
        }

        // 実行時に組み立てた場合はコードから登録する
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        if (seSlider != null) seSlider.onValueChanged.AddListener(SetSeVolume);
        if (invertYButton != null) invertYButton.onClick.AddListener(ToggleInvertY);
        if (invertXButton != null) invertXButton.onClick.AddListener(ToggleInvertX);
        if (resetButton != null) resetButton.onClick.AddListener(ResetSettings);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (menuButton != null) menuButton.onClick.AddListener(Open);
    }

    private void ClearClickListeners(Button button)
    {
        if (buildingInEditor)
        {
#if UNITY_EDITOR
            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; --i)
            {
                UnityEventTools.RemovePersistentListener(button.onClick, i);
            }
#endif
            return;
        }

        button.onClick = new Button.ButtonClickedEvent();
    }

    // ==================================================================
    // UI生成のための小さな道具
    // ==================================================================

    /// <summary>エディタで生成した場合、Undo（Ctrl+Z）で消せるようにする</summary>
    private void RegisterCreated(GameObject obj)
    {
#if UNITY_EDITOR
        if (buildingInEditor && obj != null)
        {
            Undo.RegisterCreatedObjectUndo(obj, "Create Settings UI");
        }
#endif
    }

    /// <summary>エディタで値を変える前に Undo に記録する</summary>
    private void RecordUndo(Object target, string label)
    {
#if UNITY_EDITOR
        if (buildingInEditor && target != null)
        {
            Undo.RecordObject(target, label);
        }
#endif
    }

    private void DestroyUIObject(GameObject obj)
    {
        if (obj == null) return;

#if UNITY_EDITOR
        if (buildingInEditor)
        {
            Undo.DestroyObjectImmediate(obj);
            return;
        }
#endif
        Destroy(obj);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private RectTransform CreateRect(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        obj.layer = parent.gameObject.layer;
        return rect;
    }

    private Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        Image image = obj.GetComponent<Image>();
        image.rectTransform.SetParent(parent, false);
        image.color = color;
        obj.layer = parent.gameObject.layer;
        return image;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string content, float fontSize,
                                       TextAlignmentOptions alignment, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        text.rectTransform.SetParent(parent, false);
        obj.layer = parent.gameObject.layer;

        if (uiFont != null)
        {
            text.font = uiFont;
        }
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    /// <summary>「ラベル ＋ スライダー ＋ 数値」の1行を作る</summary>
    private Slider CreateSliderRow(Transform parent, string label, float y, out TextMeshProUGUI valueText)
    {
        RectTransform row = CreateRowRect(parent, label + "Row", y);

        TextMeshProUGUI labelText = CreateText("Label", row, label, 22f, TextAlignmentOptions.Left, textColor);
        SetAnchors(labelText.rectTransform, new Vector2(0f, 0f), new Vector2(0.36f, 1f));

        // スライダー本体
        GameObject sliderObj = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.SetParent(row, false);
        sliderObj.layer = row.gameObject.layer;
        sliderRect.anchorMin = new Vector2(0.38f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.83f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.offsetMin = new Vector2(0f, -6f);
        sliderRect.offsetMax = new Vector2(0f, 6f);

        Image background = CreateImage("Background", sliderRect, new Color(1f, 1f, 1f, 0.18f));
        Stretch(background.rectTransform);

        RectTransform fillArea = CreateRect("Fill Area", sliderRect);
        Stretch(fillArea);
        Image fill = CreateImage("Fill", fillArea, accentColor);
        Stretch(fill.rectTransform);

        RectTransform handleArea = CreateRect("Handle Slide Area", sliderRect);
        Stretch(handleArea);
        handleArea.offsetMin = new Vector2(7f, 0f);
        handleArea.offsetMax = new Vector2(-7f, 0f);
        Image handle = CreateImage("Handle", handleArea, Color.white);
        handle.rectTransform.sizeDelta = new Vector2(14f, 8f);

        Slider slider = sliderObj.GetComponent<Slider>();
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.transition = Selectable.Transition.ColorTint;
        slider.colors = MakeSliderColorBlock();

        // 右端の数値表示
        valueText = CreateText("Value", row, "100%", 20f, TextAlignmentOptions.Right, textColor);
        SetAnchors(valueText.rectTransform, new Vector2(0.85f, 0f), new Vector2(1f, 1f));

        return slider;
    }

    /// <summary>「ラベル ＋ ON/OFF ボタン」の1行を作る</summary>
    private Button CreateToggleRow(Transform parent, string label, float y, out TextMeshProUGUI stateText)
    {
        RectTransform row = CreateRowRect(parent, label + "Row", y);

        TextMeshProUGUI labelText = CreateText("Label", row, label, 22f, TextAlignmentOptions.Left, textColor);
        SetAnchors(labelText.rectTransform, new Vector2(0f, 0f), new Vector2(0.6f, 1f));

        Button button = CreateButton("Toggle", row, offLabel, new Vector2(110f, 30f), Vector2.zero, out stateText);

        RectTransform buttonRect = button.transform as RectTransform;
        if (buttonRect != null)
        {
            buttonRect.anchorMin = buttonRect.anchorMax = new Vector2(1f, 0.5f);
            buttonRect.pivot = new Vector2(1f, 0.5f);
            buttonRect.anchoredPosition = Vector2.zero;
        }

        return button;
    }

    private RectTransform CreateRowRect(Transform parent, string name, float y)
    {
        RectTransform row = CreateRect(name, parent);
        row.anchorMin = row.anchorMax = new Vector2(0.5f, 0.5f);
        row.pivot = new Vector2(0.5f, 0.5f);
        row.sizeDelta = new Vector2(windowSize.x - 70f, 36f);
        row.anchoredPosition = new Vector2(0f, y);
        return row;
    }

    private static void SetAnchors(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private Button CreateButton(string name, Transform parent, string label, Vector2 size, Vector2 anchoredPosition,
                                out TextMeshProUGUI labelText)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        obj.layer = parent.gameObject.layer;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        Image image = obj.GetComponent<Image>();
        image.color = Color.white;

        Button button = obj.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        button.colors = MakeColorBlock();

        labelText = CreateText("Label", rect, label, 20f, TextAlignmentOptions.Center, textColor);
        Stretch(labelText.rectTransform);

        return button;
    }

    /// <summary>スライダーのつまみ用（普段は白、選択中は強調色）</summary>
    private ColorBlock MakeSliderColorBlock()
    {
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = Color.white;
        colors.highlightedColor = accentColor;
        colors.selectedColor = accentColor;
        colors.pressedColor = accentColor;
        colors.disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);
        colors.fadeDuration = 0.1f;
        return colors;
    }

    private ColorBlock MakeColorBlock()
    {
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = new Color(1f, 1f, 1f, 0.22f);
        colors.highlightedColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.55f);
        colors.selectedColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.55f);
        colors.pressedColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.85f);
        colors.disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.3f);
        colors.fadeDuration = 0.1f;
        return colors;
    }

    private static void SetNavigation(Selectable target, Selectable up, Selectable down, Selectable left, Selectable right)
    {
        if (target == null) return;

        Navigation navigation = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnUp = up,
            selectOnDown = down,
            selectOnLeft = left,
            selectOnRight = right,
        };
        target.navigation = navigation;
    }
}
