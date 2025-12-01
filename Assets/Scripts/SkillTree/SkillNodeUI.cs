using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class SkillNodeUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Image border;

    public Color unlockedColor = Color.yellow;
    public Color lockedColor = Color.gray;

    public SkillNodeJSON data;
    public bool unlocked;

    public RectTransform RectTransform => (RectTransform)transform;

    public void InitializeFromJSON(SkillNodeJSON nodeData)
    {
        data = nodeData;
        unlocked = nodeData.isStartingNode;
        border.color = unlocked ? unlockedColor : lockedColor;
        //icon.sprite = Resources.Load<Sprite>(data.icon);
    }

    public void Unlock()
    {
        unlocked = true;
        border.color = unlockedColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!unlocked)
        {
            unlocked = true;
            border.color = unlockedColor;
        }

        // Show tooltip (handles toggle automatically)
        SkillTooltip.Instance.Show(this);
    }
}
