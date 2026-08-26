#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// 設定パネル（SettingsMenu）をシーン上に実物として作るエディタ用の処理。
///
/// 以前は Tools メニューに項目を出していたが、
/// 作成は最初の1回しか使わないのでメニューからは外した。
/// 作成・作り直し・削除は SettingsMenu の Inspector に出るボタンから行う。
///
/// 生成されるもの
///   ・SettingsMenu の中に SettingsCanvas > Panel > Window …（パネルの中身一式）
///   ・タイトルメニューに settingsButton（quitButton の複製）
/// どちらも普通のオブジェクトなので、手で位置や色を編集できる。
/// 実行後はシーンの保存（Ctrl+S）を忘れずに。
/// </summary>
public static class SettingsMenuTool
{
    /// <summary>UIを生成してシーンを変更済みにする</summary>
    public static void Build(SettingsMenu menu)
    {
        if (menu == null) return;

        Undo.RecordObject(menu, "Create Settings UI");
        menu.BuildUI(true);

        EditorUtility.SetDirty(menu);
        EditorSceneManager.MarkSceneDirty(menu.gameObject.scene);

        Selection.activeGameObject = menu.gameObject;
        EditorGUIUtility.PingObject(menu.gameObject);

        Debug.Log("[SettingsMenu] 設定パネルを作成しました。ヒエラルキーで編集できます。シーンの保存（Ctrl+S）を忘れずに。", menu.gameObject);
    }

    /// <summary>UIを消してメニューの並びを元に戻す</summary>
    public static void Clear(SettingsMenu menu)
    {
        if (menu == null) return;

        Undo.RecordObject(menu, "Remove Settings UI");
        menu.ClearUI(true);

        EditorUtility.SetDirty(menu);
        EditorSceneManager.MarkSceneDirty(menu.gameObject.scene);

        Debug.Log("[SettingsMenu] 設定パネルを削除し、メニューの並びを元に戻しました。", menu.gameObject);
    }
}

/// <summary>
/// SettingsMenu の Inspector に「UIを作成 / 作り直す / 削除」ボタンを足す。
/// 設定パネルの作成まわりの操作は、すべてここから行う。
/// </summary>
[CustomEditor(typeof(SettingsMenu))]
public class SettingsMenuInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SettingsMenu menu = (SettingsMenu)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("パネルの生成", EditorStyles.boldLabel);

        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("再生中は生成・削除できません。", MessageType.Info);
            return;
        }

        if (!menu.HasUI)
        {
            EditorGUILayout.HelpBox("まだUIが作られていません。下のボタンで作成してください。", MessageType.Warning);

            if (GUILayout.Button("設定パネルのUIを作成する"))
            {
                SettingsMenuTool.Build(menu);
            }
            return;
        }

        if (GUILayout.Button("UIを作り直す（手を加えた変更は消えます）"))
        {
            if (EditorUtility.DisplayDialog("作り直し", "今のUIを消して作り直します。よろしいですか？", "作り直す", "やめる"))
            {
                menu.ClearUI(true);
                SettingsMenuTool.Build(menu);
            }
        }

        if (GUILayout.Button("UIを削除する（メニューの並びも元に戻す）"))
        {
            SettingsMenuTool.Clear(menu);
        }
    }
}
#endif
