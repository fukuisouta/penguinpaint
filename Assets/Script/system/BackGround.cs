using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2Dゲームに奥行きを表現する視差効果（パララックススクロール）と、背景画像の無限ループを制御するクラス
/// </summary>
public class BackGround : MonoBehaviour
{
    [SerializeField, Header("視差効果（0=カメラと完全に同期、1=その場に固定）"), Range(0, 1)]
    private float _parallaxEffect;

    private GameObject _camera;
    private float _length;     // 背景画像（SpriteRenderer）の純粋な横幅（ユニットサイズ）
    private float _startPosX;  // 背景オブジェクトの基準となる初期X座標

    void Start()
    {
        // 開始時のX位置を記録
        _startPosX = transform.position.x;

        // SpriteRendererのバウンズ（サイズ）から、画像の正確な横幅を取得
        _length = GetComponent<SpriteRenderer>().bounds.size.x;

        // メインカメラを取得
        _camera = Camera.main.gameObject;
    }

    private void FixedUpdate()
    {
        // 物理演算やカメラ移動の同期ズレ・ガタつきを防ぐため、FixedUpdateで処理
        _Parallax();
    }

    /// <summary>
    /// カメラの座標に応じて背景の位置をずらし、ループ境界を超えたら位置をワープさせる
    /// </summary>
    private void _Parallax()
    {
        // temp: ループ判定用のカメラ相対移動量（1に近づくほどカメラから置いていかれる計算になる）
        float temp = _camera.transform.position.x * (1 - _parallaxEffect);

        // dist: 実際に背景を動かすべき移動量
        float dist = _camera.transform.position.x * _parallaxEffect;

        // 基準位置 ＋ 計算した移動量を適用（Y軸とZ軸は現在の位置をキープ）
        transform.position = new Vector3(_startPosX + dist, transform.position.y, transform.position.z);

        // --- 無限ループ処理 ---
        // カメラが背景の右側の境界（幅1枚分）を超えたら、基準位置を右側にスライドさせる
        if (temp > _startPosX + _length)
        {
            _startPosX += _length;
        }
        // 反対に、カメラが左側の境界を超えたら基準位置を左側にスライドさせる
        else if (temp < _startPosX - _length)
        {
            _startPosX -= _length;
        }
    }
}