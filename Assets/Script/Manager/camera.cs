using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーへの追従と、ダメージ時などのカメラシェイク（振動）を制御するクラス
/// </summary>
public class CameraManager : MonoBehaviour
{
    [Header("追従設定")]
    [SerializeField, Tooltip("左右のズレ調整（プラスで右、マイナスで左にカメラがズレる）")]
    private float _offsetX = 0f;

    [Header("振動（シェイク）設定")]
    [SerializeField, Tooltip("振動する時間（秒）")]
    private float _shakeTime = 0.3f;
    [SerializeField, Tooltip("振動の大きさ（激しさ）")]
    private float _shakeMagnitude = 0.1f;

    private Player _player;
    private Vector3 _initPos; // ゲーム開始時のカメラ位置（高さ・左端・奥行きの基準値）
    private float _shakeCount;
    private int _currentPlayerHP;

    private bool _isShaking = false;       // 現在振動中かどうかのフラグ
    private Vector3 _shakeOffset = Vector3.zero; // 振動によって発生する位置のズレ

    void Start()
    {
        // シーン上のプレイヤーを自動検索
        _player = FindAnyObjectByType<Player>();

        // ゲーム開始時のカメラ位置を記憶（カメラの「高さ(Y)」と「奥行き(Z)」をこの位置で固定するため）
        _initPos = transform.position;
    }

    // プレイヤーの移動後（Update後）にカメラを動かすことで、画面のがたつき（チャタリング）を防ぐ
    void LateUpdate()
    {
        _FollowPlayer();
    }

    /// <summary>
    /// プレイヤーのX座標を追従し、各種制限や振動を計算してカメラ位置を適用する
    /// </summary>
    private void _FollowPlayer()
    {
        if (_player == null) return;

        // 1. 左右（X軸）だけ、プレイヤーの位置 ＋ 設定したオフセットを計算
        float targetX = _player.transform.position.x + _offsetX;

        // 2. スタート地点（初期のX位置）より左にはカメラが戻らないようにクランプ（制限）
        targetX = Mathf.Clamp(targetX, _initPos.x, Mathf.Infinity);

        // 3. Y軸（高さ）とZ軸（奥行き）は初期位置のまま固定したベース座標を作成
        Vector3 basePosition = new Vector3(targetX, _initPos.y, _initPos.z);

        // 4. ベース座標に振動によるズレを足して、最終的なカメラ位置を決定
        transform.position = basePosition + _shakeOffset;
    }

    /// <summary>
    /// 外部（Playerスクリプトなど）から呼び出すことで、カメラを振動させる
    /// </summary>
    public void StartShake()
    {
        // 二重起動を防止
        if (!_isShaking)
        {
            StartCoroutine(_Shake());
        }
    }

    /// <summary>
    /// ランダムなノイズで指定時間カメラを揺らすコルーチン
    /// </summary>
    private IEnumerator _Shake()
    {
        _isShaking = true;
        _shakeCount = 0f;

        while (_shakeCount < _shakeTime)
        {
            // 設定されたマグニチュード（大きさ）の範囲内でランダムな座標のズレを計算
            float x = Random.Range(-_shakeMagnitude, _shakeMagnitude);
            float y = Random.Range(-_shakeMagnitude, _shakeMagnitude);

            _shakeOffset = new Vector3(x, y, 0f);
            _shakeCount += Time.deltaTime;

            yield return null; // 1フレーム待機
        }

        // 振動が終わったらズレを完全にリセット
        _shakeOffset = Vector3.zero;
        _isShaking = false;
    }
}