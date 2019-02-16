using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowARCamera : MonoBehaviour
{
    public Transform aRCamera;

    public Text text1;
    public Text text2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = aRCamera.position;
        transform.rotation = aRCamera.rotation;

        text1.text = transform.position.ToString("G2");
        text2.text = Camera.main.transform.rotation.ToString("G2");
        
        //if(m_SessionOrigin == null)
        //{
        //    Debug.Log("Null!");

        //}

        //Main_cam_text.text = Camera.main.transform.position.ToString("G2");
        //origin_cam_text.text = aRCamera.position.ToString("G2");

        //this.transform.position = m_SessionOrigin. .transform.position;
    }
}
