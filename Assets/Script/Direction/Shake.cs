using UnityEngine;

/// <summary>
/// パーリンノイズを用いて、オブジェクトをランダムかつ滑らかに揺らすクラス（カメラシェイク等に利用）
/// </summary>
public class Shake : MonoBehaviour
{
    [SerializeField, Tooltip("全体の揺れの大きさ")] public float shakeRange = 1.0f;
    [SerializeField, Tooltip("揺れるスピード")] public float shakeSpeed = 2.0f;
    [SerializeField, Tooltip("X軸方向の揺れ倍率")] public float shakeX = 1.0f;
    [SerializeField, Tooltip("Y軸方向の揺れ倍率")] public float shakeY = 1.0f;

    private Vector3 initialPosition; // 揺れの基準となる初期座標

    void Start()
    {
       
        initialPosition = transform.position;
    }

    void Update()
    {
        // Mathf.PerlinNoise は 0.0 ～ 1.0 を返すため、2倍して1を引くことで 「-1.0 ～ 1.0」 の範囲に変換
        // 時間（Time.time）をシード値にすることで、毎フレーム滑らかに変化する値を抽出
        float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0.0f) * 2 - 1;
        float offsetY = Mathf.PerlinNoise(0.0f, Time.time * shakeSpeed) * 2 - 1;

        // 各軸のインスペクター設定値を乗算して、揺れの幅を調整
        offsetX *= shakeRange * shakeX;
        offsetY *= shakeRange * shakeY;

        // 初期位置にオフセット（ズレ）を加算して座標を更新
        transform.position = initialPosition + new Vector3(offsetX, offsetY, 0);
    }
}