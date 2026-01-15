using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

public class SkillTooltip : MonoBehaviour
{
    public static SkillTooltip Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public float fadeDuration = 0.15f;
    public Vector2 defaultOffset = new Vector2(150f, 0f);
    public Vector2 edgeMargin = new Vector2(20f, 20f);

    private RectTransform rect;
    private Canvas canvas;
    private bool isVisible = false;
    private SkillNodeUI currentNode;

    void Awake()
    {
        Instance = this;
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show tooltip anchored near the node
    /// </summary>
    public void Show(SkillNodeUI node)
    {
        if (currentNode == node)
        {
            Hide();
            return;
        }

        currentNode = node;
        titleText.text = node.data.courseCode;
        bodyText.text = node.data.courseDescription;

        gameObject.SetActive(true);

        RebuildLayout(); // ensures panel resizes b4 positioning

        PositionTooltip(node.RectTransform);

        isVisible = true;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    private void RebuildLayout()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void Hide()
    {
        if (!isVisible) return;

        isVisible = false;
        currentNode = null;
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private void PositionTooltip(RectTransform target)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        // Get node world corners
        Vector3[] nodeCorners = new Vector3[4];
        target.GetWorldCorners(nodeCorners);

        // Top-right corner of node (screen space)
        Vector2 nodeTopRightScreen =
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, nodeCorners[2]);

        // Initial desired screen position (to the right of node)
        Vector2 desiredScreenPos = nodeTopRightScreen + defaultOffset;

        // Convert screen position to canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            desiredScreenPos,
            canvas.worldCamera,
            out Vector2 localPos
        );

        // Tooltip uses top-left pivot
        rect.anchoredPosition = localPos;

        // Clamp tooltip fully inside canvas
        ClampToCanvas(canvasRect);
    }

    private IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float t = 0;
        float startAlpha = canvasGroup.alpha;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    private void ClampToCanvas(RectTransform canvasRect)
    {
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 tooltipSize = rect.rect.size;

        Vector2 pos = rect.anchoredPosition;

        float minX = -canvasSize.x / 2f + edgeMargin.x;
        float maxX =  canvasSize.x / 2f - tooltipSize.x - edgeMargin.x;

        float maxY =  canvasSize.y / 2f - edgeMargin.y;
        float minY = -canvasSize.y / 2f + tooltipSize.y + edgeMargin.y;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rect.anchoredPosition = pos;
    }


    [Header("New Input System")]
    public InputActionReference clickAction; // Assign SkillTree/Click

    void OnEnable()
    {
        if (clickAction != null)
            clickAction.action.performed += OnClickPerformed;
    }

    void OnDisable()
    {
        if (clickAction != null)
            clickAction.action.performed -= OnClickPerformed;
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (!isVisible) return;

        Vector2 clickPosition = Mouse.current.position.ReadValue(); // fallback
        if (context.control.device is Pointer pointer)
            clickPosition = pointer.position.ReadValue();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = clickPosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        bool clickedTooltipOrNode = false;
        foreach (var r in results)
        {
            if (r.gameObject.GetComponent<SkillNodeUI>() != null || r.gameObject == gameObject)
            {
                clickedTooltipOrNode = true;
                break;
            }
        }

        if (!clickedTooltipOrNode)
            Hide();
    }
}
