using UnityEngine;

/// <summary>
/// プレイヤーが触れるとインクを回復させ、効果音を鳴らして自身を消去するアイテムクラス
/// </summary>
public class InkRefillItem : MonoBehaviour
{
    public float refillAmount = 500f; // このアイテムのインク回復量
    private InkPenScript inkSystem;   // インクを管理している本体スクリプトへの参照

    [Header("オーディオ設定")]
    [SerializeField] private AudioClip refillSE; // インスペクターで設定する回復時の効果音

    void Start()
    {
        // シーン内からインクシステム本体を自動で探し出す
        inkSystem = GameObject.FindAnyObjectByType<InkPenScript>();

        if (inkSystem == null)
        {
            Debug.LogWarning("InkPenScript が見つかりません！");
        }
    }

    /// <summary>
    /// 2Dの衝突判定（トリガー）イベント
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("【デバッグ】インクが「" + other.name + "」に触れました！");

        // 衝突相手のTagが「Player」の場合のみ回収処理を行う
        if (other.CompareTag("Player"))
        {
            if (inkSystem != null)
            {
                // --- 音を鳴らす処理 ---
                if (refillSE != null)
                {
                    // PlayClipAtPointを使うことで、このゲームオブジェクト（Item）が
                    // 直後にDestroyされても、音が途切れずに最後まで鳴り響く
                    AudioSource.PlayClipAtPoint(refillSE, Camera.main.transform.position);
                }

                // 本体側へ回復量を渡し、用済みになったアイテム自身をシーンから消去
                inkSystem.RefillInk(refillAmount);
                Destroy(gameObject);
            }
        }
    }
}