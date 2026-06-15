using UnityEngine;

/// <summary>
/// アプリケーションの目標フレームレート（FPS）を一元管理・固定するクラス
/// </summary>
public class FPSController : MonoBehaviour
{
    [Header("目標フレームレート (30 や 60 など自由に変更可能)")]
    [SerializeField] private int m_targetFPS = 30;

    void Awake()
    {
        // ゲームが起動した最初の瞬間にターゲットFPSを適用
        SetFPS(m_targetFPS);
    }

    /// <summary>
    /// 目標フレームレートを上書き変更し、Unityシステムに反映させる
    /// </summary>
    /// <param name="fps">固定したいFPS値（例: 30, 60, 120）</param>
    public void SetFPS(int fps)
    {
        m_targetFPS = fps;
        Application.targetFrameRate = m_targetFPS;
        Debug.Log($"ゲームのFPSを 【{m_targetFPS}】 に固定しました。");
    }

    /// <summary>
    /// [Unityエディタ専用] インスペクター上で変数の値が変更されたときに自動実行されるコールバック
    /// </summary>
    void OnValidate()
    {
        // 開発中にゲームプレイ（再生ボタンが押されている状態）であれば、インスペクターのスライダーなどを動かすだけで即座にFPSが切り替わる
        if (Application.isPlaying)
        {
            Application.targetFrameRate = m_targetFPS;
        }
    }
}