using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    private Animator animator;
    private PlayerHealth playerHealth;

    public SwordAttack swordAttack;
    public Vector2 movementInput;
    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private bool canMove = true;

    private bool isDead = false;
    private bool deathAnimationPlayed = false; // Flag to ensure death animation only plays once

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();

        if (animator == null)
        {
            Debug.LogError("Animator component is missing from this GameObject! Please add an Animator component.");
            return;
        }

        if (swordAttack == null)
        {
            swordAttack = GetComponent<SwordAttack>();
        }

        if (playerHealth != null)
        {
            playerHealth.OnDeath += TriggerDeathAnimation;
        }
    }

    void FixedUpdate()
    {
        if (canMove && !isDead)
        {
            bool isMoving = false;

            if (movementInput != Vector2.zero)
            {
                bool success = TryMove(movementInput);
                if (!success)
                {
                    success = TryMove(new Vector2(movementInput.x, 0));
                    if (!success)
                    {
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }

                isMoving = success;
            }

            if (animator != null)
            {
                animator.SetBool("IsMoving", isMoving);
            }
            else
            {
                Debug.LogWarning("Animator component is missing! Movement animation won't play.");
            }

            if (movementInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movementInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    void Update()
    {
        if (isDead)
        {
            Debug.Log("Player is dead. Waiting for Spacebar input.");

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log("New Input System detected Spacebar press.");
                RestartGame();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Legacy Input detected Spacebar press.");
                RestartGame();
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset);
            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
        }
        return false;
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire()
    {
        if (playerHealth != null && !playerHealth.isDead)
        {
            if (animator != null)
            {
                animator.SetTrigger("swordAttack");
            }
            else
            {
                Debug.LogWarning("Animator is missing, cannot trigger sword attack animation.");
            }
        }
    }

    public void SwordAttack()
    {
        LockMovement();
        if (spriteRenderer.flipX)
        {
            swordAttack.AttackLeft();
        }
        else
        {
            swordAttack.AttackRight();
        }
    }

    public void EndSwordAttack()
    {
        UnlockMovement();
        swordAttack.StopAttack();
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    private void TriggerDeathAnimation()
    {
        if (deathAnimationPlayed) return; // Prevent re-triggering

        isDead = true;
        deathAnimationPlayed = true; // Mark animation as played
        Debug.Log("TriggerDeathAnimation called. isDead set to true.");

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        else
        {
            Debug.LogWarning("Animator is missing! Cannot trigger death animation.");
        }

        LockMovement();
        Debug.Log("Death animation triggered.");
    }

    private void RestartGame()
    {
        if (isDead)
        {
            Debug.Log("Attempting to restart the game...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Game Restarted!");
        }
    }

    // Called via Animation Event at the end of the Death animation
    public void OnDeathAnimationComplete()
    {
        Debug.Log("Death animation completed.");
        // Additional logic, if needed, can go here
    }
}
