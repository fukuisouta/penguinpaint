using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// リザルト（結果発表）画面の表示およびボタン操作を管理するクラス
/// </summary>
public class ResultManager : MonoBehaviour
{
    [Header("UI設定")]
    public TextMeshProUGUI lastScoreText;   // 今回のスコアテキスト
    public TextMeshProUGUI highScoreText;   // ハイスコア（BEST）テキスト
    public GameObject newRecordObject;      // 「New Record」と書かれた画像やUIオブジェクト

    [Header("演出設定")]
    [SerializeField, Tooltip("決定音のプレハブ")] private GameObject m_submitSE;
    [SerializeField, Tooltip("フェードスクリプトの参照")] private Fade m_fade = null;

    void Start()
    {
        // 端末（PlayerPrefs）に保存されているスコアデータを取得
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (lastScoreText != null) lastScoreText.text = lastScore.ToString() + "m";
        if (highScoreText != null) highScoreText.text = "BEST: " + highScore.ToString() + "m";

        // 今回のスコアがハイスコア以上、かつ0点より大きい場合に「New Record」を表示
        if (newRecordObject != null)
        {
            newRecordObject.SetActive(lastScore >= highScore && lastScore > 0);
        }
    }

    void Update()
    {
        // 利便性のため、画面クリックだけでなくスペースキーでもリトライできるようにする
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnRetryButton();
        }
    }

    // --- ボタン・キー入力に割り当てる関数 ---

    /// <summary>
    /// リトライボタンが押されたとき（エンドレスモードへ遷移）
    /// </summary>
    public void OnRetryButton()
    {
        PlayTransition("2,4ENDLESS");
    }

    /// <summary>
    /// タイトルに戻るボタンが押されたとき
    /// </summary>
    public void OnTitleButton()
    {
        PlayTransition("0,Title");
    }

    // --- フェード演出の共通処理 ---

    /// <summary>
    /// 決定音を鳴らし、フェードアウトしてから指定シーンへ遷移する
    /// </summary>
    private void PlayTransition(string sceneName)
    {
        if (m_submitSE != null) Instantiate(m_submitSE);

        if (m_fade != null)
        {
            m_fade.FadeOut(1.5f, () => {
                StartCoroutine(LoadAnyScene(sceneName));
            });
        }
        else
        {
            // フェードの参照がない場合は即座に移動（デバッグ・エラー回避用）
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// シーンロード後にフェードインさせるコルーチン
    /// </summary>
    private IEnumerator LoadAnyScene(string name)
    {
        SceneManager.LoadScene(name);
        yield return null; // 1フレーム待機してロード完了を待つ
        if (m_fade != null) m_fade.FadeIn(1.5f);
    }
}