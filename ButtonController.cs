using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;

public class ButtonController : MonoBehaviour
{
    public GameObject scrollView;

    public BoolReactiveProperty IsExpanded = new BoolReactiveProperty(false);

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.OnClickAsObservable()
            .Subscribe(x => IsExpanded.Value = !IsExpanded.Value);
    }

    private void Start()
    {
        IsExpanded.Subscribe(ButtonHandler);
    }

    private void ButtonHandler(bool flag)
    {
        if (flag)
        {
            var buttons = transform.parent.gameObject.GetComponentsInChildren<ButtonController>();
            foreach (var btn in buttons)
            {
                if (btn.button != button && btn.IsExpanded.Value == true)
                    btn.IsExpanded.Value = false;
            }

            scrollView.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            scrollView.SetActive(true);
            print("BT Sibling Index: " + transform.GetSiblingIndex());
            print("SV Sibling Index: " + scrollView.transform.GetSiblingIndex());
        }
        else
            scrollView.SetActive(false);
    }
}
