using System.Collections;
using UnityEngine;

/// <summary>経緯度取得クラス</summary>
public class LonLatGetter : MonoBehaviour
{
    /// <summary>経緯度取得間隔（秒）</summary>
    private const float _intervalSeconds = 1.0f;
   // public static float Lon;
   // public static float Lat;

    /// <summary>ロケーションサービスのステータス</summary>
    private LocationServiceStatus _locationServiceStatus;

    /// <summary>経度</summary>
    public float Longitude { get; private set; }

    /// <summary>経度</summary>
    public float Latitude { get; private set; }

    /// <summary>緯度経度情報が取得可能か</summary>
    /// <returns>可能ならtrue、不可能ならfalse</returns>
    public bool CanGetLonLat()
    {
        return Input.location.isEnabledByUser && (_locationServiceStatus ==  LocationServiceStatus.Running);
    }

    /// <summary>経緯度取得処理</summary>
    /// <returns>一定期間毎に非同期実行するための戻り値</returns>
    private IEnumerator Start()
    {
        while (true)
        {
            _locationServiceStatus = Input.location.status;
            // GPSの許可
            if (Input.location.isEnabledByUser)
            {
                switch (_locationServiceStatus)
                {
                    case LocationServiceStatus.Stopped:
                        Input.location.Start();
                        Input.compass.enabled = true;
                        break;
                    case LocationServiceStatus.Running:
                        Longitude = Input.location.lastData.longitude;
                       
                        Latitude = Input.location.lastData.latitude;
                        break;
                    default:
                        break;
                }
            }

            yield return new WaitForSeconds(_intervalSeconds);
        }
    }
}