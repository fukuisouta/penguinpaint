using UnityEngine;
using System.Collections.Generic; // リスト（List）を使用するために必要

/// <summary>
/// プレイヤーの進行度に合わせて、足場・インク・敵をランダムかつ動的に生成し続けるクラス
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("プレハブ設定")]
    public GameObject roadPrefab;       // 生成する足場（床）のプレハブ
    public GameObject inkPrefab;        // 生成するインク回復アイテムのプレハブ
    public GameObject[] enemyPrefabs;   // 敵キャラクターのプレハブ配列（複数登録可能）
    public Transform player;            // プレイヤーのTransform（位置基準用）

    [Header("生成の調整")]
    public float roadWidth = 19.0f;     // 足場プレハブ1枚辺りの横幅（Xサイズ）
    public float spawnDistance = 100f;  // プレイヤーからどれくらい先まであらかじめ生成しておくか
    public float lifeTime = 15f;        // 生成されたオブジェクトが自動破棄されるまでの秒数

    [Header("難易度設定")]
    public float minY = -3.5f;          // 足場が生成される高さ（Y軸）の最小値
    public float maxY = 4.5f;           // 足場が生成される高さ（Y軸）の最大値
    [Range(0f, 1f)] public float inkSpawnRate = 0.15f;   // インクが生成される確率（15%）
    [Range(0f, 1f)] public float enemySpawnRate = 0.35f; // 敵が生成される確率（35%）

    [Header("インクの出現制限")]
    [Tooltip("画面内（ゲーム内）に同時に存在できるインクの最大数")]
    public int maxInkCount = 3;

    private float nextSpawnX = 0f;      // 次に足場を生成すべきX座標の書き込み位置

    // 生成され、まだシーン内に残っているインクオブジェクトを追跡するためのリスト
    private List<GameObject> activeInks = new List<GameObject>();

    void Start()
    {
        if (player != null)
        {
            // 開始直後、プレイヤーの少し先（足場2枚分先）から自動生成の帳尻を合わせる
            nextSpawnX = player.position.x + (roadWidth * 2f);
        }
    }

    void Update()
    {
        if (player == null) return;

        // リストの中から、すでにDestroyされて「空（null）」になった要素（回収されたインクなど）を綺麗に掃除する
        activeInks.RemoveAll(ink => ink == null);

        // プレイヤーの現在位置から「spawnDistance」の距離を満たすまで、ループで先々のステージを生成し続ける
        while (nextSpawnX < player.position.x + spawnDistance)
        {
            SpawnLevelPart();
        }
    }

    /// <summary>
    /// ステージの1セクション（足場、インク、敵）を組み立てて生成する
    /// </summary>
    void SpawnLevelPart()
    {
        // 毎回ランダムな高さを決定
        float targetY = Random.Range(minY, maxY);
        float spawnX = nextSpawnX;

        // 1. 基本となる足場を生成
        CreateRoad(targetY);

        // 2. インクの生成判定（上限数を超えておらず、かつ確率をパスした場合）
        if (activeInks.Count < maxInkCount && Random.value < inkSpawnRate)
        {
            CreateInk(spawnX, targetY);
        }

        // 3. 敵の生成判定
        if (Random.value < enemySpawnRate)
        {
            CreateEnemy(spawnX, targetY);
        }

        // 次の区画の生成に向けて、X座標のポインタを足場1枚分右にずらす
        nextSpawnX += roadWidth;
    }

    /// <summary>
    /// 指定の高さに足場を生成し、ライフタイム後に自動消滅させる
    /// </summary>
    void CreateRoad(float y)
    {
        Vector3 pos = new Vector3(nextSpawnX, y, 0);
        GameObject road = Instantiate(roadPrefab, pos, Quaternion.identity);
        Destroy(road, lifeTime); // 一定時間後にメモリ解放
    }

    /// <summary>
    /// 足場の少し上にインクを生成し、同時出現数管理リストに登録する
    /// </summary>
    void CreateInk(float x, float roadY)
    {
        // 足場のトップから少し浮かせた位置（1.8〜2.8ユニット上）にランダム配置
        float inkY = roadY + Random.Range(1.8f, 2.8f);
        Vector3 pos = new Vector3(x, inkY, 0);
        GameObject ink = Instantiate(inkPrefab, pos, Quaternion.identity);
        ink.tag = "InkItem";

        // リストに登録して現在数をカウントできるようにする
        activeInks.Add(ink);

        Destroy(ink, lifeTime);
    }

    /// <summary>
    /// 配列に登録された敵プレハブからランダムに1種類選び、足場の上に生成する
    /// </summary>
    void CreateEnemy(float x, float roadY)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        // 生成する敵の種類をインデックスで抽選
        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyToSpawn = enemyPrefabs[index];

        float enemyY = roadY;

        // 【仕様拡張の残痕】もしインデックス0番（特定のアザラシなど）なら高さを微調整する、といった個別処理が可能
        if (index == 0)
        {
            // 必要に応じてここに個別位置補正ロジックを記述
        }

        Vector3 pos = new Vector3(x, enemyY, 0);
        GameObject enemy = Instantiate(enemyToSpawn, pos, Quaternion.identity);
        Destroy(enemy, lifeTime);
    }
}