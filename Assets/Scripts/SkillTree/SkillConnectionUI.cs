using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class SkillConnectionUI : MonoBehaviour
{
    public RectTransform from;
    public RectTransform to;
    public LineRenderer line;
    public RectTransform arrowHead; // yeah bro where

    Coroutine fadeRoutine;
    float currentAlpha = 1f;

    public List<ConnectionHandle> controlHandles = new();
    public ConnectionType type;

    public int segments = 20;

    public RectTransform clickTarget; // assign at runtime

    RectTransform parentRT;

    // -------------------------
    // INIT
    // -------------------------

    void Awake()
    {
        parentRT = transform.parent as RectTransform;
    }

    // -------------------------
    // MOVING
    // -------------------------

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
        UpdateClickTarget();
    }

    void UpdateArrow(Vector3 tip, Vector3 from)
    {
        if (!arrowHead) return;

        arrowHead.anchoredPosition = tip;
        Vector3 dir = (tip - from).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowHead.rotation = Quaternion.Euler(0, 0, angle);
    }

    void UpdateClickTarget()
    {
        if (!clickTarget) return;

        int midIndex = segments / 2;
        Vector3 midPos = line.GetPosition(midIndex);

        clickTarget.anchoredPosition = midPos;
    }

    // -------------------------
    // HANDLES
    // -------------------------

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

    // -------------------------
    // VISIBILITY
    // -------------------------

    public void SetVisible(bool visible, float duration = 0.2f)
    {
        float target = visible ? 1f : 0f;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeConnection(target, duration));
    }

    void ApplyAlpha(float a)
    {
        SetLineAlpha(a);
        SetArrowAlpha(a);

        // Optional: edit-mode click target
        if (clickTarget)
        {
            var img = clickTarget.GetComponent<UnityEngine.UI.Image>();
            if (img)
            {
                var c = img.color;
                c.a = a;
                img.color = c;
            }
        }
    }

    IEnumerator FadeConnection(float target, float duration)
    {
        float start = currentAlpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            currentAlpha = Mathf.Lerp(start, target, t / duration);

            ApplyAlpha(currentAlpha);
            yield return null;
        }

        currentAlpha = target;
        ApplyAlpha(currentAlpha);
    }

    void SetLineAlpha(float a)
    {
        if (!line) return;

        var sc = line.startColor;
        var ec = line.endColor;

        sc.a = a;
        ec.a = a;

        line.startColor = sc;
        line.endColor = ec;
    }


    void SetArrowAlpha(float a)
    {
        if (!arrowHead) return;

        var img = arrowHead.GetComponent<UnityEngine.UI.Image>();
        if (!img) return;

        var c = img.color;
        c.a = a;
        img.color = c;
    }


    // -------------------------
    // EDITING
    // -------------------------
    public void RequestDelete()
    {
        if (SkillTreeManager.Instance == null) return;
        if (!SkillTreeManager.Instance.editMode) return;
        SkillTreeManager.Instance.TryDeleteConnection(this);
    }
}