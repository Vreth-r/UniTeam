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
    public bool isVisible = true;

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
                c.SetVisible(visible);
    }

    // -------------------------
    // TOOLTIP
    // -------------------------

    public void ShowTooltip()
    {
        SkillTooltip.Instance.Show(this);
    }

    // -------------------------
    // VISIBILITY
    // -------------------------

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        float alpha = visible ? 1f : 0f;
        SetImageAlpha(icon, alpha);
        SetImageAlpha(border, alpha);

        // Fade all graphics
        var graphics = GetComponentsInChildren<Graphic>(true);
        foreach (var g in graphics)
        {
            var c = g.color;
            c.a = alpha;
            g.color = c;

            // Disable interaction
            g.raycastTarget = visible;
        }
    }

    void SetImageAlpha(Image img, float a)
    {
        if (!img) return;
        var c = img.color;
        c.a = a;
        img.color = c;
    }

    // -------------------------
    // CONTEXT SWITCHING
    // -------------------------

    public void ApplyProgramContext(TreeContext program)
    {
        if (program == TreeContext.All || data.context == null)
        {
            border.color = baseColor;
            SetVisible(true);
            SetConnectionsVisible(true);
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
            SetVisible(false);
            SetConnectionsVisible(false);
            return;
        }

        switch (requirement.ToLower())
        {
            case "required":
                border.color = requiredColor;
                SetVisible(true);
                SetConnectionsVisible(true);
                break;
            case "elective":
                border.color = electiveColor;
                SetVisible(true);
                SetConnectionsVisible(true);
                break;
            default:
                border.color = unavailableColor;
                SetVisible(false);
                SetConnectionsVisible(false);
                break;
        }
    }
}
