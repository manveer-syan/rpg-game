using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private Animator animator;
    private float moveSpeed = 0.2f;
    private Rigidbody2D rb;
    private Transform target;
    private Vector2 moveDirection;
    public int damage = 20;
    public float health = 3;
    public event Action OnDeath;

    private bool isFrozen = false; // To track if the enemy is frozen
    private bool isDead = false;   // To track if the enemy is dead

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy Health: " + health);  // Debugging health reduction
        if (health <= 0)
        {
            Defeated();
        }
    }

    // Getter and setter for health
    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                Defeated();
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player GameObject not found.");
        }

        // Subscribe to the player's OnDeath event
        PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnDeath += FreezeEnemy; // Freeze enemy when the player dies
        }
    }

    private void Update()
    {
        if (isFrozen || isDead) return; // Stop movement if frozen or dead

        if (target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction;
        }
    }

    private void FixedUpdate()
    {
        if (isFrozen || isDead) return; // Stop movement if frozen or dead

        if (target)
        {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

    private void FreezeEnemy()
    {
        // Check if rb is null before accessing it to prevent the MissingReferenceException
        if (rb == null) return;

        isFrozen = true; // Freeze enemy movement and actions
        rb.velocity = Vector2.zero; // Stop any movement
    }

    private void Defeated()
    {
        if (isDead) return; // Prevent double death logic execution

        isDead = true; // Mark the enemy as dead

        // Debug log to ensure the method is being triggered
        Debug.Log("Enemy defeated, triggering death animation.");

        // Start the death animation coroutine
        StartCoroutine(PlayDefeatedAnimation());
    }

    private IEnumerator PlayDefeatedAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Defeated"); // Ensure "Defeated" is the correct trigger in the Animator
            // Wait for the animation to finish (based on animation length)
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            Debug.LogWarning("Animator component not found.");
        }

        // After animation finishes, remove the enemy
        RemoveEnemy();
    }

    public void RemoveEnemy()
    {
        // Null check for Rigidbody2D before modifying
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Stop any movement before destroying
        }

        Destroy(gameObject);  // Destroy the enemy game object
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy otherEnemy = collision.GetComponent<Enemy>();  // Check if collided with another enemy
        if (otherEnemy != null)
        {
            Debug.Log("Enemy collided with another enemy!");
            otherEnemy.TakeDamage(damage); // Inflict damage on the other enemy
        }
    }

    void Die()
    {
        OnDeath?.Invoke();  // Notify that the enemy is dead
        Destroy(gameObject);
    }
}
