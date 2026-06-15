using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 描画された個々の線オブジェクトを制御し、古い頂点から時間経過で消滅させるクラス
/// </summary>
public class VanishingLine : MonoBehaviour
{
    /// <summary>
    /// マウスが通過した各頂点の座標と生成時間を記録する構造体
    /// </summary>
    private struct PointInfo
    {
        public Vector2 position;  // 頂点の2D座標
        public float spawnTime;   // この頂点が生成されたゲーム時間（Time.time）
    }

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private float lifeDuration;          // 線の寿命（秒）
    private bool isInputActive = true;   // 現在進行形でマウスが押され、線が伸びている最中か

    // --- メモリ負荷（GC Alloc）を削減するためのデータ構造 ---
    // 先に入れた古い点から順に消すため、FIFO（先入先出）の Queue が最適
    private Queue<PointInfo> pointsQueue = new Queue<PointInfo>();

    // 毎フレーム new List() をすると重くなるため、あらかじめ枠（バッファ）を用意して使い回す
    private List<Vector3> drawBuffer = new List<Vector3>();
    private List<Vector2> collisionBuffer = new List<Vector2>();

    /// <summary>
    /// 生成直後に呼び出され、コンポーネントの取得と寿命の設定を行う
    /// </summary>
    public void Initialize(float lifetime)
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        lifeDuration = lifetime;
    }

    private void Update()
    {
        // マウスが押されている間は、現在のマウス位置を線に追記し続ける
        if (isInputActive)
        {
            HandleInput();
        }

        // 寿命を迎えた古い点をQueueから取り除く
        RemoveOldPoints();

        // 残った点をもとにグラフィックと当たり判定を再構築
        RefreshGraphics();

        // マウスが離され（これ以上伸びない）、かつ全ての点が寿命で消えたらオブジェクト自体を破棄
        if (!isInputActive && pointsQueue.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// マウスの動きを監視し、新しい頂点座標を登録する
    /// </summary>
    private void HandleInput()
    {
        // マウスを離したら「このストロークの入力は終了」とみなす
        if (Input.GetMouseButtonUp(0))
        {
            isInputActive = false;
            return;
        }

        // 現在のマウスの位置をワールド座標に変換
        Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 最初の1点目であるか、または直前の点からある程度（0.1ユニット以上）離れていたら新しい点を追加
        // （これにより、同じ場所でマウスを止めていても無駄な頂点が量産されるのを防ぐ）
        if (pointsQueue.Count == 0 || Vector2.Distance(currentMousePos, drawBuffer[drawBuffer.Count - 1]) > 0.1f)
        {
            pointsQueue.Enqueue(new PointInfo
            {
                position = currentMousePos,
                spawnTime = Time.time // 現在の時間を刻印
            });
        }
    }

    /// <summary>
    /// 寿命（lifeDuration）を超えた古い頂点を先頭から順番に削除する
    /// </summary>
    private void RemoveOldPoints()
    {
        // キューの先頭（一番古い点）を見て、経過時間が寿命を超えている間、ループで取り除き続ける
        while (pointsQueue.Count > 0 && Time.time - pointsQueue.Peek().spawnTime > lifeDuration)
        {
            pointsQueue.Dequeue();
        }
    }

    /// <summary>
    /// 残っている頂点データから、LineRendererとEdgeCollider2Dの形状を更新する
    /// </summary>
    private void RefreshGraphics()
    {
        int count = pointsQueue.Count;

        // 点が2つ未満（1点のみ、または0点）では線として成り立たないため、描画とコライダーを消して終了
        if (count < 2)
        {
            lineRenderer.positionCount = 0;
            if (edgeCollider != null) edgeCollider.enabled = false;
            return;
        }

        // 前フレームのデータをバッファからクリアし、現在のQueueのデータを詰め替える
        drawBuffer.Clear();
        collisionBuffer.Clear();

        foreach (var p in pointsQueue)
        {
            drawBuffer.Add(new Vector3(p.position.x, p.position.y, 0)); // LineRenderer用（Vector3）
            collisionBuffer.Add(p.position);                             // EdgeCollider用（Vector2）
        }

        // LineRendererに座標配列を渡して描画を更新
        lineRenderer.positionCount = drawBuffer.Count;
        lineRenderer.SetPositions(drawBuffer.ToArray());

        // EdgeCollider2Dの形を描画と完全に一致させる
        if (edgeCollider != null)
        {
            edgeCollider.enabled = true;
            edgeCollider.points = collisionBuffer.ToArray();
        }
    }
}