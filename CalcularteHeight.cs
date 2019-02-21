using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;

public class CalcularteHeight : MonoBehaviour
{
    public Text pose;
    public Text poseCount;

    ARSessionOrigin m_SessionOrigin;
    

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Start()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_SessionOrigin.Raycast(new Vector3(Screen.width/2, Screen.height/2, 0f), s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = s_Hits[0].pose;
            pose.text = hitPose.position.y.ToString();
            poseCount.text = s_Hits.Count.ToString();
        }



        //Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        //Ray ray = Camera.main.ScreenPointToRay(center);

        //if (m_SessionOrigin.Raycast(ray, s_Hits, TrackableType.PlaneWithinPolygon))
        //{
        //    Pose hitPose = s_Hits[0].pose;
        //    Debug.Log(hitPose.position);
        //    pose.text = hitPose.position.y.ToString();
        //}
    }

//    [SerializeField] float wallSearchDistance = 5f;
//    [SerializeField] bool drawline = true;

//    private void FixedUpdate()
//    {
//#if UNITY_EDITOR
//        if(drawline)
//        {
//            Debug.DrawRay(transform.position, transform.forward * wallSearchDistance, Color.yellow)

//        }
//#endif


//    }


}
