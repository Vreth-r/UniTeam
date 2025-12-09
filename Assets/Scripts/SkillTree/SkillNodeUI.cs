using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SkillNodeUI : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image icon;
    public Image border;

    public Color unlockedColor = Color.yellow;
    public Color lockedColor = Color.gray;

    public SkillNodeJSON data;
    public bool unlocked;
    public bool isExpanded = true;
    public List<SkillNodeUI> children = new List<SkillNodeUI>();

    public LineRenderer parentLine;

    public RectTransform RectTransform => (RectTransform)transform;

    // editing vars
    bool isDragging = false;
    Vector2 dragOffset;

    public bool editModeEnabled = false; // set by manager
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!editModeEnabled) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            RectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        dragOffset = RectTransform.anchoredPosition - localPoint;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !editModeEnabled) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            RectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        RectTransform.anchoredPosition = localPoint + dragOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void InitializeFromJSON(SkillNodeJSON nodeData)
    {
        data = nodeData;
        unlocked = nodeData.isStartingNode;
        border.color = unlocked ? unlockedColor : lockedColor;
        //icon.sprite = Resources.Load<Sprite>(data.icon);
    }

    public void Unlock()
    {
        unlocked = true;
        border.color = unlockedColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Always show tooltip when clicking any node
        SkillTooltip.Instance.Show(this);

        // Additional behavior ONLY for expansion nodes
        if (data.isExpansionNode)
        {
            ToggleExpansion();
        }
    }

    public void ToggleExpansion()
    {
        if (isExpanded)
            CollapseRecursively();
        else
            Expand();
    }

    public void Expand()
    {
        isExpanded = true;

        foreach (var child in children)
        {
            child.gameObject.SetActive(true);

            if (child.parentLine != null)
                child.parentLine.gameObject.SetActive(true);

            // If the child is an expansion node AND starts expanded
            if (child.data.isExpansionNode && child.data.startsExpanded)
                child.Expand();
        }
    }

    public void CollapseRecursively()
    {
        isExpanded = false;

        foreach (var child in children)
        {
            child.gameObject.SetActive(false);

            if (child.parentLine != null)
                child.parentLine.gameObject.SetActive(false);

            // Force collapse deeper nodes
            child.CollapseRecursively();
        }
    }

    public void ExpandImmediate()
    {
        isExpanded = true;

        foreach (var child in children)
        {
            child.gameObject.SetActive(true);

            if (child.parentLine != null)
                child.parentLine.gameObject.SetActive(true);

            if (child.data.isExpansionNode && child.data.startsExpanded)
                child.ExpandImmediate();
        }
    }

    public void CollapseImmediate()
    {
        isExpanded = false;

        foreach (var child in children)
        {
            child.gameObject.SetActive(false);

            if (child.parentLine != null)
                child.parentLine.gameObject.SetActive(false);

            child.CollapseImmediate();
        }
    }


    public void AssignChildren(List<SkillNodeUI> childNodes)
    {
        children = childNodes;
    }
}
