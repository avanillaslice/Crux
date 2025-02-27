using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;
    private float animationDuration;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Get the duration of the animation
            animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
            // Destroy the game object after the animation finishes
            Destroy(gameObject, animationDuration);
        }
        else
        {
            Debug.LogWarning("Animator component not found on the explosion prefab.");
            // If no animator is found, destroy the game object immediately
            Destroy(gameObject);
        }
    }
}
