using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// ステージ選択（セレクト）画面でのボタン入力とシーン遷移を管理するクラス
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    [SerializeField, Tooltip("決定音のプレハブ")] private GameObject m_submitSE;
    [SerializeField, Tooltip("フェードスクリプトの参照")] private Fade m_fade = null;

    /// <summary>
    /// ステージ選択ボタンが押された時に実行する関数
    /// </summary>
    /// <param name="bossID">ボタンから渡されるボスID（0から始まる想定）</param>
    public void OnStageSelectButtonPressed(int bossID)
    {
        // 1. 決定音を生成して鳴らす
        if (m_submitSE != null) Instantiate(m_submitSE);

        // 2. フェードアウトを開始
        if (m_fade != null)
        {
            // フェードアウトが完全に終わった瞬間に、コールバック（ラムダ式）で次のステージをロード
            m_fade.FadeOut(1.5f, () =>
            {
                // ビルド設定のインデックスに合わせるため「bossID + 1」のシーンをロード
                StartCoroutine(LoadSelectedStage(bossID + 1));
            });
        }
        else
        {
            // フェードの参照がない場合は即座に移動（エラー回避用）
            SceneManager.LoadScene(bossID + 1);
        }
    }

    /// <summary>
    /// 指定されたビルドインデックスのシーンをロードし、完了後にフェードインさせるコルーチン
    /// </summary>
    private IEnumerator LoadSelectedStage(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);

        // ロード完了後、次のシーンで正常にFadeオブジェクトが動き出すように1フレーム待機
        yield return null;

        // 次のシーンに引き継がれた（または配置されている）Fadeがあればフェードイン
        if (m_fade != null) m_fade.FadeIn(1.5f);
    }
}