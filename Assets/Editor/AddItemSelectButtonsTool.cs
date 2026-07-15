#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテム選択パネルに Gun / Syabon ボタンを自動追加するエディタツール。
/// メニュー: Tools > SpacePhantom > Gun・Syabonボタンを選択パネルに追加
/// Gameシーンを開いた状態で1回実行するだけでOK（2回実行しても重複しない）。
/// </summary>
public static class AddItemSelectButtonsTool
{
    [MenuItem("Tools/SpacePhantom/Gun・Syabonボタンを選択パネルに追加")]
    public static void AddButtons()
    {
        // --- UIManagerを取得 ---
        var ui = Object.FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
        if (ui == null)
        {
            EditorUtility.DisplayDialog("エラー", "シーン内にUIManagerが見つかりません。Gameシーンを開いてから実行してください。", "OK");
            return;
        }

        // --- 既存の選択ボタン（HPUp / LaserOrPower / AddFanel）を探す ---
        var allButtons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Button templateButton = null; // AddFanelボタンを複製元にする
        var rowButtons = new List<Button>();
        foreach (var b in allButtons)
        {
            for (int i = 0; i < b.onClick.GetPersistentEventCount(); i++)
            {
                string m = b.onClick.GetPersistentMethodName(i);
                if (m == "AddGun" || m == "AddSyabon")
                {
                    EditorUtility.DisplayDialog("スキップ", "Gun/Syabonボタンは既に追加されています。", "OK");
                    return;
                }
                if (m == "AddFanel") templateButton = b;
                if (m == "AddFanel" || m == "HPUp" || m == "LaserOrPower") rowButtons.Add(b);
            }
        }
        if (templateButton == null)
        {
            EditorUtility.DisplayDialog("エラー", "複製元（AddFanelボタン）が見つかりません。", "OK");
            return;
        }

        // --- 既存ボタンの並びから新しい行の位置を計算 ---
        var templateRt = templateButton.GetComponent<RectTransform>();
        float height = templateRt.rect.height;
        var xs = rowButtons.Select(b => b.GetComponent<RectTransform>().anchoredPosition.x).OrderBy(x => x).ToArray();
        float minY = rowButtons.Min(b => b.GetComponent<RectTransform>().anchoredPosition.y);
        float spacing = (xs.Length >= 2) ? (xs.Last() - xs.First()) / (xs.Length - 1) : templateRt.rect.width * 1.2f;
        if (Mathf.Approximately(spacing, 0f)) spacing = templateRt.rect.width * 1.2f;
        float newRowY = minY - height * 1.2f;

        // --- マネージャー参照を取得 ---
        var gunManager = Object.FindAnyObjectByType<GunManagerScript>(FindObjectsInactive.Include);
        var syabonManager = Object.FindAnyObjectByType<SyabonManagerScript>(FindObjectsInactive.Include);

        // --- ボタンを2つ複製 ---
        CreateButton(templateButton, "GunButton", "Gun", new Vector2(-spacing * 0.5f, newRowY), ui.AddGun);
        CreateButton(templateButton, "SyabonButton", "Syabon", new Vector2(spacing * 0.5f, newRowY), ui.AddSyabon);

        // --- UIManagerのSerializeFieldへ自動アサイン ---
        var so = new SerializedObject(ui);
        so.FindProperty("gunManager").objectReferenceValue = gunManager;
        so.FindProperty("syabonManager").objectReferenceValue = syabonManager;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(ui.gameObject.scene);

        string warn = "";
        if (gunManager == null) warn += "\n※ GunManagerがシーンに見つからなかったため未アサインです。";
        if (syabonManager == null) warn += "\n※ SyabonManagerがシーンに見つからなかったため未アサインです。";
        EditorUtility.DisplayDialog("完了",
            "GunButton / SyabonButton を追加し、UIManagerへの参照も設定しました。\nシーンを保存してください（Ctrl+S）。" + warn, "OK");
    }

