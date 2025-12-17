using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class SkillTreeManager : MonoBehaviour
{
    public string jsonFileName = "skilltree.json";
    public SkillNodeUI nodePrefab;
    public RectTransform nodeParent;
    public LineRenderer linePrefab;

    public SkillTreePanController panController;
    public SkillNodeInputHandler inputHandler;
    private Animator uiAnimation;

    Dictionary<string, SkillNodeUI> nodeLookup = new();

    SkillTreeJSON treeData;

    // editing:

    public Button editModeButton;
    public Button saveButton;
    public bool editMode = false;
    string jsonPath;

    void Awake()
    {
        jsonPath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
    }

    void Start()
    {
        LoadJSON();
        GenerateTree();
        editModeButton.onClick.AddListener(ToggleEditMode);
        saveButton.onClick.AddListener(SaveTree);
        SkillNodeInputHandler.Instance.allNodes = nodeLookup;
    }

    void LoadJSON()
    {
        string json = File.ReadAllText(jsonPath);
        treeData = JsonUtility.FromJson<SkillTreeJSON>(json);
    }

    void GenerateTree()
    {
        // 1) Create each node UI
        foreach (var nodeData in treeData.nodes)
        {

            var ui = Instantiate(nodePrefab, nodeParent);

            // play a spawn in animation
            uiAnimation = ui.GetComponentInChildren<Animator>();
            uiAnimation.Rebind();

            ui.InitializeFromJSON(nodeData);
            ui.RectTransform.anchoredPosition = nodeData.position;

            nodeLookup[nodeData.id] = ui;
            
            // dont play animation until a timer ends idk how to do that tho LMAO
            uiAnimation.Play("node-appear");

        }

        // 2) Build child relationships
        foreach (var nd in treeData.nodes)
        {
            var parentUI = nodeLookup[nd.id];

            List<SkillNodeUI> children = new List<SkillNodeUI>();
            foreach (var childID in nd.connected)
            {
                if (nodeLookup.TryGetValue(childID, out var childUI))
                    children.Add(childUI);
            }

            parentUI.AssignChildren(children);
        }

        // 3) Create lines now that hierarchy is known
        foreach (var nd in treeData.nodes)
        {
            var fromUI = nodeLookup[nd.id];

            foreach (var targetID in nd.connected)
            {
                if (nodeLookup.TryGetValue(targetID, out var toUI))
                {
                    // Store reference to the line inside the child for hiding
                    var line = CreateLine(fromUI.RectTransform, toUI.RectTransform);
                    toUI.parentLine = line;
                }
            }
        }

        // 4) Apply initial expand/collapse state
        foreach (var nd in treeData.nodes)
        {
            var ui = nodeLookup[nd.id];

            if (ui.data.isExpansionNode)
            {
                if (ui.data.startsExpanded)
                    ui.ExpandImmediate();
                else
                    ui.CollapseImmediate();
            }
            else if (!ui.data.isStartingNode)
            {
                // Regular locked nodes default hidden unless a parent shows them
                ui.gameObject.SetActive(false);
            }
        }
    }

    void ToggleEditMode()
    {
        editMode = !editMode;

        inputHandler.editModeEnabled = editMode;

        panController.editMode = editMode;

        saveButton.gameObject.SetActive(editMode);
    }

    void SaveTree()
    {
        // Copy current RT positions back to the treeData model
        foreach (var node in treeData.nodes)
        {
            if (nodeLookup.TryGetValue(node.id, out var ui))
            {
                node.position = ui.RectTransform.anchoredPosition;
            }
        }

        // Serialize JSON
        var json = JsonUtility.ToJson(treeData, true);

        // Save to disk
        File.WriteAllText(jsonPath, json);

        Debug.Log("Tree saved to: " + jsonPath);
    }

    LineRenderer CreateLine(RectTransform a, RectTransform b)
    {
        var line = Instantiate(linePrefab, nodeParent);

        line.useWorldSpace = false;
        line.positionCount = 2;

        // Set positions (anchored space)
        // line.SetPosition(0, a.anchoredPosition);
        // line.SetPosition(1, b.anchoredPosition);

        line.numCapVertices = 8;
        line.numCornerVertices = 5;
        line.startWidth = 6f;
        line.endWidth = 6f;
        line.alignment = LineAlignment.TransformZ;

        // animate the line 
        StartCoroutine (AnimateLine(a, b, line));
    }

    private IEnumerator AnimateLine(RectTransform a, RectTransform b, LineRenderer line)
    {
        float startTime = Time.time;

        Vector3 startPosition = a.anchoredPosition;
        Vector3 endPosition = b.anchoredPosition;

        Vector3 pos = startPosition;
        while (pos != endPosition)
        {
            float t = Time.time - startTime / 0.1f;
            pos = Vector3.Lerp(startPosition, endPosition, t);
            line.SetPosition(1, pos);
            yield return null;
        }

    }

}
