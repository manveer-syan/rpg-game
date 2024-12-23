using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Collider2D swordCollider; // Assign this in the Inspector
    public float damage = 1f;        // Damage dealt by the sword
    private Vector2 rightAttackOffset;

    private void Start()
    {
        if (swordCollider == null)
        {
            Debug.LogError("Sword Collider is not assigned!");
        }
        rightAttackOffset = transform.localPosition;
    }

    public void AttackRight()
    {
        swordCollider.enabled = true; // Enable collider for right attack
        Debug.Log("Attack Right activated");
        Debug.Log($"Sword localPosition during right attack: {rightAttackOffset}");
    }

    public void AttackLeft()
    {
        swordCollider.enabled = true; // Enable collider for left attack
        Debug.Log("Attack Left activated");
        transform.localPosition = new Vector3(-rightAttackOffset.x, rightAttackOffset.y);
    }

    public void StopAttack()
    {
        swordCollider.enabled = false; // Disable collider after attack
        Debug.Log("Attack stopped");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Triggered by: {other.name} with tag {other.tag}");
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.Health -= damage;
                Debug.Log($"Damaged {other.name}, new health: {enemy.Health}");
            }
            else
            {
                Debug.LogWarning("Enemy script not found on collided object.");
            }
        }
    }

}
