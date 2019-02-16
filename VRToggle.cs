using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public class VRToggle : MonoBehaviour
{
    public Text buttonText;


    void Update()
    {

    }

    public void ToggleVR()
    {
        if (XRSettings.loadedDeviceName == "cardboard")
        {
            StartCoroutine(LoadDevice("none"));
            buttonText.text = "Disable VR Mode";
        }
        else
        {
            StartCoroutine(LoadDevice("cardboard"));
            buttonText.text = "Enable VR Mode";
        }
    }


    IEnumerator LoadDevice(string newDevice)
    {
        if (String.Compare(XRSettings.loadedDeviceName, newDevice, true) != 0)
        {
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            XRSettings.enabled = true;
        }
    }

    void DemoMethod()
    {
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        UnityEngine.XR.InputTracking.GetNodeStates(nodeStates);
        foreach (XRNodeState nodeState in nodeStates)
        {
            if(nodeState.nodeType == XRNode.RightHand)
            {
                Vector3 vel;
                if (nodeState.TryGetVelocity(out vel))
                {
                    Debug.Log("Safe to use value");

                }

            }

        }
    }
}