using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーからゴールまでの進捗状況を計算し、スライダーUI（ミニマップ）に反映するクラス
/// </summary>
public class MiniMap : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Transform m_player; // 追跡対象となるプレイヤーのTransform
    [SerializeField] private Transform m_goal;   // 到達目標となるゴールのTransform
    [SerializeField] private Slider m_mapSlider; // 進捗を表示するためのSliderコンポーネント

    private float m_startPositionX;  // 計測を開始した時点のプレイヤーのX座標
    private float m_goalPositionX;   // ゴールオブジェクトのX座標
    private bool m_isReady = false;  // カウントダウンが終わり、計測を開始してよいかどうかのフラグ

    void Start()
    {
        // 必要な参照がインスペクターで割り当てられていない場合はエラーを防ぐためスクリプトを停止
        if (m_player == null || m_goal == null || m_mapSlider == null)
        {
            Debug.LogWarning("ミニマップの参照が足りません！");
            this.enabled = false;
            return;
        }

        // スライダーの最小値・最大値を 0〜1（割合表示用）に強制固定
        m_mapSlider.minValue = 0f;
        m_mapSlider.maxValue = 1f;
        m_mapSlider.value = 0f;
    }

    /// <summary>
    /// GameManagerの3カウントダウン終了時に外部から呼び出され、初期位置をロックして計測を開始する
    /// </summary>
    public void SetupMiniMap()
    {
        if (m_player == null || m_goal == null) return;

        // 3カウントが終了した「まさにその瞬間」のプレイヤーの座標をスタート地点として記憶
        m_startPositionX = m_player.position.x;
        m_goalPositionX = m_goal.position.x;

        m_isReady = true; // アップデート内での進捗計算を許可
        Debug.Log("ミニマップ：GameManagerからの合図を受け取り、計測を開始しました。");
    }

    void Update()
    {
        // カウントダウン中、または参照が外れている場合は計算を行わない
        if (!m_isReady || m_player == null || m_mapSlider == null) return;

        // 全体の総距離（スタートからゴールまで）を計算
        float totalDistance = m_goalPositionX - m_startPositionX;

        // もし総距離が0以下（ゴールがプレイヤーより左、または同じ位置）なら計算をパス
        if (totalDistance <= 0f) return;

        // プレイヤーが現在スタートからどれだけ進んだかの距離を計算
        float currentProgress = m_player.position.x - m_startPositionX;

        // 進捗割合（0.0 〜 1.0）を算出し、Mathf.Clampで範囲外にはみ出さないようガードしてスライダーに代入
        m_mapSlider.value = Mathf.Clamp01(currentProgress / totalDistance);
    }
}