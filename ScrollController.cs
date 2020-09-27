using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ScrollController : MonoBehaviour
{
    private Scrollbar scrollbar;
    private ButtonController[] buttons;

    [SerializeField]
    [Range(0, 1f)]
    private float sliderValue;

    [SerializeField]
    private float buttonHeightRate = 0f;

    [SerializeField]
    private GameObject scrollView;

    private void Awake()
    {
        scrollbar = GetComponent<ScrollRect>().verticalScrollbar;
        buttons = GetComponentsInChildren<ButtonController>();
        
    }

    private void Start()
    {
        foreach (var btn in buttons)
        {
            btn.IsExpanded
                .Where(_ => true)
                .Subscribe(x =>
                {
                    var btnIndex = btn.transform.GetSiblingIndex();
                    var svIndex = scrollView.transform.GetSiblingIndex();

                    if(svIndex < btnIndex)
                        sliderValue = 1f - (btnIndex - 1)  * buttonHeightRate;
                    else
                        sliderValue = 1f - btnIndex * buttonHeightRate;

                    scrollbar.value = sliderValue;
                });
        }
        scrollbar.value = 1f;
        var rect = buttons[0].GetComponent<RectTransform>();
        buttonHeightRate = rect.sizeDelta.y / GetComponent<RectTransform>().sizeDelta.y;
    }

    private void Update()
    {
        //scrollbar.value = sliderValue;
    }
}
