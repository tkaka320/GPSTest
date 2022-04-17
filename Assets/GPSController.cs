using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSController : MonoBehaviour
{
    [SerializeField] Text GpsStatus;
    [SerializeField] Text GpsLatitude;
    [SerializeField] Text GpsLongtitude;
    [SerializeField] Text GpsAltitude;
    [SerializeField] Text GpsHorizontal;
    [SerializeField] Text GpsStampTime;
    [SerializeField] Text GpsInitStatus;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GpsLocation());
        ButtonController.instant.onClickBtn += OnUpdateGps;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GpsLocation()
    {
#if UNITY_EDITOR
        GpsInitStatus.text = "GPS is initializing...";
        yield return new WaitForSeconds(3);
        GpsInitStatus.text = "GPS is connecting...";
        yield return new WaitWhile(() => !UnityEditor.EditorApplication.isRemoteConnected);
        yield return new WaitForSecondsRealtime(5f);       
#elif UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation))
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);

        if (!Input.location.isEnabledByUser)
        {
            GpsInitStatus.text = "GPS is Disabled in device...";
            yield break;
        }
#elif UNITY_IOS
        if (!UnityEngine.Input.location.isEnabledByUser) {
            GpsInitStatus.text = "IOS and Location not enabled";
            yield break;
        }
#endif
        Input.location.Start();
        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            GpsStatus.text = "Time Out";
            yield break;
        }
        if(Input.location.status == LocationServiceStatus.Failed)
        {
            GpsStatus.text = "Connect Failed";
            yield break;
        }
        else
        {
            GpsStatus.text = "GPS Running";
            InvokeRepeating("UpdateGPS", 1, 1);
        }
    }
    void UpdateGPS()
    {
        UnityEngine.Debug.Log("GPS Updating...");
        if(Input.location.status == LocationServiceStatus.Running)
        {
            GpsStatus.text = "GPS Running";
            GpsLatitude.text = Input.location.lastData.latitude.ToString();
            GpsLongtitude.text = Input.location.lastData.longitude.ToString();
            GpsAltitude.text = Input.location.lastData.verticalAccuracy.ToString();
            GpsHorizontal.text = Input.location.lastData.horizontalAccuracy.ToString();
            GpsStampTime.text = Input.location.lastData.timestamp.ToString();
        }
        else
        {
            GpsStatus.text = "GPS Stopped";
        }
    }

    void OnUpdateGps()
    {
        StartCoroutine(GpsLocation());
    }
    private void OnDestroy()
    {
        ButtonController.instant.onClickBtn -= OnDestroy;
    }
}
