using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class SkillConnectionUI : MonoBehaviour
{
    public RectTransform from;
    public RectTransform to;
    public LineRenderer line;
    public RectTransform arrowHead;

    public List<ConnectionHandle> controlHandles = new();
    public ConnectionType type;

    public int segments = 20;

    RectTransform parentRT;

    void Awake()
    {
        parentRT = transform.parent as RectTransform;
    }

    void LateUpdate()
    {
        UpdateLine();
    }

    public void UpdateLine()
    {
        if (!from || !to) return;

        Vector3 p0 = parentRT.InverseTransformPoint(from.position);
        Vector3 p2 = parentRT.InverseTransformPoint(to.position);

        Vector3 p1 = controlHandles.Count > 0
            ? parentRT.InverseTransformPoint(controlHandles[0].RectTransform.position)
            : (p0 + p2) * 0.5f;

        line.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 pos = Bezier.Evaluate(p0, p1, p2, t);
            line.SetPosition(i, pos);
        }

        UpdateArrow(p2, p1);
    }

    void UpdateArrow(Vector3 tip, Vector3 from)
    {
        if (!arrowHead) return;

        arrowHead.anchoredPosition = tip;
        Vector3 dir = (tip - from).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowHead.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void AddHandle(GameObject handlePrefab, Vector2 localPos)
    {
        var handle = Instantiate(handlePrefab, transform);
        var handleRT = handle.GetComponent<RectTransform>();
        handleRT.anchoredPosition = localPos;

        var handleCH = handleRT.GetComponent<ConnectionHandle>();
        handleCH.Initialize(this);

        controlHandles.Add(handleCH);
    }

    public void RemoveHandle(ConnectionHandle handle)
    {
        controlHandles.Remove(handle);
        DestroyImmediate(handle.gameObject);
    }
}