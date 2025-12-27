using UnityEngine;

[System.Serializable]
public class SkillTreeJSON
{
    public SkillNodeJSON[] nodes;
    public SkillConnectionJSON[] connections;
}

[System.Serializable]
public class SkillConnectionJSON
{
    public string from;
    public string to;
    public Vector2[] controlPoints; // bezier handles
    public ConnectionType type;
}

public enum ConnectionType
{
    Prerequisite,
    Antirequisite
}

[System.Serializable]
public class SkillNodeJSON
{
    // data
    public string courseCode;
    public string longTitle;
    public string courseDescription;

    public string[] prerequisites;
    public string[] antirequisites;
    public string[] customrequisites;

    // visual specifics
    public Vector2 position;
    public bool isStartingNode;
    public bool isExpansionNode;
    public bool startsExpanded;
}
