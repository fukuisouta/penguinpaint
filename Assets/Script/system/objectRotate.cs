using UnityEngine;

/// <summary>
/// アタッチされたオブジェクトを、Z軸を中心に毎フレーム一定速度で回転させる演出用クラス
/// </summary>
public class objectRotate : MonoBehaviour
{
    [SerializeField, Header("回転速度（プラスで反時計回り、マイナスで時計回り）")]
    private float rotationSpeed = 2.0f;

    void Update()
    {
        // 自身の回転（Rotation）のZ軸に、毎フレーム rotationSpeed 分の角度を足し合わせる
        transform.Rotate(0, 0, rotationSpeed);
    }
}