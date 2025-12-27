using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    public RectTransform RectTransform => (RectTransform)transform;

    // GRAPH
    [Header("Graph")]
    public List<SkillConnectionUI> incomingConnections = new();

    // -------------------------
    // INITIALIZATION
    // -------------------------

    public void InitializeFromJSON(SkillNodeJSON nodeData)
    {
        data = nodeData;
        unlocked = data.isStartingNode;
        border.color = unlocked ? unlockedColor : lockedColor;
    }

    // -------------------------
    // LINES
    // -------------------------
    public void RegisterIncomingConnection(SkillConnectionUI connection)
    {
        incomingConnections.Add(connection);
    }

    public void SetConnectionsVisible(bool visible)
    {
        foreach (var c in incomingConnections)
            if (c != null)
                c.gameObject.SetActive(visible);
    }

    // -------------------------
    // TOOLTIP
    // -------------------------

    public void ShowTooltip()
    {
        SkillTooltip.Instance.Show(this);
    }
}
