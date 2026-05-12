using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // スタートボタンを押したときに呼ばれる
    public void OnStartButton()
    {
        // ゲームシーンに遷移（シーン名は実際のファイル名に合わせる）
        SceneManager.LoadScene("Game");
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
