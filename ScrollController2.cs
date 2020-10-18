using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ScrollController2 : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;

    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private float moveTime = 1f;

    public IntReactiveProperty selectedIndex;

    private List<GameObject> items = new List<GameObject>();

    private ScrollRect scrollRect;
    private RectTransform content;
    private RectTransform viewport;

    private Bounds m_contentBounds;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        viewport = scrollRect.viewport;
    }

    void Start()
    {
        items.Clear();

        for (int i = 0; i < 10; i++)
        {
            items.Add(Instantiate(itemPrefab, content));
        }

        selectedIndex.Subscribe(index => StaticData.model.selected.Value = index);

        StaticData.model.selected.Subscribe(index => StartCoroutine(MoveTo(GetItemPos(index), moveTime)));
    }

    void Update()
    {
        
    }
    public void OnClick()
    {
        //GetFirstItem();
        //scrollRect.verticalNormalizedPosition = 1f;
        //StartCoroutine(Move(moveTime));

        StartCoroutine(MoveTo(GetItemPos(5), moveTime));
    }

    private IEnumerator MoveTo(float value, float time)
    {
        float totalTime = 0f;

        float delta = value - scrollRect.verticalNormalizedPosition;
        float orgPos = scrollRect.verticalNormalizedPosition;
        print("orgVertical: " + orgPos);
        //while(Mathf.Abs(value - scrollRect.verticalNormalizedPosition) > 0.0001f)
        while(curve.Evaluate(totalTime/time) < 1)
        {
            print("curve: " + curve.Evaluate(totalTime / time));
            scrollRect.verticalNormalizedPosition = orgPos +
                curve.Evaluate(totalTime / time) * delta;
            totalTime += Time.deltaTime;
            yield return null;
        }
        scrollRect.verticalNormalizedPosition = orgPos + curve.Evaluate(1) * delta;

        print("vertical: " + scrollRect.verticalNormalizedPosition);
    }

    private float GetItemPos(int index)
    {
        var item = items[index];
        var rt = item.GetComponent<RectTransform>();
        var itemPosY = Mathf.Abs(rt.anchoredPosition.y);

        if (content.rect.height == 0)
            return 1;

        var deltaheight = itemPosY / (content.rect.height - viewport.rect.height);
        print("Anchored Position: " + rt.anchoredPosition.y);
        print("Delta Height : " + deltaheight);

        return 1 - deltaheight;
    }

    private GameObject GetFirstItem()
    {
        m_contentBounds.center = content.rect.center;
        m_contentBounds.size = content.rect.size;
        var boundsMin = m_contentBounds.min;
        var boundsMax = m_contentBounds.max;

        var worldToLocal = content.worldToLocalMatrix;

        foreach (var item in items)
        {
            RectTransform rt = item.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var corner in corners)
            {
                var vector3 = worldToLocal.MultiplyPoint3x4(corner);
                vMin = Vector3.Min(vector3, vMin);
                vMax = Vector3.Max(vector3, vMax);

            }

            Bounds itemBounds = new Bounds(vMin, Vector3.zero);
            itemBounds.Encapsulate(vMax);

            if (m_contentBounds.Contains(vMin))
            {
                print("vMin");
            }

            if (m_contentBounds.Contains(vMax))
            {
                print("vMax");
            }

            if (m_contentBounds.Contains(vMin) && m_contentBounds.Contains(vMax))
            {
                print("Name: " + item.name);
                print("Content: " + m_contentBounds.min + " " + m_contentBounds.max);
                print("Item: " + itemBounds.min + " " + itemBounds.max);
                return item;
#pragma warning disable CS0162 // 到達できないコードが検出されました
                break;
#pragma warning restore CS0162 // 到達できないコードが検出されました
            }
        }
        return null;
    }
}
