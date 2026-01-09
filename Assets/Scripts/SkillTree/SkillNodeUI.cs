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

    [Header("Hooks")]
    public GameObject textPanelGO;
    public GameObject courseCodeGO;

    [Header("Hover Animation")]
    public float hoverAnimSpeed = 12f;

    CanvasGroup textPanelCG;
    CanvasGroup courseCodeCG;
    bool isHovered;
    Vector2 targetSize;


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
    }

    public void Awake()
    {
        RectTransform = transform as RectTransform;
        textPanelCG = GetOrAddCanvasGroup(textPanelGO);
        courseCodeCG = GetOrAddCanvasGroup(courseCodeGO);

        textPanelCG.alpha = 0f;
        courseCodeCG.alpha = 0f;

        targetSize = RectTransform.sizeDelta;
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject go)
    {
        if (!go) return null;
        var cg = go.GetComponent<CanvasGroup>();
        if (!cg) cg = go.AddComponent<CanvasGroup>();
        return cg;
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
    // HOVER
    // -------------------------

    public void HoverEnter(Vector2 size)
    {
        targetSize = size;
        isHovered = true;

        textPanelGO.SetActive(true);
        courseCodeGO.SetActive(true);
    }

    public void HoverExit(Vector2 size)
    {
        targetSize = size;
        isHovered = false;
    }

    void AnimateHover()
    {
        // smoove
        RectTransform.sizeDelta = Vector2.Lerp(RectTransform.sizeDelta, targetSize, Time.deltaTime * hoverAnimSpeed);
        float targetAlpha = isHovered ? 1f : 0f;
        if (textPanelCG)
        {
            textPanelCG.alpha = Mathf.Lerp(textPanelCG.alpha, targetAlpha, Time.deltaTime * hoverAnimSpeed);
        }

        if (courseCodeCG)
        {
            courseCodeCG.alpha = Mathf.Lerp(courseCodeCG.alpha, targetAlpha, Time.deltaTime * hoverAnimSpeed);
        }

        // hide when faded
        if (!isHovered && textPanelCG && textPanelCG.alpha < 0.01f)
        {
            textPanelGO.SetActive(false);
            courseCodeGO.SetActive(false);
        }
    }

    // -------------------------
    // UPDATE
    // -------------------------

    void Update()
    {
        AnimateHover();
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
