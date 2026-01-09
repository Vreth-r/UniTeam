using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SkillNodeInputHandler : MonoBehaviour
{
    public static SkillNodeInputHandler Instance { get; private set; }

    [Header("Input Actions")]
    public InputActionReference leftClickAction;
    public InputActionReference rightClickAction;
    public InputActionReference pointerPosAction;
    public InputActionReference dragAction;

    [Header("World Space Canvas")]
    public Camera uiCamera;

    [Header("Editor Mode Dragging")]
    public bool editModeEnabled = false;

    [Header("Hover")]
    public Vector2 normalSize = new Vector2(25, 25);
    public Vector2 hoverSize = new Vector2(150, 150);

    private SkillNodeUI hoveredNode = null;

    [Header("Assigned From SkillTree Generator")]
    public Dictionary<string, SkillNodeUI> allNodes = new Dictionary<string, SkillNodeUI>();

    // Dragging state
    private SkillNodeUI draggingNode = null;
    private Vector2 dragOffset;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple SkillNodeInputManagers in scene! Only one allowed.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        leftClickAction.action.Enable();
        rightClickAction.action.Enable();
        pointerPosAction.action.Enable();
        dragAction.action.Enable();

        leftClickAction.action.performed += OnLeftClickPerformed;
        leftClickAction.action.canceled += OnLeftClickReleased;
        rightClickAction.action.performed += OnRightClickPerformed;
    }

    private void OnDisable()
    {
        leftClickAction.action.performed -= OnLeftClickPerformed;
        leftClickAction.action.canceled -= OnLeftClickReleased;
        rightClickAction.action.performed -= OnRightClickPerformed;
    }

    // -------------------------
    // HOVER
    // -------------------------

    private void UpdateHover()
    {
        SkillNodeUI nodeUnderPointer = GetNodeUnderPointer();

        // if changed
        if (hoveredNode != nodeUnderPointer)
        {
            // reset prev
            if (hoveredNode != null)
            {
                hoveredNode.HoverExit(normalSize);
            }

            hoveredNode = nodeUnderPointer;

            // apply hover size
            if (hoveredNode != null && hoveredNode.isVisible)
            {
                hoveredNode.HoverEnter(hoverSize);
            }
        }
    }

    // -------------------------
    // LEFT CLICK (drag + expand)
    // -------------------------

    private void OnLeftClickPerformed(InputAction.CallbackContext ctx)
    {
        if (!editModeEnabled) return;

        SkillNodeUI node = GetNodeUnderPointer();
        if (node == null) return;

        // line mode
        if (SkillTreeManager.Instance.lineEditMode == LineEditMode.AddLine)
        {
            SkillTreeManager.Instance.TrySelectNodeForConnection(node);
            return;
        }

        // Start dragging this node
        draggingNode = node;

        Vector2 screenPos = pointerPosAction.action.ReadValue<Vector2>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            node.RectTransform.parent as RectTransform,
            screenPos,
            uiCamera,
            out var localPoint
        );

        dragOffset = node.RectTransform.anchoredPosition - localPoint;
    }

    private void OnLeftClickReleased(InputAction.CallbackContext ctx)
    {
        // If we were dragging, stop dragging and don't treat it as a click
        if (draggingNode != null)
        {
            draggingNode = null;
            return;
        }

        if (editModeEnabled) return;

        SkillNodeUI node = GetNodeUnderPointer();
        if (node == null) return;

        // // Expand/collapse node
        // if (node.data.isExpansionNode)
        //     node.ToggleExpansion();
    }

    // -------------------------
    // RIGHT CLICK (tooltip)
    // -------------------------

    private void OnRightClickPerformed(InputAction.CallbackContext ctx)
    {
        SkillNodeUI node = GetNodeUnderPointer();
        if (node == null || !node.isVisible) return;

        node.ShowTooltip();
    }

    // -------------------------
    // DRAG UPDATE
    // -------------------------

    private void Update()
    {
        UpdateHover();

        if (!editModeEnabled || draggingNode == null)
            return;

        Vector2 screenPos = dragAction.action.ReadValue<Vector2>();
        if (screenPos == Vector2.zero)
            screenPos = pointerPosAction.action.ReadValue<Vector2>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            draggingNode.RectTransform.parent as RectTransform,
            screenPos,
            uiCamera,
            out var localPoint
        );

        draggingNode.RectTransform.anchoredPosition = localPoint + dragOffset;
    }

    // -------------------------
    // HIT TEST
    // -------------------------

    private SkillNodeUI GetNodeUnderPointer()
    {
        Vector2 screenPos = pointerPosAction.action.ReadValue<Vector2>();

        // Nodes may overlap → front-most should win
        // So iterate *backwards* through hierarchy if needed
        foreach (var node in allNodes)
        {
            if (node.Value == null || !node.Value.gameObject.activeInHierarchy)
                continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(
                node.Value.RectTransform, screenPos, uiCamera))
            {
                return node.Value;
            }
        }

        return null;
    }
}
