using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFocusManager : MonoBehaviour {

    private static ObjectFocusManager _instance;
    public static ObjectFocusManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<ObjectFocusManager>();
                if(_instance == null)
                {
                    _instance = (new GameObject("Singleton")).AddComponent<ObjectFocusManager>();
                }
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
    void Awake()
    {
        if (Instance && Instance != this)
            Destroy(gameObject);
        Instance = this;
    }
    private void OnDisable()
    {
        Instance = null;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
