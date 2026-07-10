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
