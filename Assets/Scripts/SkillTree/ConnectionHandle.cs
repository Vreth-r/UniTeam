using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionHandle : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public RectTransform RectTransform { get; private set; }
    SkillConnectionUI connection;

    void Awake()
    {
        RectTransform = (RectTransform)transform;
    }

    public void Initialize(SkillConnectionUI owner)
    {
        connection = owner;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // optional: highlight connection
        Debug.Log("Drag start");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (connection == null || RectTransform == null)
        {
            return;
        }

        RectTransform parentRT = RectTransform.parent as RectTransform;
        if (parentRT == null)
        {
            return;
        }
        Camera cam = eventData.pressEventCamera;

        // Screen Space Overlay uses null camera
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRT,
            eventData.position,
            cam,
            out Vector2 localPos
        );

        RectTransform.anchoredPosition = localPos;
        connection.UpdateLine();
    }
}