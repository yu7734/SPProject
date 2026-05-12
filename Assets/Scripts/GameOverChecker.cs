using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーのHPを外側から監視して、0以下になったらゲームオーバー画面に遷移させるスクリプト。
/// PlayerManagerなど既存のコードには一切手を加えない。
/// </summary>
public class GameOverChecker : MonoBehaviour
{
    [SerializeField, Header("プレイヤー参照")] private PlayerManager player;
    [SerializeField, Header("遷移先シーン名")] private string gameOverSceneName = "GameOver";
    [SerializeField, Header("遷移までの待ち時間（秒）")] private float delay = 1.0f;

    private bool isTriggered = false;

    void Update()
    {
        if (player == null || isTriggered) return;

        // HPが0以下になったら一度だけ遷移処理を発動
        if (player.playerHP <= 0)
        {
            isTriggered = true;
            Invoke(nameof(LoadGameOverScene), delay);
        }
    }

    private void LoadGameOverScene()
    {
        // 念のため時間を通常通りに戻す（アイテム選択中などに死んだ場合のため）
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameOverSceneName);
    }
}
