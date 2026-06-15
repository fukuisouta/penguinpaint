using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マウスの移動距離と待機時間を組み合わせて、リアルタイムにインク消費を計算・制御するクラス
/// </summary>
public class InkPenScript : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("Ink Settings")]
    public float maxInk = 100f;
    private float currentInk;

    [Tooltip("マウスの移動距離（ピクセル単位）に応じた消費倍率")]
    public float consumptionRate = 0.5f;

    [Tooltip("マウスを全く動かさず、ただクリックを維持している時に1秒間で消費される量")]
    public float idleConsumptionRate = 5f;

    [Header("UI")]
    [Tooltip("インク表示用のスライダーコンポーネント")]
    public Slider inkBar;

    private Vector3 lastMousePosition; // 前フレームのマウス位置（移動距離計算用）
    private bool isDrawing = false;     // 現在描画（インク消費）中かどうかのフラグ

    void Start()
    {
        currentInk = maxInk;
        if (inkBar != null)
        {
            inkBar.maxValue = maxInk;
            inkBar.value = currentInk;
        }
    }

    void Update()
    {
        if (!gameManager.m_play) return;

        HandleInput();

        // 描画フラグが立っている間のみインクを消費させる
        if (isDrawing)
        {
            ConsumeInk();
        }
    }

    /// <summary>
    /// マウスボタンの入力状態を見て、描画状態を切り替える
    /// </summary>
    void HandleInput()
    {
        // クリックした瞬間にインクがあれば描画スタート
        if (Input.GetMouseButtonDown(0) && currentInk > 0)
        {
            isDrawing = true;
            lastMousePosition = Input.mousePosition; // 開始位置を記録
        }

        // マウスを離すか、インクが底を突いたら描画ストップ
        if (Input.GetMouseButtonUp(0) || currentInk <= 0)
        {
            isDrawing = false;
        }
    }

    /// <summary>
    /// 移動コストと時間コストを合算してインクを減らす
    /// </summary>
    void ConsumeInk()
    {
        // 1. 前フレームからのマウスの物理的な移動距離（画面上のピクセル距離）を計算
        float distance = Vector3.Distance(Input.mousePosition, lastMousePosition);

        // --- 消費量の計算 ---
        // 移動した分のコスト
        float moveCost = distance * consumptionRate;
        // 止まっていても引かれる時間分のコスト
        float timeCost = idleConsumptionRate * Time.deltaTime;

        // 2つのコストを合算してインクから差し引く
        float totalCost = moveCost + timeCost;
        currentInk -= totalCost;
        currentInk = Mathf.Max(currentInk, 0); // 0未満にならないようストッパー

        // 3. UIの更新
        if (inkBar != null)
        {
            inkBar.value = currentInk;
        }

        // 次フレームの計算に向けて、現在の位置を「直前の位置」として記憶
        lastMousePosition = Input.mousePosition;

        // インクが切れたら専用の終了処理へ
        if (currentInk <= 0)
        {
            OnInkEmpty();
        }
    }

    /// <summary>
    /// インクが無くなった瞬間の通知と後処理
    /// </summary>
    void OnInkEmpty()
    {
        Debug.Log("インクがなくなりました！");
        isDrawing = false;
    }

    /// <summary>
    /// 他のスクリプト（LineDrawerなど）が現在の残量を確かめるためのゲッター関数
    /// </summary>
    public float GetCurrentInk()
    {
        return currentInk;
    }

    /// <summary>
    /// 回復アイテム（InkRefillItem）などからインクを補給するための関数
    /// </summary>
    public void RefillInk(float amount)
    {
        currentInk += amount;
        currentInk = Mathf.Min(currentInk, maxInk); // 最大値を超えないように制限

        if (inkBar != null)
        {
            inkBar.value = currentInk;
        }
        Debug.Log("インクが回復しました。現在のインク: " + currentInk);
    }
}