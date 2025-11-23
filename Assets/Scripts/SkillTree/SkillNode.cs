using UnityEngine;

[System.Serializable]
public class SkillTreeJSON
{
    public SkillNodeJSON[] nodes;
}

[System.Serializable]
public class SkillNodeJSON
{
    public string id;
    public string name;
    public string description;
    public Vector2 position;
    public string[] connected;
    public bool isStartingNode;
}
