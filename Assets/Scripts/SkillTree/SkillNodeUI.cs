using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class SkillNodeUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public Image border;

    [Header("Colors")]
    public Color unlockedColor = Color.yellow;
    public Color lockedColor = Color.gray;

    [Header("Data")]
    public SkillNodeJSON data;
    public bool unlocked;
    public bool isExpanded = true;
    public List<SkillNodeUI> children = new List<SkillNodeUI>();
    public LineRenderer parentLine;

    public RectTransform RectTransform => (RectTransform)transform;

    public void InitializeFromJSON(SkillNodeJSON nodeData)
    {
        data = nodeData;
        unlocked = nodeData.isStartingNode;
        border.color = unlocked ? unlockedColor : lockedColor;
    }

    public void Unlock()
    {
        unlocked = true;
        border.color = unlockedColor;
    }

    // -------------------------
    // EXPANSION LOGIC
    // -------------------------

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

    // -------------------------
    // TOOLTIP
    // -------------------------

    public void ShowTooltip()
    {
        SkillTooltip.Instance.Show(this);
    }
}
