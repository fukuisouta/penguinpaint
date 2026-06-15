using UnityEngine;
using TMPro;

/// <summary>
/// プレイヤーの移動距離からスコアを算出し、ハイスコアの保存を行うクラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public Transform player;         // 追従対象のプレイヤーのTransform
    public TextMeshProUGUI scoreText; // スコア表示用のUIテキスト
    private float startX;            // ゲーム開始時のプレイヤーの初期X座標
    private bool isGameOver = false; // スコア更新をストップするためのゲームオーバーフラグ

    void Start()
    {
        // 開始時のプレイヤーのX座標を「0m地点」の基準として記憶
        if (player != null) startX = player.position.x;
    }

    void Update()
    {
        // ゲームオーバー時、または必要なコンポーネントがない場合は計算しない
        if (isGameOver || player == null || scoreText == null) return;

        // 現在位置からスタート位置を引いて「進んだ距離」を算出
        float distance = player.position.x - startX;

        // 小数点以下を切り捨てて整数にし、UIテキストを更新
        int currentScore = Mathf.FloorToInt(distance);
        scoreText.text = "SCORE: " + currentScore.ToString() + "m";
    }

    /// <summary>
    /// ゲームオーバー時にスコアの計算を止め、データをセーブする（Playerスクリプト等から呼ぶ）
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return; // 二重処理の防止
        isGameOver = true;

        // 最終的なスコア（移動距離）を確定させる
        int finalScore = Mathf.FloorToInt(player.position.x - startX);

        // 1. 今回のスコアを保存
        PlayerPrefs.SetInt("LastScore", finalScore);

        // 2. ハイスコアの更新チェックと保存
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (finalScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", finalScore);
        }

        // 変更したデータをディスクに確実に書き込む
        PlayerPrefs.Save();
    }
}