using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 画面のフェードイン・フェードアウト演出を管理するクラス
/// </summary>
public class Fade : MonoBehaviour
{
    [SerializeField, Header("フェードパネル")]
    private Image m_image = null;

    private void Start()
    {
        // 開始時は画面を真っ黒（Alpha = 1）にしておき、そこからフェードインさせる
        var c = m_image.color;
        c.a = 1f;
        m_image.color = c;
        m_image.enabled = true;

        FadeIn(2.0f);
    }

    /// <summary>
    /// コンポーネントアタッチ時、またはリセット時に自動でImageを取得する（エディタ補助）
    /// </summary>
    private void Reset()
    {
        m_image = GetComponent<Image>();
    }

    /// <summary>
    /// 画面を明るくする（暗転状態からゲーム画面を表示する）
    /// </summary>
    /// <param name="duration">演出時間</param>
    /// <param name="on_completed">完了時のコールバック処理</param>
    public void FadeIn(float duration, Action on_completed = null)
    {
        // is_reversing = true にすることで、Alpha値を 1 -> 0 へ減少させる
        StartCoroutine(ChangeAlphaValueFrom0To1OverTime(duration, on_completed, true));
    }

    /// <summary>
    /// 画面を暗くする（ゲーム画面から暗転させる）
    /// </summary>
    /// <param name="duration">演出時間</param>
    /// <param name="on_completed">完了時のコールバック処理</param>
    public void FadeOut(float duration, Action on_completed = null)
    {
        // is_reversing = false（デフォルト）で、Alpha値を 0 -> 1 へ増加させる
        StartCoroutine(ChangeAlphaValueFrom0To1OverTime(duration, on_completed));
    }

    /// <summary>
    /// 時間経過でアルファ値を変化させる共通コルーチン
    /// </summary>
    /// <param name="is_reversing">true の場合はフェードイン（1→0）、false の場合はフェードアウト（0→1）</param>
    private IEnumerator ChangeAlphaValueFrom0To1OverTime
    (
        float duration,
        Action on_completed,
        bool is_reversing = false
    )
    {
        m_image.enabled = true;

        var elapsed_time = 0.0f;
        var color = m_image.color;

        while (elapsed_time < duration)
        {
            // 進捗率を 0.0 ～ 1.0 の間にクランプ
            var elapsed_rate = Mathf.Min(elapsed_time / duration, 1.0f);

            // 反転フラグに応じてAlpha値を計算
            color.a = is_reversing ? 1.0f - elapsed_rate : elapsed_rate;
            m_image.color = color;

            elapsed_time += Time.deltaTime;
            yield return null;
        }

        // フェードイン（画面が見える状態）が完了したら、描画負荷を下げるためにImageを無効化
        if (is_reversing) m_image.enabled = false;

        // 完了通知（コールバック）があれば実行
        on_completed?.Invoke();
    }
}