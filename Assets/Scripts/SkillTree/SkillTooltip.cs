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
    public Vector2 defaultOffset = new Vector2(150f, 50f);
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
            // Toggle off if same node
            Hide();
            return;
        }

        currentNode = node;
        titleText.text = node.data.courseCode;
        bodyText.text = node.data.courseDescription;

        PositionTooltip(node.RectTransform);

        gameObject.SetActive(true);
        isVisible = true;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
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
        // Get node position in canvas local space
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, target.position),
            canvas.worldCamera,
            out localPos
        );

        Vector2 pos = localPos + defaultOffset;

        // Get canvas size
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 tooltipSize = rect.sizeDelta;

        // Flip X if off right edge
        if (pos.x + tooltipSize.x + edgeMargin.x > canvasSize.x / 2f)
            pos.x = localPos.x - tooltipSize.x - defaultOffset.x;

        // Flip Y if off top edge
        if (pos.y + tooltipSize.y + edgeMargin.y > canvasSize.y / 2f)
            pos.y = localPos.y - tooltipSize.y - defaultOffset.y;

        rect.anchoredPosition = pos;
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
