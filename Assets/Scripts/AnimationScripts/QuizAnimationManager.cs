using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class QuizAnimationManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Transform circleParent;
    public Image lineImage;
    public Transform lineTransform;
    private Animator animator;

    private Color redColor = new Color(228f, 0, 114f);
    int questionCount = 2;
    Vector3 originalPosition;
    InputAction resumeAction;
    [SerializeField] private AnimationCurve node_tick_curve;
    Tween nodeTween;
    Tween resetTween;
    float elapsedTime = 0;
    void Start()
    {
        resumeAction = InputSystem.actions.FindAction("Next");
        DOTween.Init();
        // animator = GetComponent<Animator>();

        //resumeAction.performed += ctx => PlayAnimation();
        originalPosition = circleParent.transform.position;

        //ResetAnimation();
        circleParent.position =  new Vector3(originalPosition.x - 400f, circleParent.position.y, 0);
        circleParent.DOMove(new Vector3(originalPosition.x, circleParent.position.y, 0), 0.5f).SetEase(node_tick_curve);
        lineTransform.DOMove(new Vector3(-1820, lineTransform.position.y, 0), 0.5f).SetEase(Ease.OutSine).From();
        //text.DOFade(0f, 0.5f).From();
    }

    void LateUpdate()
    {
        if (circleParent.position.x >= 1910)
        {
            ResetAnimation();
        }
    }


    public void PlayAnimation()
    {
        float newX = circleParent.position.x + 380f;
        nodeTween = circleParent.DOMove(new Vector3(newX, circleParent.position.y, 0), 0.8f).SetEase(node_tick_curve);
        nodeTween.Play();
        // Sequence TextSequence = DOTween.Sequence();
        // TextSequence.Append(text.DOFade(0f, 0.25f));
        // TextSequence.Append(text.DOFade(1f, 0.25f));
        //StartCoroutine(UpdateText());
    }

    void ResetAnimation()
    {
        nodeTween.Kill();
        circleParent.position =  new Vector3(originalPosition.x - 400f, circleParent.position.y, 0);
        nodeTween = circleParent.DOMove(new Vector3(originalPosition.x, circleParent.position.y, 0), 0.5f).SetEase(node_tick_curve);
        nodeTween.Play();
        //UpdateLineColour();
    }

    void UpdateLineColour()
    {
        lineImage.DOColor(redColor, 1f);
    }

    IEnumerator UpdateText()
    {
        yield return new WaitForSeconds(0.2f);
        text.text = "Placeholder question number " + questionCount;
        questionCount ++;
        elapsedTime -= 0.1f;
    }
}
