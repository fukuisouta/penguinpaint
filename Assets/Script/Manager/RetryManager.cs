using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シンプルに現在のステージを最初からやり直す（リトライ）ためのクラス
/// </summary>
public class RetryManager : MonoBehaviour
{
    /// <summary>
    /// 現在開いているアクティブなシーンを即座に再ロードする
    /// </summary>
    public void RetryGame()
    {
        // 現在アクティブなシーンの情報を取得
        Scene currentScene = SceneManager.GetActiveScene();

        // そのシーン名を使ってロードし直す（フェード等の演出なし）
        SceneManager.LoadScene(currentScene.name);
    }
}