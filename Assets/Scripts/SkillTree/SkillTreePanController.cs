using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreePanController : MonoBehaviour
{
    [Header("Skill Tree Root (moves when dragging)")]
    public RectTransform content;

    [Header("Input Actions (drag from InputActions asset)")]
    public InputActionReference pointAction;
    public InputActionReference panAction;
    public InputActionReference clickAction;
    public InputActionReference scrollAction;

    [Header("Settings")]
    public float dragSpeed = 1f;
    public float zoomSpeed = 0.1f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;

    private bool dragging = false;
    private Vector2 lastPoint;

    private void OnEnable()
    {
        clickAction.action.started += StartDrag;
        clickAction.action.canceled += EndDrag;
        panAction.action.performed += DoPan;
        scrollAction.action.performed += DoScroll;

        clickAction.action.Enable();
        panAction.action.Enable();
        scrollAction.action.Enable();
        pointAction.action.Enable();
    }

    private void OnDisable()
    {
        clickAction.action.started -= StartDrag;
        clickAction.action.canceled -= EndDrag;
        panAction.action.performed -= DoPan;
        scrollAction.action.performed -= DoScroll;

        clickAction.action.Disable();
        panAction.action.Disable();
        scrollAction.action.Disable();
        pointAction.action.Disable();
    }

    // ----------- DRAGGING -------------------

    private void StartDrag(InputAction.CallbackContext ctx)
    {
        dragging = true;
        lastPoint = pointAction.action.ReadValue<Vector2>();
    }

    private void EndDrag(InputAction.CallbackContext ctx)
    {
        dragging = false;
    }

    private void DoPan(InputAction.CallbackContext ctx)
    {
        if (!dragging)
            return;

        Vector2 delta = ctx.ReadValue<Vector2>();
        content.anchoredPosition += delta * dragSpeed;
    }

    // ----------- ZOOMING -------------------

    private void DoScroll(InputAction.CallbackContext ctx)
    {
        Vector2 scroll = ctx.ReadValue<Vector2>();
        float amount = scroll.y;

        if (Mathf.Abs(amount) < 0.01f)
            return;

        float newScale = content.localScale.x + amount * zoomSpeed;
        newScale = Mathf.Clamp(newScale, minScale, maxScale);

        content.localScale = new Vector3(newScale, newScale, 1f);
    }
}
