using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionClickTargetUI : MonoBehaviour, IPointerClickHandler
{
    SkillConnectionUI connection;

    public void Initialize(SkillConnectionUI owner)
    {
        connection = owner;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (connection == null) return;
        connection.RequestDelete();
    }
}