using System.Collections;
using UnityEngine;

public class Iguana : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PlayAnimation("Hit",0.4f));
        }

    }
    IEnumerator PlayAnimation(string animationName, float transitionTime)
    {
        _animator.CrossFade(animationName, transitionTime);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length-0.25f);
        _animator.CrossFade("Idle", transitionTime);
    }
}
