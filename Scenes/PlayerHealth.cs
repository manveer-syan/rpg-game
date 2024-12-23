using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    private EnemySpawner enemySpawner;
    public bool isDead = false;

    public event System.Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found in the scene.");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Ignore damage if already dead

        currentHealth -= damage; // Reduce health
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
{
    if (isDead) return; // Prevent multiple death triggers

    isDead = true;

    if (enemySpawner != null)
    {
        enemySpawner.StopSpawning();
    }

    Debug.Log("Player has died!");

    // Invoke the OnDeath event and log it
    OnDeath?.Invoke();
    Debug.Log("OnDeath event invoked.");

    // Optional: Disable player movement and interactions
    GetComponent<PlayerController>()?.LockMovement();
}

}
