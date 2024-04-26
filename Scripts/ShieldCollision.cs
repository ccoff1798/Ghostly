using UnityEngine;
using System.Collections;

public class ShieldCollision : MonoBehaviour
{
    public float freezeDuration = 1f;
    public float pushBackForce = 10f;
    public GameObject owner; // To prevent the shield from interacting with the player itself

    public void Initialize(float force, GameObject ownerObject)
    {
        pushBackForce = force;
        owner = ownerObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != owner) // Ensure the shield doesn't push back its owner
        {
            if (collision.gameObject.CompareTag("Enemy")) // Check if the object is an enemy
            {
                Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                EnemyAi enemyAi = collision.gameObject.GetComponent<EnemyAi>();
                Transform enemyTransform = collision.gameObject.transform;
                if (enemyRigidbody != null)
                {
                    Debug.Log("Shield collided with enemy: " + collision.gameObject.name);
                    Vector3 pushDirection = collision.transform.position - transform.position;
                    pushDirection.y = 0; // Optionally keep the push direction horizontal
                    enemyRigidbody.AddForce(pushDirection.normalized * pushBackForce, ForceMode.Impulse);

                    enemyAi.Fall();

                    
                 
                }

            }
        }
    }

    // private IEnumerator FreezeEnemyPosition(Rigidbody enemyRigidbody, float freezeDuration)
    // {
    //     // Store the original constraints
    //     var originalConstraints = enemyRigidbody.constraints;

    //     // Freeze position
    //     enemyRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

    //     // Wait for the specified duration
    //     yield return new WaitForSeconds(freezeDuration);

    //     // Unfreeze position and return to original constraints
    //     enemyRigidbody.constraints = originalConstraints;

    //     Animator enemyAnimator = enemyRigidbody.GetComponent<Animator>();
    //     if (enemyAnimator != null)
    //     {
    //         enemyAnimator.SetBool("IsFalling", false); // Enemy is no longer falling
    //     }
    // }

}