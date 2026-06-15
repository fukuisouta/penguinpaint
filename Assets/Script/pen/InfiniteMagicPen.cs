using UnityEngine;

/// <summary>
/// 無制限にマジックペンで線を描き始めるための起点となるクラス
/// </summary>
public class InfiniteMagicPen : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("線のプレハブ")]
    [Tooltip("LineRendererとEdgeCollider2Dがあらかじめアタッチされた線のベースオブジェクト")]
    public GameObject linePrefab;

    [Header("設定")]
    [Tooltip("描いた線（または頂点）が消えるまでの秒数")]
    public float lineLifetime = 2.0f;

    private void Update()
    {
        // カウントダウン中など、ゲームがプレイ中（m_play = true）でなければ入力を受け付けない
        if (gameManager != null && !gameManager.m_play) return;

        // 左クリック（または画面タップ）した瞬間に、新しいストローク（線オブジェクト）を1個生成
        if (Input.GetMouseButtonDown(0))
        {
            StartNewLine();
        }
    }

    /// <summary>
    /// 線プレハブのインスタンス化と初期設定を行う
    /// </summary>
    void StartNewLine()
    {
        // 原点（Vector3.zero）で生成し、実際の線の位置はVanishingLine側で制御させる
        GameObject newLineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

        // 動的にVanishingLineコンポーネントをアタッチし、消える時間を初期化
        VanishingLine vanishingScript = newLineObj.AddComponent<VanishingLine>();
        vanishingScript.Initialize(lineLifetime);
    }
}