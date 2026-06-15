using UnityEngine;

/// <summary>
/// ゲームオーバー時のUI表示・非表示を管理するクラス
/// </summary>
public class GameOverUIManager : MonoBehaviour
{
    [SerializeField, Tooltip("ゲームオーバー画面となるUIパネル")] private GameObject m_gameOverPanel;

    void Start()
    {
        // ゲーム開始時は誤表示を防ぐために必ず非表示にする
        if (m_gameOverPanel != null)
        {
            m_gameOverPanel.SetActive(false);
        }
    }

    /// <summary>
    /// ゲームオーバーパネルを表示する（Playerスクリプトの死亡処理などから呼び出す）
    /// </summary>
    public void ShowGameOver()
    {
        if (m_gameOverPanel != null)
        {
            m_gameOverPanel.SetActive(true);
        }
    }
}