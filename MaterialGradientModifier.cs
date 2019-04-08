using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class MaterialGradientModifier : MonoBehaviour
{

    Renderer _renderer;

    //public Color myColor;
    [SerializeField] Gradient gradient;

    float _gradientPosition = -1;
    public float gradientPosition
    {
        get { return _gradientPosition; }
        set
        {
            if (_gradientPosition != value)
            {
                _gradientPosition = value;
                _renderer.material.color = gradient.Evaluate(_gradientPosition);
            }

        }
    }


    //void SetGradientPosition (float position)
    //{
    //    if(position == gradientPosition)
    //        return;

    //    gradientPosition = position;
    //    _renderer.material.color = gradient.Evaluate(gradientPosition);
        
    //}


    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        
    }

    //void Start()
    //{
    //    gradientPosition = 0;

    //}

    //void Update()
    //{
    //    gradientPosition = Mathf.Sin((Time.time) * 0.5f) + 0.5f;
    //    //SetGradientPosition(Mathf.Sin((Time.time)*0.5f) + 0.5f);
    //}
}
