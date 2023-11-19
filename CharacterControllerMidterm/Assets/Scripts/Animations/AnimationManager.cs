using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implementing a callback system to play the right animations. Is this a better way to approach what I was doing previously?
public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void OnHit(bool isHit)
    {
        animator.SetBool("isHitbyObstacle", isHit);
        animator.SetBool("normalState", !isHit);
    }
}
