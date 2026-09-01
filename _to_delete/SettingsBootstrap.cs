using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 設定パネル（SettingsMenu）をシーンに自動で用意する仕組み。
///
/// タイトルシーンが読み込まれた時に SettingsMenu が1つも無ければ、
/// 空の GameObject を作って自動でアタッチする。
/// つまり Unity 側で何も配置しなくても Settings ボタンが増える。
///
/// 自分で SettingsMenu を置いた場合はそちらが優先され、自動生成はされない
/// （Inspector で色や文言を細かく変えたい時は手で置くこと）。
///
/// 対象シーンを増やしたい場合は AutoSetupScenes に名前を足す。
/// </summary>
public static class SettingsBootstrap
{
    /// <summary>自動で設定パネルを用意するシーン名</summary>
    private static readonly string[] AutoSetupScenes =
    {
        SceneName.Title,
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        // 二重登録を防いでから登録する
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 最初のシーン（エディタでタイトルから再生した場合など）にも適用する
        TrySetup(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySetup(scene);
    }

    private static void TrySetup(Scene scene)
    {
        if (!scene.IsValid()) return;

        bool isTargetScene = false;
        for (int i = 0; i < AutoSetupScenes.Length; ++i)
        {
            if (scene.name == AutoSetupScenes[i])
            {
                isTargetScene = true;
                break;
            }
        }
        if (!isTargetScene) return;

        // 既に置かれていれば何もしない（手動配置を優先）
        if (Object.FindAnyObjectByType<SettingsMenu>() != null) return;

        GameObject obj = new GameObject(nameof(SettingsMenu));
        obj.AddComponent<SettingsMenu>();
    }
}
