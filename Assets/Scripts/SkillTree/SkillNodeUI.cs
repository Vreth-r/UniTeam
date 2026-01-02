using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SkillNodeUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public Image border;
    public TextMeshProUGUI courseCodeText;

    [Header("Colors")]
    public Color baseColor = Color.gray;
    public Color requiredColor = Color.purple;
    public Color electiveColor = Color.green;
    public Color unavailableColor = Color.black;

    [Header("Data")]
    public SkillNodeJSON data;

    public bool unlocked;

    // GRAPH
    [Header("Graph")]
    public List<SkillConnectionUI> incomingConnections = new();

    public RectTransform RectTransform { get; private set; }

    // -------------------------
    // INITIALIZATION
    // -------------------------

    public void InitializeFromJSON(SkillNodeJSON nodeData)
    {
        data = nodeData;
        unlocked = data.isStartingNode;
        border.color = requiredColor;
        courseCodeText.text = data.courseCode;
        RectTransform = transform as RectTransform;
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

    // -------------------------
    // CONTEXT SWITCHING
    // -------------------------

    public void ApplyProgramContext(TreeContext program)
    {
        if (program == TreeContext.All || data.context == null)
        {
            border.color = baseColor;
            return;
        }

        string requirement = program switch
        {
            TreeContext.Acting => data.context.Acting,
            TreeContext.Production => data.context.Production,
            TreeContext.Dance => data.context.Dance,
            _ => null
        };

        if (string.IsNullOrEmpty(requirement) || requirement == "none")
        {
            border.color = unavailableColor;
            return;
        }

        switch (requirement.ToLower())
        {
            case "required":
                border.color = requiredColor;
                break;
            case "elective":
                border.color = electiveColor;
                break;
            default:
                border.color = unavailableColor;
                break;
        }
    }
}