    /// <summary>
    /// アイテム選択パネルに ItemSelectRandomizer を自動アタッチする。
    /// メニュー: Tools > SpacePhantom > ランダムアイテム選択をパネルに追加
    /// </summary>
    [MenuItem("Tools/SpacePhantom/ランダムアイテム選択をパネルに追加")]
    public static void AddRandomizer()
    {
        var ui = Object.FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
        if (ui == null)
        {
            EditorUtility.DisplayDialog("エラー", "シーン内にUIManagerが見つかりません。Gameシーンを開いてから実行してください。", "OK");
            return;
        }

        var so = new SerializedObject(ui);
        var panel = so.FindProperty("selectItemImage").objectReferenceValue as GameObject;
        if (panel == null)
        {
            EditorUtility.DisplayDialog("エラー", "UIManagerのSelect Item Image（選択パネル）が未設定です。", "OK");
            return;
        }

        if (panel.GetComponentInChildren<ItemSelectRandomizer>(true) != null)
        {
            EditorUtility.DisplayDialog("スキップ", "ItemSelectRandomizerは既にパネルに付いています。", "OK");
            return;
        }

        Undo.AddComponent<ItemSelectRandomizer>(panel);
        EditorSceneManager.MarkSceneDirty(panel.scene);
        EditorUtility.DisplayDialog("完了",
            $"{panel.name} に ItemSelectRandomizer を追加しました。\n" +
            "候補はGun(緑)/Fanel(赤)/Laser(青)/Syabon(紫)の4種で、パネルが開くたびに3つ抽選されます。\n" +
            "シーンを保存してください（Ctrl+S）。", "OK");
    }

