using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// ゲームの開始カウントダウンやプレイ状態、BGMなどを統括する管理クラス
/// </summary>
public class GameManager : MonoBehaviour
{
    public bool m_play = false;       // ゲームが実際にプレイ可能（カウントダウン終了後）かどうかのフラグ
    private bool m_start = false;      // カウントダウンが開始されているかどうかのフラグ
    private bool m_starttext = false;  // 「GO!!」のテキスト演出中かどうかのフラグ

    [SerializeField] private TextMeshProUGUI TimerText; // カウントダウン表示用のUIテキスト
    float limitTime = 3f;                               // カウントダウンの制限時間（3秒）
    int lastSecond = -1;                                // 前フレームの整数秒（テキスト更新の判定用）

    // カウントダウン数字が縮小する演出用のフォントサイズ
    const float MAX = 250f;
    const float MIN = 160f;

    [SerializeField] private GameObject ui;      // カウントダウンUIの親オブジェクト
    [SerializeField] private GameObject startSE; // シーン開始時の効果音プレハブ
    [SerializeField] private AudioSource BGM;    // ステージのメインBGM

    [Header("ミニマップ設定")]
    [SerializeField] private MiniMap m_miniMap;

    void Start()
    {
        m_play = false;
        m_starttext = false;
        BGM = GetComponent<AudioSource>();

        // シーンが始まった瞬間にカウントダウンを開始する
        m_start = true;

        if (startSE != null)
        {
            Instantiate(startSE);
        }
    }

    void Update()
    {
        if (!m_start) return;

        limitTime -= Time.deltaTime;
        if (limitTime < 0) limitTime = 0;

        // 残り時間を切り上げて整数にする (例: 2.3秒 -> 3)
        int currentSecond = Mathf.CeilToInt(limitTime);

        // 1秒経過するごとにテキストを更新（「GO!!」演出中でない場合）
        if (currentSecond != lastSecond && !m_starttext)
        {
            lastSecond = currentSecond;

            if (currentSecond > 0)
            {
                TimerText.text = currentSecond.ToString();
                TimerText.fontSize = MAX; // 秒数が切り替わった瞬間はサイズを最大にする
            }
            else
            {
                // 0秒になったら「GO!!」演出コルーチンへ
                m_starttext = true;
                StartCoroutine(StartText());
                return;
            }
        }

        // 1秒の間で数字がだんだん小さくなる（縮小）演出の計算
        if (!m_starttext && currentSecond > 0)
        {
            float fraction = limitTime - (currentSecond - 1); // 1.0 ～ 0.0 へ変化する進捗率
            float t = 1f - fraction;                          // 0.0 ～ 1.0 へ反転

            // MAXサイズからMINサイズへ線形補間（滑らかに縮小）
            TimerText.fontSize = Mathf.Lerp(MAX, MIN, t);
        }
    }

    /// <summary>
    /// カウントダウン終了時に「GO!!」を表示し、ゲームプレイを開始させるコルーチン
    /// </summary>
    private IEnumerator StartText()
    {
        TimerText.fontSize = 300;
        TimerText.text = "GO!!";

        yield return new WaitForSeconds(0.1f);

        // 不要になったカウントダウンUIを破棄し、ゲームとBGMを始動
        Destroy(ui);
        m_play = true;
        BGM.Play();

        // ミニマップの位置計測をこの瞬間からスタート
        if (m_miniMap != null)
        {
            m_miniMap.SetupMiniMap();
        }

        // カウントダウン役が終わったため、このスクリプト自身のUpdateを停止して負荷を下げる
        enabled = false;
    }

    /// <summary>
    /// BGMを停止する（プレイヤー死亡時などに外部から呼ばれる想定）
    /// </summary>
    public void StopBGM()
    {
        if (BGM != null && BGM.isPlaying)
        {
            BGM.Stop();
        }
    }
}