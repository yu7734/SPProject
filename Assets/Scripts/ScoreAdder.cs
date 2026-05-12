using UnityEngine;

/// <summary>
/// このオブジェクト（敵など）が破壊されたときに、ScoreManagerにスコアを加算するスクリプト。
/// 敵Prefabにアタッチして、Inspectorで加算ポイントを設定するだけで使える。
/// 既存のEnemyManagerなどには手を加えない。
/// </summary>
public class ScoreAdder : MonoBehaviour
{
    [SerializeField, Header("撃破時に加算されるスコア")] private int scorePoint = 100;
    [SerializeField, Header("シーン遷移時の自動消滅では加算しない")] private bool addOnlyWhenInGame = true;

    private bool isQuitting = false;

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        // アプリ終了時やシーン遷移時の自動破棄では加算しない
        if (isQuitting) return;
        if (addOnlyWhenInGame && !gameObject.scene.isLoaded) return;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scorePoint);
        }
    }
}
