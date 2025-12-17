using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;

public class AnimationTester : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Transform circleParent;
    private Animator animator;
    int questionCount = 2;
    public int variant = 0;
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
        
    }

void LateUpdate()
    {
        if (circleParent.position.x >= 760)
        {
            nodeTween.Kill();
            circleParent.position =  new Vector3(originalPosition.x - 140f, circleParent.position.y, 0);
            nodeTween = circleParent.DOMove(new Vector3(originalPosition.x, circleParent.position.y, 0), 0.8f).SetEase(node_tick_curve);
            nodeTween.Play();
        }
    }


    void PlayAnimation()
    {
        UpdateText();
        float newX = circleParent.position.x + 120f;
        nodeTween = circleParent.DOMove(new Vector3(newX, circleParent.position.y, 0), 0.8f).SetEase(node_tick_curve);
        nodeTween.Play();
    }

    public void UpdateCircle()
    {
        // save the last animation position

        // Debug.Log("animation done");
        // float newX = circleParent.position.x + 112.92f;
        // Vector3 newPos = new Vector3 (newX, circleParent.position.y, 0);
        // circleParent.position = newPos;
    }

    public void UpdateText()
    {
        text.text = "Placeholder question number " + questionCount;
        questionCount ++;
    }
}
