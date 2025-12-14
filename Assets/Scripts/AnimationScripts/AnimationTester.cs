using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationTester : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Transform circleParent;
    private Animator animator;
    int questionCount = 2;
    public int variant = 0;
    InputAction resumeAction;
    void Start()
    {
        resumeAction = InputSystem.actions.FindAction("Next");
        animator = GetComponent<Animator>();

        resumeAction.performed += ctx => PlayAnimation(variant);
        
    }

    // Update is called once per frame

    void PlayAnimation(int variant)
    {
        Debug.Log("playing");
        if (variant == 0)
        {
            animator.Play("next-question");
        }
        else if (variant == 1)
        {
            animator.Play("v2-next-q");
        }
        else
        {
            animator.Play("v3next-question 1");
        }
    }

    public void UpdateCircle()
    {
        float newX = circleParent.position.x + 112.92f;
        Vector3 newPos = new Vector3 (newX, circleParent.position.y, 0);
        circleParent.position = newPos;
    }

    public void UpdateText()
    {
        text.text = "Placeholder question number " + questionCount;
        questionCount ++;
    }
}
