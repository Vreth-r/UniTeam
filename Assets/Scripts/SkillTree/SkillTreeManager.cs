using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;
public enum TreeContext
{
    Acting,
    Dance,
    Production,
    All
}

public enum LineEditMode
{
    None,
    AddLine,
    DeleteLine
}

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance { get; private set; }
    public string jsonFileName = "NMA.json";
    public SkillNodeUI nodePrefab;
    public RectTransform nodeParent;
    public LineRenderer linePrefab;
    public GameObject connectionClickTargetPrefab;

    public SkillTreePanController panController;
    public SkillNodeInputHandler inputHandler;
    private Animator uiAnimation;

    Dictionary<string, SkillNodeUI> nodeLookup = new();

    SkillTreeJSON treeData;

    public TreeContext context = TreeContext.All;

    // other buttons

    public Button allProgramsButton;
    public Button actingButton;
    public Button danceButton;
    public Button productionButton;

    // editing:

    public Button editModeButton;
    public Button saveButton;
    public GameObject controlHandlePrefab;
    public bool editMode = true;
    string jsonPath;

    // line editing
    public Button addLineButton;
    public Button deleteLineButton;
    [HideInInspector]
    public LineEditMode lineEditMode = LineEditMode.None;
    SkillNodeUI pendingFromNode;

    void Awake()
    {
        jsonPath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (Instance != null)
        {
            Debug.LogError("Multiple SkillTreeManagers in scene! Only one allowed.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        LoadJSON();
        GenerateTree();
        editModeButton.onClick.AddListener(ToggleEditMode);
        saveButton.onClick.AddListener(SaveTree);
        allProgramsButton.onClick.AddListener(() => SetContext(TreeContext.All));
        actingButton.onClick.AddListener(() => SetContext(TreeContext.Acting));
        danceButton.onClick.AddListener(() => SetContext(TreeContext.Dance));
        productionButton.onClick.AddListener(() => SetContext(TreeContext.Production));
        addLineButton.onClick.AddListener(() =>
        {
            lineEditMode = LineEditMode.AddLine;
            pendingFromNode = null;
        });
        deleteLineButton.onClick.AddListener(() =>
        {
            lineEditMode = LineEditMode.DeleteLine;
        });
        SkillNodeInputHandler.Instance.allNodes = nodeLookup;
        ToggleEditMode();
    }

    void LoadJSON()
    {
        string json = File.ReadAllText(jsonPath);
        treeData = JsonUtility.FromJson<SkillTreeJSON>(json);
    }

    void GenerateTree()
    {
        // 1) Create each node UI
        foreach (var course in treeData.nodes)
        {
            var ui = Instantiate(nodePrefab, nodeParent);
            ui.InitializeFromJSON(course);
            ui.ApplyProgramContext(context);
            ui.RectTransform.anchoredPosition = course.position;

            nodeLookup[course.courseCode] = ui;
        }

        // reworking this
        // 2) assign relationships derived
        foreach (var course in treeData.nodes)
        {
            var thisUI = nodeLookup[course.courseCode];

            List<SkillNodeUI> parents = new();

            if (course.prerequisites != null)
            {
                foreach (var prereqCode in course.prerequisites)
                {
                    if (nodeLookup.TryGetValue(prereqCode, out var prereqUI))
                    {
                        parents.Add(prereqUI);
                    }
                }
            }

            //thisUI.AssignParents(parents);
        }

        // 3) Create lines now that hierarchy is known
        if (treeData.connections != null && treeData.connections.Length > 0)
        {
            GenerateConnectionsFromJSON();
        }
        else
        {
            AutoGenerateConnections();
        }
    }

    public void SetContext(TreeContext newContext)
    {
        if (context == newContext)
            return;

        context = newContext;
        ApplyContextToAllNodes();
    }

    void ApplyContextToAllNodes()
    {
        foreach (var kvp in nodeLookup)
        {
            var nodeUI = kvp.Value;
            nodeUI.ApplyProgramContext(context);
        }
    }

    void ToggleEditMode()
    {
        editMode = !editMode;

        if (!editMode)
        {
            lineEditMode = LineEditMode.None;
            pendingFromNode = null;
        }

        inputHandler.editModeEnabled = editMode;

        panController.editMode = editMode;

        saveButton.gameObject.SetActive(editMode);

        foreach (var conn in FindObjectsByType<SkillConnectionUI>(FindObjectsSortMode.None))
        {
            if (conn.clickTarget)
                conn.clickTarget.gameObject.SetActive(editMode);

            foreach (var handle in conn.controlHandles)
                handle.gameObject.SetActive(editMode);
        }
    }

    void SaveTree()
    {
        // Copy current RT positions back to the treeData model
        foreach (var course in treeData.nodes)
        {
            if (nodeLookup.TryGetValue(course.courseCode, out var ui))
            {
                course.position = ui.RectTransform.anchoredPosition;
            }
        }

        var connections = new List<SkillConnectionJSON>();

        foreach (var conn in FindObjectsByType<SkillConnectionUI>(FindObjectsSortMode.None))
        {
            var conjson = new SkillConnectionJSON
            {
                from = conn.from.GetComponent<SkillNodeUI>().data.courseCode,
                to = conn.to.GetComponent<SkillNodeUI>().data.courseCode,
                type = conn.type,
                controlPoints = GetHandlePositions(conn)
            };

            connections.Add(conjson);
        }

        treeData.connections = connections.ToArray();

        var json = JsonUtility.ToJson(treeData, true);
        File.WriteAllText(jsonPath, json);
        Debug.Log("Tree saved to: " + jsonPath);
    }

    void GenerateConnectionsFromJSON()
    {
        foreach (var c in treeData.connections)
        {
            if (!nodeLookup.TryGetValue(c.from, out var fromUI)) continue;
            if (!nodeLookup.TryGetValue(c.to, out var toUI)) continue;

            var conn = CreateConnection(fromUI, toUI, c.type, false);

            // Recreate handles
            if (c.controlPoints != null)
            {
                foreach (var p in c.controlPoints)
                {
                    conn.AddHandle(controlHandlePrefab, p);
                }
            }
        }
    }

    public void TrySelectNodeForConnection(SkillNodeUI node)
    {
        if (lineEditMode != LineEditMode.AddLine)
        {
            return;
        }

        if (pendingFromNode == null)
        {
            pendingFromNode = node;
            Debug.Log("Selected FROM node: " + node.data.courseCode);
            return;
        }

        if (pendingFromNode == node)
        {
            return;
        }

        // create con
        CreateConnection(pendingFromNode, node, ConnectionType.Prerequisite, true);

        pendingFromNode = null;
        lineEditMode = LineEditMode.None;
    }

    public void TryDeleteConnection(SkillConnectionUI conn)
    {
        if (lineEditMode != LineEditMode.DeleteLine)
        {
            return;
        }

        DestroyImmediate(conn.gameObject);
        lineEditMode = LineEditMode.None;
    }

    void AutoGenerateConnections()
    {
        foreach (var course in treeData.nodes)
        {
            var toUI = nodeLookup[course.courseCode];

            if (course.prerequisites != null)
            {
                foreach (var prereq in course.prerequisites)
                {
                    if (nodeLookup.TryGetValue(prereq, out var fromUI))
                    {
                        CreateConnection(fromUI, toUI, ConnectionType.Prerequisite, true);
                    }
                }
            }

            if (course.antirequisites != null)
            {
                foreach (var antireq in course.antirequisites)
                {
                    if (nodeLookup.TryGetValue(antireq, out var fromUI))
                    {
                        CreateConnection(fromUI, toUI, ConnectionType.Antirequisite, true);
                    }
                }
            }
        }
    }

    Vector2[] GetHandlePositions(SkillConnectionUI conn)
    {
        var list = new List<Vector2>();

        foreach (var handle in conn.controlHandles)
        {
            list.Add(handle.RectTransform.anchoredPosition);
        }

        return list.ToArray();
    }

    SkillConnectionUI CreateConnection(
    SkillNodeUI from,
    SkillNodeUI to,
    ConnectionType type,
    bool createDefaultHandle)
    {
        var go = new GameObject("Connection");
        go.transform.SetParent(nodeParent, false);

        var conn = go.AddComponent<SkillConnectionUI>();
        var rt = go.AddComponent<RectTransform>();
        conn.from = from.RectTransform;
        conn.to = to.RectTransform;
        conn.type = type;

        conn.line = Instantiate(linePrefab, go.transform);
        conn.line.textureMode = LineTextureMode.Tile;

        conn.line.startColor = type == ConnectionType.Prerequisite ? Color.blue : Color.red;
        conn.line.endColor = conn.line.startColor;

        // Create control point
        if(createDefaultHandle)
        {
            Vector2 mid = (conn.from.anchoredPosition + conn.to.anchoredPosition) * 0.5f;
            conn.AddHandle(controlHandlePrefab, mid);
        }

        var clickGO = Instantiate(connectionClickTargetPrefab, go.transform);
        var clickRT = clickGO.GetComponent<RectTransform>();
        conn.clickTarget = clickRT;

        var clickUI = clickGO.GetComponent<ConnectionClickTargetUI>();
        clickUI.Initialize(conn);

        clickGO.SetActive(editMode);

        to.RegisterIncomingConnection(conn);
        return conn;
    }
}
