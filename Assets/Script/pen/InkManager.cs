using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マジックペンのインク残量を管理し、UIへの反映や障害物によるダメージを処理するクラス
/// </summary>
public class InkManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("インク設定")]
    public float maxInk = 100f;
    public float currentInk;
    [Tooltip("障害物に接触した際に一瞬で失われるインクの量")]
    public float damageAmount = 20f;
    [Tooltip("マウス押し込み時、1秒間に消費されるインクの基本量")]
    [SerializeField] public float consumeSpeed = 10f;

    [Header("UI設定")]
    [Tooltip("インク残量を表すUI（Imageの「Image Type: Filled」を想定）")]
    public Image inkBarFill;

    void Start()
    {
        // 開始時はインク満タン状態
        currentInk = maxInk;
        UpdateUI();
    }

    void Update()
    {
        // カウントダウン中など、プレイ中でなければ消費させない
        if (!gameManager.m_play) return;

        // マウスの左ボタン(0)を押している間は、マウスを動かしていなくても一定ペースでインクを消費
        if (Input.GetMouseButton(0))
        {
            if (currentInk > 0)
            {
                // Time.deltaTimeをかけることで、PCのフレームレートに依存せず秒単位で一定に減る
                ReduceInk(consumeSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// プレイヤー（ペン先など）が障害物に接触したときの判定
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // タグが「Obstacle（障害物）」または「Spike（トゲ）」なら一括でインクダメージを適用
        if (other.CompareTag("Obstacle") || other.CompareTag("Spike"))
        {
            ReduceInk(damageAmount);
        }
    }

    /// <summary>
    /// インクを減算し、0～最大値の範囲に収めてUIに反映する
    /// </summary>
    public void ReduceInk(float amount)
    {
        currentInk -= amount;
        currentInk = Mathf.Clamp(currentInk, 0f, maxInk); // 負の値や最大値超えを防ぐガード

        UpdateUI();
    }

    /// <summary>
    /// インクバーの見た目（FillAmount）を残量割合（0.0～1.0）に更新する
    /// </summary>
    private void UpdateUI()
    {
        if (inkBarFill != null)
        {
            inkBarFill.fillAmount = currentInk / maxInk;
        }
    }
}