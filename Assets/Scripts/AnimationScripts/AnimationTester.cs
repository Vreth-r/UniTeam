using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class AnimationTester : MonoBehaviour
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

        resumeAction.performed += ctx => PlayAnimation();
        originalPosition = circleParent.transform.position;

        //ResetAnimation();
        lineTransform.DOMove(new Vector3(0, lineTransform.position.y, 0), 0.5f).SetEase(Ease.OutSine);


        
    }

    void LateUpdate()
    {
        if (circleParent.position.x >= 760)
        {
            ResetAnimation();
        }
    }


    void PlayAnimation()
    {
        UpdateText();
        float newX = circleParent.position.x + 120f;
        nodeTween = circleParent.DOMove(new Vector3(newX, circleParent.position.y, 0), 0.8f).SetEase(node_tick_curve);
        nodeTween.Play();
    }

    void ResetAnimation()
    {
        nodeTween.Kill();
        circleParent.position =  new Vector3(originalPosition.x - 140f, circleParent.position.y, 0);
        nodeTween = circleParent.DOMove(new Vector3(originalPosition.x, circleParent.position.y, 0), 0.5f).SetEase(node_tick_curve);
        nodeTween.Play();
        //UpdateLineColour();
    }

    void UpdateLineColour()
    {
        lineImage.DOColor(redColor, 1f);
    }

    public void UpdateText()
    {
        text.text = "Placeholder question number " + questionCount;
        questionCount ++;
    }
}
