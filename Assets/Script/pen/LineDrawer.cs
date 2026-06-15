using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// マウス入力を検知し、インク残量がある場合に新しい「描画線オブジェクト」をインスタンス化するマネージャークラス
/// </summary>
public class LineDrawer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public GameObject linePrefab;    // 線オブジェクトのプレハブ
    public InkPenScript inkSystem;   // 残量チェック用のインクシステム参照

    [Header("消える設定")]
    public float pointLifetime = 2.0f; // 描かれた各頂点が個別に生まれてから消えるまでの時間

    private void Update()
    {
        if (!gameManager.m_play) return;

        // マウスがクリックされ、かつインクが1以上残っている場合にのみ新しい線を引く
        if (Input.GetMouseButtonDown(0))
        {
            if (inkSystem != null && inkSystem.GetCurrentInk() > 0)
            {
                CreateLineInstance();
            }
        }
    }

    /// <summary>
    /// 新しい線プレハブを生成し、その場でActiveLineコンポーネントを外付けして初期化する
    /// </summary>
    void CreateLineInstance()
    {
        GameObject newLineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

        // 生成したオブジェクトに描画・衝突ロジックである「ActiveLine」をアタッチして設定を注入
        ActiveLine lineScript = newLineObj.AddComponent<ActiveLine>();
        lineScript.Setup(pointLifetime, inkSystem);
    }
}


/// <summary>
/// 実際に生成された「線」そのものが持つ制御ロジック。
/// マウスの軌跡を追って頂点を増やし、それぞれの点に対して個別の消滅タイマーコルーチンを走らせる。
/// </summary>
public class ActiveLine : MonoBehaviour
{
    private LineRenderer lr;
    private EdgeCollider2D col;
    private float lifetime;
    private InkPenScript ink;
    private List<Vector2> points = new List<Vector2>(); // 線の頂点リスト
    private bool isDrawing = false;                     // 現在もマウスで引っ張られて描画中か

    /// <summary>
    /// LineDrawer側から生成時に呼び出され、初期コンポーネントのセットと描画ループの開始を行う
    /// </summary>
    public void Setup(float pointLifetime, InkPenScript inkSystem)
    {
        lr = GetComponent<LineRenderer>();
        col = GetComponent<EdgeCollider2D>();
        lifetime = pointLifetime;
        ink = inkSystem;

        // 生成された瞬間に、自身の描画メインループ（コルーチン）を開始する
        StartCoroutine(DrawRoutine());
    }

    /// <summary>
    /// マウスが押されている間、その位置を追尾してリストへ頂点を追加し続けるメインループ
    /// </summary>
    IEnumerator DrawRoutine()
    {
        isDrawing = true;

        // マウスの左ボタンが押し続けられており、かつインクが残っている間ループ
        while (Input.GetMouseButton(0) && ink != null && ink.GetCurrentInk() > 0)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // 最初の点であるか、または直前の登録点から0.1ユニット以上離れた場合にのみ新ポイントとして承認
            if (points.Count == 0 || Vector2.Distance(mousePos, points[points.Count - 1]) > 0.1f)
            {
                Vector2 newPoint = mousePos;
                points.Add(newPoint);

                // 【重要】追加した「この特定の点だけ」を数秒後に消去するための専用タイマーコルーチンを個別に起動
                StartCoroutine(RemovePointRoutine(newPoint));
            }

            // 見た目とコライダーをリフレッシュ
            UpdateVisuals();
            yield return null; // 1フレーム待機して次のマウス位置チェックへ
        }

        // ループを抜けたら（ボタンを離すかインク切れ）新規の頂点追加をストップ
        isDrawing = false;
    }

    /// <summary>
    /// 特定の頂点データに対して寿命を数え、時間が来たらリストからピンポイントで取り除くタイマー
    /// </summary>
    IEnumerator RemovePointRoutine(Vector2 point)
    {
        // この点が生まれてから指定の寿命（秒）が経過するまで待つ
        yield return new WaitForSeconds(lifetime);

        // リストからこの該当する点を削除する（これにより線の一部が欠ける、または縮む表現になる）
        points.Remove(point);
        UpdateVisuals();

        // プレイヤーが既にマウスを引き上げ終えており、かつ全ての点が寿命で消え去ったら、このオブジェクトの役目は終了
        if (!isDrawing && points.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// リストに格納されているVector2の座標群をもとにLineRendererとEdgeCollider2Dの形を再生成する
    /// </summary>
    void UpdateVisuals()
    {
        // 点が2つ未満なら線を作れないため初期化して差し戻す
        if (points.Count < 2)
        {
            lr.positionCount = 0;
            if (col != null) col.points = new Vector2[0];
            return;
        }

        // LineRendererの頂点数を現在のリスト数に合わせ、Vector3配列に変換してセット
        lr.positionCount = points.Count;
        lr.SetPositions(ConvertListToVector3(points));

        // EdgeCollider2Dの接点（Points）配列を更新して、描画とコライダーを同期
        if (col != null)
        {
            col.points = points.ToArray();
        }
    }

    /// <summary>
    /// LineRendererで扱うために、Vector2のリストをVector3の配列へ詰め替えるヘルパー関数
    /// </summary>
    Vector3[] ConvertListToVector3(List<Vector2> list2D)
    {
        Vector3[] array3D = new Vector3[list2D.Count];
        for (int i = 0; i < list2D.Count; i++)
        {
            array3D[i] = new Vector3(list2D[i].x, list2D[i].y, 0);
        }
        return array3D;
    }
}