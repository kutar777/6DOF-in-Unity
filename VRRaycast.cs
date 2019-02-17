using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRRaycast : MonoBehaviour
{
    public Transform player;
    public Text countText;

    private Vector3 originalScale;
    private float timeCount;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;


    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
        float distance;

        if (Physics.Raycast (ray, out hit))
        {
            distance = hit.distance;
            timeCount += 0.01f;
            countText.text = timeCount.ToString("G2");
            if (timeCount > 2f)
            {
                player.position = new Vector3(hit.point.x, player.position.y, hit.point.z);
                timeCount = 0f;
            }
        }
        else
        {
            distance = Camera.main.farClipPlane * 0.95f;
            timeCount = 0f;
        }

        transform.position = Camera.main.transform.position +
            Camera.main.transform.rotation * Vector3.forward * distance;
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0.0f, 180.0f, 0.0f);
        transform.localScale = originalScale * distance;


    }
}