    /// <summary>
    /// 選択パネルの各ボタンをカード型レイアウト（名前/Lv/アイコン/説明文）に変換する。
    /// メニュー: Tools > SpacePhantom > 選択ボタンをカード型レイアウトに変換
    /// 既存のTMPを「アイテム名」として上段に移動し、Lv・アイコン・説明文の子要素を追加する。
    /// </summary>
    [MenuItem("Tools/SpacePhantom/選択ボタンをカード型レイアウトに変換")]
    public static void ConvertToCardLayout()
    {
        var ui = Object.FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
        if (ui == null)
        {
            EditorUtility.DisplayDialog("エラー", "シーン内にUIManagerが見つかりません。Gameシーンを開いてから実行してください。", "OK");
            return;
        }
        var so = new SerializedObject(ui);
        var panel = so.FindProperty("selectItemImage").objectReferenceValue as GameObject;
        if (panel == null)
        {
            EditorUtility.DisplayDialog("エラー", "UIManagerのSelect Item Image（選択パネル）が未設定です。", "OK");
            return;
        }

        int converted = 0;
        foreach (var btn in panel.GetComponentsInChildren<Button>(true))
        {
            if (btn.GetComponentInChildren<ItemButtonView>(true) != null) continue; // 変換済み

            // 既存のTMPを「アイテム名」として上段に配置し直す
            var nameTmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (nameTmp == null) continue;
            Undo.RecordObject(nameTmp.rectTransform, "Card Layout");
            Undo.RecordObject(nameTmp, "Card Layout");
            SetAnchors(nameTmp.rectTransform, new Vector2(0.08f, 0.70f), new Vector2(0.92f, 0.90f));
            nameTmp.alignment = TextAlignmentOptions.Center;
            nameTmp.enableAutoSizing = true;
            nameTmp.fontSizeMin = 10f;
            nameTmp.raycastTarget = false;

            // Lv・アイコン・説明文の子要素を追加
            var levelTmp = CreateTmpChild(btn.transform, "LevelText", nameTmp, new Vector2(0.08f, 0.60f), new Vector2(0.92f, 0.70f));
            levelTmp.text = "Lv 0/4";

            var iconGo = new GameObject("Icon", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(iconGo, "Card Layout");
            iconGo.transform.SetParent(btn.transform, false);
            SetAnchors((RectTransform)iconGo.transform, new Vector2(0.28f, 0.30f), new Vector2(0.72f, 0.58f));
            var iconImage = iconGo.AddComponent<Image>();
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            iconImage.enabled = false; // スプライト未設定の間は非表示

            var descTmp = CreateTmpChild(btn.transform, "DescText", nameTmp, new Vector2(0.08f, 0.06f), new Vector2(0.92f, 0.26f));
            descTmp.text = "〜説明〜";

            // ItemButtonViewを付けて参照を接続
            var view = Undo.AddComponent<ItemButtonView>(btn.gameObject);
            view.nameLabel = nameTmp;
            view.levelLabel = levelTmp;
            view.iconImage = iconImage;
            view.descLabel = descTmp;
            ApplyCardAnchors(view);
            EditorUtility.SetDirty(view);
            converted++;
        }

        EditorSceneManager.MarkSceneDirty(panel.scene);
        EditorUtility.DisplayDialog("完了",
            converted > 0
                ? $"{converted}個のボタンをカード型に変換しました。\nアイコン画像はRandomizerのItem Poolの各Icon欄に割り当ててください。\nシーンを保存してください（Ctrl+S）。"
                : "変換対象がありません（すべて変換済みです）。", "OK");
    }

    /// <summary>
    /// Projectウィンドウで選択中のTMPフォントアセット(SDF)を、
    /// アイテム選択カードの全テキスト（名前/Lv/説明文）に一括適用する。
    /// メニュー: Tools > SpacePhantom > 選択中のフォントをアイテムカードに適用
    /// </summary>
    [MenuItem("Tools/SpacePhantom/選択中のフォントをアイテムカードに適用")]
    public static void ApplyFontToItemCards()
    {
        var font = Selection.activeObject as TMP_FontAsset;
        if (font == null)
        {
            EditorUtility.DisplayDialog("エラー",
                "先にProjectウィンドウでフォントアセット（〜 SDF）を選択してから実行してください。\n（TTFファイルではなく、右クリック→Create > TextMeshPro > Font Assetで作った方です）", "OK");
            return;
        }

        var ui = Object.FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
        var so = (ui != null) ? new SerializedObject(ui) : null;
        var panel = so?.FindProperty("selectItemImage").objectReferenceValue as GameObject;
        if (panel == null)
        {
            EditorUtility.DisplayDialog("エラー", "選択パネルが見つかりません。Gameシーンを開いてから実行してください。", "OK");
            return;
        }

        int count = 0;
        foreach (var view in panel.GetComponentsInChildren<ItemButtonView>(true))
        {
            foreach (var tmp in new[] { view.nameLabel, view.levelLabel, view.descLabel })
            {
                if (tmp == null) continue;
                Undo.RecordObject(tmp, "Apply Card Font");
                tmp.font = font;
                EditorUtility.SetDirty(tmp);
                count++;
            }
        }

        EditorSceneManager.MarkSceneDirty(panel.scene);
        EditorUtility.DisplayDialog("完了",
            count > 0
                ? $"{count}個のテキストに「{font.name}」を適用しました。シーンを保存してください（Ctrl+S）。"
                : "カード型のテキストが見つかりません。先に「選択ボタンをカード型レイアウトに変換」を実行してください。", "OK");
    }

    /// <summary>
    /// カードの4要素の配置を整えなおす（文字を上に寄せて、説明文が収まるように）。
    /// メニュー: Tools > SpacePhantom > カードレイアウトを再調整
    /// 変換済みのカードに対して何度でも実行可能。
    /// </summary>
    [MenuItem("Tools/SpacePhantom/カードレイアウトを再調整")]
    public static void RelayoutItemCards()
    {
        var ui = Object.FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
        var so = (ui != null) ? new SerializedObject(ui) : null;
        var panel = so?.FindProperty("selectItemImage").objectReferenceValue as GameObject;
        if (panel == null)
        {
            EditorUtility.DisplayDialog("エラー", "選択パネルが見つかりません。Gameシーンを開いてから実行してください。", "OK");
            return;
        }

        int count = 0;
        foreach (var view in panel.GetComponentsInChildren<ItemButtonView>(true))
        {
            Undo.RecordObject(view.nameLabel != null ? (Object)view.nameLabel.rectTransform : view, "Relayout Card");
            ApplyCardAnchors(view);
            count++;
        }

        EditorSceneManager.MarkSceneDirty(panel.scene);
        EditorUtility.DisplayDialog("完了",
            count > 0 ? $"{count}枚のカードを再配置しました。シーンを保存してください（Ctrl+S）。"
                      : "カードが見つかりません。先に「選択ボタンをカード型レイアウトに変換」を実行してください。", "OK");
    }

    /// <summary>
    /// 各カードボタンにFadeInDropdownを付けて、左から順に時間差で降ってくるように設定する。
    /// メニュー: Tools > SpacePhantom > カードの時間差ドロップをセットアップ
    /// 何度実行してもOK（ディレイ値を設定し直すだけ）。
    /// </summary>
    [MenuItem("Tools/SpacePhantom/カードの時間差ドロップをセットアップ")]
    public static void SetupStaggeredDrop()
    {
        const float delayStep = 0.08f; // カード1枚ごとの出現ディレイ（秒）

        var ui = Object.FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
        var so = (ui != null) ? new SerializedObject(ui) : null;
        var panel = so?.FindProperty("selectItemImage").objectReferenceValue as GameObject;
        if (panel == null)
        {
            EditorUtility.DisplayDialog("エラー", "選択パネルが見つかりません。Gameシーンを開いてから実行してください。", "OK");
            return;
        }

        // ボタンを画面上の並び順（左→右）にしてディレイを振る
        var btns = panel.GetComponentsInChildren<Button>(true)
            .OrderBy(b => b.GetComponent<RectTransform>().position.x)
            .ToArray();

        int count = 0;
        foreach (var btn in btns)
        {
            var drop = btn.GetComponent<FadeInDropdown>();
            if (drop == null)
            {
                drop = Undo.AddComponent<FadeInDropdown>(btn.gameObject); // CanvasGroupも自動で付く
            }
            var dropSo = new SerializedObject(drop);
            dropSo.FindProperty("startDelay").floatValue = delayStep * count;
            dropSo.ApplyModifiedProperties();
            count++;
        }

        EditorSceneManager.MarkSceneDirty(panel.scene);
        EditorUtility.DisplayDialog("完了",
            $"{count}枚のカードに時間差ドロップを設定しました（左から{delayStep}秒間隔）。\nシーンを保存してください（Ctrl+S）。", "OK");
    }

    // カード内の配置定義（ここを変えて「再調整」を実行すれば全カードに反映される）
    private static void ApplyCardAnchors(ItemButtonView view)
    {
        if (view.nameLabel != null)
        {
            SetAnchors(view.nameLabel.rectTransform, new Vector2(0.06f, 0.79f), new Vector2(0.94f, 0.97f));
            view.nameLabel.enableAutoSizing = true;
            view.nameLabel.fontSizeMin = 10f;
            view.nameLabel.fontSizeMax = 64f;
            EditorUtility.SetDirty(view.nameLabel);
        }
        if (view.levelLabel != null)
        {
            SetAnchors(view.levelLabel.rectTransform, new Vector2(0.06f, 0.68f), new Vector2(0.94f, 0.79f));
            view.levelLabel.enableAutoSizing = true;
            view.levelLabel.fontSizeMin = 8f;
            view.levelLabel.fontSizeMax = 46f;
            EditorUtility.SetDirty(view.levelLabel);
        }
        if (view.iconImage != null)
        {
            SetAnchors((RectTransform)view.iconImage.transform, new Vector2(0.20f, 0.30f), new Vector2(0.80f, 0.66f));
            EditorUtility.SetDirty(view.iconImage);
        }
        if (view.descLabel != null)
        {
            // 説明文はアイコンの下に横幅広めのボックスを確保し、自動縮小で必ず収める
            SetAnchors(view.descLabel.rectTransform, new Vector2(0.03f, 0.06f), new Vector2(0.97f, 0.38f));
            view.descLabel.enableAutoSizing = true;
            view.descLabel.fontSizeMin = 8f;
            view.descLabel.fontSizeMax = 20f;
            view.descLabel.textWrappingMode = TextWrappingModes.Normal;
            EditorUtility.SetDirty(view.descLabel);
        }
    }

    private static void SetAnchors(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // フォント設定を既存TMPからコピーした子TMPを作る
    private static TextMeshProUGUI CreateTmpChild(Transform parent, string name, TextMeshProUGUI copyFrom, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, "Card Layout");
        go.transform.SetParent(parent, false);
        SetAnchors((RectTransform)go.transform, anchorMin, anchorMax);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        if (copyFrom != null)
        {
            tmp.font = copyFrom.font;
            tmp.color = copyFrom.color;
        }
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 8f;
        tmp.fontSizeMax = 40f;
        tmp.raycastTarget = false;
        return tmp;
    }

    // テンプレートを複製してラベル・位置・OnClickを設定し、ラベルのTMPを返す
    private static TextMeshProUGUI CreateButton(Button template, string name, string label, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
    {
        var go = Object.Instantiate(template.gameObject, template.transform.parent);
        Undo.RegisterCreatedObjectUndo(go, "Add " + name);
        go.name = name;

        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;

        // OnClickを差し替え（複製されたAddFanelのリスナーを全削除して新しいものを追加）
        var button = go.GetComponent<Button>();
        while (button.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(button.onClick, 0);
        }
        UnityEventTools.AddPersistentListener(button.onClick, onClick);

        // ラベルのTMPテキストを変更
        var tmp = go.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null) tmp.text = label;
        return tmp;
    }
}
#endif
