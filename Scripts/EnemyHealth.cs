using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour

{
    public float health = 100f;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Health: " + health);
        animator.SetTrigger("TakeDamage");
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, 2f); // Waits 2 seconds before destroying to allow animation to play
    }
}
