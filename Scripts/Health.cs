using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour

{
    public float health = 100f;

    public GameObject player;
    Animator animator;
    GameOverScript gameOverScript;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
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
    Debug.Log("Dying and showing game over.");
    animator.SetTrigger("Die");
    GameOverScript.instance.ShowGameOver();
    player.GetComponent<PlayerMovement>().enabled = false;
    player.GetComponent<PlayerCombat>().enabled = false;
}

// This will be called by the animation event
public void DestroyGameObject()
{
    Destroy(gameObject);
}
}
