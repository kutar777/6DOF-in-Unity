using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR;
using DebugWindow;
//using Assets.Scripts.Core.Utils;

[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARPlaneManager))]
public class MeasureHeight : MonoBehaviour
{
    Camera m_ARCamera;
    ARSessionOrigin m_SessionOrigin;
           
    Vector3 ScreenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    Pose poseHit;

    
    private float _originHeight;

    public float originHeight
    {
        get
        {
            return _originHeight;
        }

        set
        {
            _originHeight = value;
        }
    }

    void Awake ()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_ARCamera = m_SessionOrigin.camera;
    }
    void OnEnable ()
    {
        GetComponent<ARPlaneManager>().planeAdded += OnPlaneAdded;
    
    }
    void OnDisable()
    {
        GetComponent<ARPlaneManager>().planeAdded -= OnPlaneAdded;
    }
    void OnPlaneAdded(ARPlaneAddedEventArgs eventArgs)
    {
        var plane = eventArgs.plane;

        //　水平地面であるか確認する
        if (plane.boundedPlane.Alignment == PlaneAlignment.Horizontal)
        {
            var state = plane.trackingState;
            Logging.Debug("Plane detection is: " + state.ToString());

            var name = plane.boundedPlane.Id;
            //// Add to our log so the user knows something happened.
            //if (logText != null)
            //    logText.text = string.Format("\n{0}", plane.boundedPlane.Id);
        }
    }

    void Update()
    {
        // ARがOFFだと、レイキャストは飛ばさない
        if (ARSubsystemManager.systemState > ARSystemState.Ready)
        {
            // VRモード中は、レイキャストは飛ばさない
            if (!XRSettings.enabled)
            {
                DetectPlanes();
            }
        }
    }
    public void DetectPlanes()
    {
        Ray ray = m_ARCamera.ScreenPointToRay(ScreenCenter);
        //Logging.Debug("Preparing Ray");
        if (m_SessionOrigin.Raycast(ray, s_Hits, TrackableType.Planes))
        {
            //Logging.Debug("Raycasting");
            poseHit = s_Hits[0].pose;

            // 検知された地面のｙ座標の絶対値をデバイス高さに変換
            _originHeight = Mathf.Abs(poseHit.position.y);
        }

    }
    void OnGUI()
    {
        GUILayout.Label("Height: " + originHeight);
        GUILayout.Label("XR: " + XRSettings.enabled.ToString());
    }
}
