using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    public float damage = 50f;
    private bool isCoolingDown = false;
    private bool isReflected = false; // Flag to indicate the projectile has been reflected

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered with: " + other.tag); // Log what the projectile collides with

        if (isCoolingDown && !other.CompareTag("Enemy")) 
        {
            Debug.Log("Collision ignored due to cooldown");
            return;
        }

        if (other.CompareTag("Shield") && !isReflected)
        {
            isReflected = true; // Mark the projectile as reflected
            Debug.Log("Projectile reflected");
            ReverseDirection();
            StartCoroutine(CoolDownCollision());
        }
        else if (other.CompareTag("Player") && !isReflected)
        {
            Debug.Log("Damage to player");
            ProcessDamage(other.GetComponent<Health>());
        }
        else if (other.CompareTag("Enemy") )
        {
            Debug.Log("Damage to enemy");
            ProcessDamage(other.GetComponent<EnemyHealth>());
        }
    }

    private void ReverseDirection()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 180, 0));
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = -rb.velocity;
        }
        Debug.Log("Direction reversed, velocity: " + rb.velocity);
    }

    private void ProcessDamage(Component healthComponent)
    {
        Health health = healthComponent as Health;
        EnemyHealth enemyHealth = healthComponent as EnemyHealth;

        if (health != null)
        {
            Debug.Log("Applying damage to player health");
            health.TakeDamage(damage);
        }
        else if (enemyHealth != null)
        {
            Debug.Log("Applying damage to enemy health");
            enemyHealth.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Health component not found on the object");
        }

        Destroy(gameObject); // Destroy the projectile after it deals damage
    }

    IEnumerator CoolDownCollision()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(0.2f); // Cooldown period to prevent multiple triggers
        isCoolingDown = false;
        Debug.Log("Cooldown complete");
    }
}
