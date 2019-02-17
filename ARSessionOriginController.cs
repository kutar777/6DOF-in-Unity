using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.Experimental.XR;

public class ARSessionOriginController : MonoBehaviour
{
    public GameObject playerCamera;
    public Transform player_cam;
    ARSessionOrigin m_Origin;
    

    public Text text3;
    public Text text4;
    public Text text5;

    // Start is called before the first frame update
    void Start()
    {
        m_Origin = GetComponent<ARSessionOrigin>();

    }

    // Update is called once per frame
    void Update()
    {
        var state = ARSubsystemManager.systemState;
        //var camera = Camera.allCameras;
               
        if (m_Origin.camera != null)
        {
            text3.text = m_Origin.camera.transform.position.ToString("G2");
            text4.text = m_Origin.camera.transform.rotation.ToString("G2");
            //text5.text = m_Origin.camera.isActiveAndEnabled.ToString();
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_Origin.camera.enabled = !m_Origin.camera.enabled;
        }


    }
}
