using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using UnityEngine.UI;

public class SessionOriginController : MonoBehaviour
{
    ARSessionOrigin aRSessionOrigin;
    public Text text3;
    public Text text4;


    // Start is called before the first frame update
    void Start()
    {
        aRSessionOrigin = GetComponent<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        var status = ARSubsystemManager.systemState;
        var data = ARSubsystemManager.inputSubsystem;


        //text1.text = status.ToString();        
        text3.text = aRSessionOrigin.camera.transform.position.ToString("G2");
        text4.text = aRSessionOrigin.camera.transform.rotation.ToString("G2");
    }
}
