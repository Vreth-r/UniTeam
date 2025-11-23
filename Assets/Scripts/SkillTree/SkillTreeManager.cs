using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SkillTreeManager : MonoBehaviour
{
    public string jsonFileName = "skilltree.json";
    public SkillNodeUI nodePrefab;
    public RectTransform nodeParent;
    public LineRenderer linePrefab;

    Dictionary<string, SkillNodeUI> nodeLookup = new();

    SkillTreeJSON treeData;

    void Start()
    {
        LoadJSON();
        GenerateTree();
    }

    void LoadJSON()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        string json = File.ReadAllText(path);
        treeData = JsonUtility.FromJson<SkillTreeJSON>(json);
    }

    void GenerateTree()
    {
        // Create each node
        foreach (var nodeData in treeData.nodes)
        {
            var ui = Instantiate(nodePrefab, nodeParent);
            ui.InitializeFromJSON(nodeData);

            ui.RectTransform.anchoredPosition = nodeData.position;

            nodeLookup[nodeData.id] = ui;
        }

        // Create lines between nodes
        foreach (var nd in treeData.nodes)
        {
            var fromUI = nodeLookup[nd.id];

            foreach (var targetID in nd.connected)
            {
                if (nodeLookup.TryGetValue(targetID, out var toUI))
                {
                    CreateLine(fromUI.RectTransform, toUI.RectTransform);
                }
            }
        }
    }

    void CreateLine(RectTransform a, RectTransform b)
    {
        var line = Instantiate(linePrefab, nodeParent);

        line.useWorldSpace = false;
        line.positionCount = 2;

        // Set positions (anchored space)
        line.SetPosition(0, a.anchoredPosition);
        line.SetPosition(1, b.anchoredPosition);

        // Nice smooth ends
        line.numCapVertices = 8;
        line.numCornerVertices = 5;

        // Crisp thin UI-line style
        line.startWidth = 6f; 
        line.endWidth = 6f;

        line.alignment = LineAlignment.TransformZ;
    }
}
