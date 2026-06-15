using UnityEngine;

/// <summary>
/// プレイヤーの後方に通り過ぎたオブジェクトを検知し、自動的に破棄してメモリ負荷を軽減するクラス
/// </summary>
public class Destroyer : MonoBehaviour
{
    private Transform player; // プレイヤーのTransform参照

    void Start()
    {
        // シーン内から「Player」タグが付いたオブジェクトを自動検索して位置を追跡
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 自身（このスクリプトがついたオブジェクト）のX座標が、プレイヤーより20ユニット以上後ろ（左側）になった場合
        if (player.position.x - 20f > transform.position.x)
        {
            // 用済みとみなし、シーンから完全に消去
            Destroy(gameObject);
        }
    }
}