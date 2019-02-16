using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleChanger : MonoBehaviour
{
    public Transform unitySpace;
    public Text buttonText;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeScale()
    {
        unitySpace.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        buttonText.text = unitySpace.localScale.ToString("G2");
        
    }

}
