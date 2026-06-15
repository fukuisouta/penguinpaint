using UnityEngine;
using System.Collections;

/// <summary>
/// 生成されたLineRendererの色（アルファ値）を徐々に透明にフェードさせ、自動でオブジェクトを破棄するクラス
/// </summary>
public class LineAutoDestruct : MonoBehaviour
{
    public float lifetime = 3.0f;      // 線がそのまま維持される時間（秒）
    public float fadeDuration = 1.0f;  // フェードが始まってから完全に消えるまでの時間（秒）
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        // スコープ（フェードから破棄まで）の一連の流れをコルーチンで実行
        StartCoroutine(FadeAndDestroy());
    }

    /// <summary>
    /// 維持時間を待機した後、アルファ値をグラデーションを介して徐々に減算して消去する
    /// </summary>
    IEnumerator FadeAndDestroy()
    {
        // 1. 設定された lifetime の秒数分だけ、何もしないで待機
        yield return new WaitForSeconds(lifetime);

        float elapsed = 0;
        // 元々のLineRendererが持っている色情報（カラーキー）をキープしておく
        Gradient originalGradient = lr.colorGradient;

        // 2. 指定時間（fadeDuration）かけて透明度を下げるループ処理
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // 時間の経過率に応じて、アルファ値を 1.0(不透明) から 0.0(完全透明) へ補間
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            // 新しいグラデーションデータを作成し、元のRGB色はそのままにAlphaだけ上書き
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                originalGradient.colorKeys, // 色味は元のまま保持
                new GradientAlphaKey[] {
                    new GradientAlphaKey(alpha, 0f), // 線の始点側の不透明度
                    new GradientAlphaKey(alpha, 1f)  // 線の終点側の不透明度
                }
            );
            lr.colorGradient = gradient;

            yield return null; // 1フレーム待機してループを滑らかにする
        }

        // 3. 完全に透明になったので、オブジェクトをメモリから解放
        Destroy(gameObject);
    }
}