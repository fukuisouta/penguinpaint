using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ダメージ発生時の画面フラッシュ（赤色）演出を制御するクラス
/// </summary>
public class DamageEffect : MonoBehaviour
{
    private Image m_image;

    void Awake()
    {
        m_image = GetComponent<Image>();

        // 初期状態は完全に透明（非表示）にしておく
        if (m_image != null) m_image.color = new Color(1, 0, 0, 0);
    }

    /// <summary>
    /// フラッシュ演出を開始する（外部から呼び出される想定）
    /// </summary>
    public void Flash()
    {
        gameObject.SetActive(true);

        // 念のためのインスペクター未設定対策（初期化漏れ防止）
        if (m_image == null) m_image = GetComponent<Image>();

        // 連続でダメージを受けた際、前の演出をキャンセルして最初から再生する
        StopAllCoroutines();
        StartCoroutine(DamageSequence());
    }

    /// <summary>
    /// 赤画面から徐々に透明に戻していくコルーチン
    /// </summary>
    private IEnumerator DamageSequence()
    {
        float duration = 0.5f; // 演出にかける時間（秒）
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 経過時間に応じて不透明度（Alpha値）を 0.5 から 0 へ線形補間
            float a = Mathf.Lerp(0.5f, 0, elapsed / duration);
            if (m_image != null) m_image.color = new Color(1, 0, 0, a);

            yield return null; // 1フレーム待機
        }

        // 演出終了時に確実に完全に透明（Alpha = 0）にする
        if (m_image != null) m_image.color = new Color(1, 0, 0, 0);
    }
}