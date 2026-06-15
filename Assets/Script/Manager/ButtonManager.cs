using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// UIボタンの各種イベント（シーン遷移、ゲーム終了など）を管理するクラス
/// </summary>
public class ButtonManager : MonoBehaviour
{
    [SerializeField, Tooltip("ボタン選択時の決定音プレハブ")] private GameObject m_submitSE;
    [SerializeField, Tooltip("フェード演出スクリプトの参照")] private Fade m_fade = null;

    // プレイヤーが最後にプレイ（死亡）したステージ名を記録する静的変数（初期値は "GamePlay"）
    private static string lastPlayedScene = "GamePlay";

    /// <summary>
    /// 現在のシーン名をリトライ先として保存する（Playerスクリプトなどから死亡時に呼び出す想定）
    /// </summary>
    public static void SaveCurrentScene()
    {
        lastPlayedScene = SceneManager.GetActiveScene().name;
        Debug.Log("リトライ先を保存しました: " + lastPlayedScene);
    }

    /// <summary>
    /// アプリケーションを終了する
    /// </summary>
    public void end_game()
    {
        Debug.Log("ゲーム終了");

        // タイムスケールを等倍に戻してからクイック（ポーズ中などの終了対策）
        Time.timeScale = 1f;
        Application.Quit();
    }

    /// <summary>
    /// 保存された「最後にプレイしたシーン」へフェード付きでリトライする
    /// </summary>
    public void retry_game()
    {
        if (m_submitSE != null) Instantiate(m_submitSE);

        m_fade.FadeOut(1.5f, () => {
            StartCoroutine(LoadAnyScene(lastPlayedScene));
        });
    }

    /// <summary>
    /// タイトル画面へフェード付きで遷移する
    /// </summary>
    public void change_scene_title()
    {
        if (m_submitSE != null) Instantiate(m_submitSE);
        m_fade.FadeOut(1.5f, () => {
            StartCoroutine(LoadAnyScene("0,Title"));
        });
    }

    /// <summary>
    /// マップ選択（セレクト）画面へフェード付きで遷移する
    /// </summary>
    public void change_scene_select()
    {
        if (m_submitSE != null) Instantiate(m_submitSE);
        m_fade.FadeOut(1.5f, () => {
            StartCoroutine(LoadAnyScene("1,Select_Map"));
        });
    }

    /// <summary>
    /// 指定されたシーンをロードし、完了後にフェードインさせる共通コルーチン
    /// </summary>
    private IEnumerator LoadAnyScene(string name)
    {
        SceneManager.LoadScene(name);
        yield return null; // シーンロード完了まで1フレーム待機
        if (m_fade != null) m_fade.FadeIn(1.5f);
    }
}