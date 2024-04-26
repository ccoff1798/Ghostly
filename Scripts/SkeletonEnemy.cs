using UnityEngine;

public class SkeletonEnemy : MonoBehaviour
{
    private Animator animator;
    public float speed = 3;
    
    public GameObject playerObject;

    private Rigidbody rb;
    private Vector3 wanderTarget = Vector3.zero;
 // Reference to the Rigidbody component

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    void FixedUpdate()
{
    bool isPlayerVisible = LookForPlayer();
    if (isPlayerVisible)
    {
        RotateTowards(playerObject.transform.position);
        Move();
    }
    else
    {
        animator.SetBool("IsMoving", false);
        Wander(); // Call wander when the player is not visible
    }
}


    

    void Move()
    {
        animator.SetBool("IsMoving", true);
        Vector3 direction = (playerObject.transform.position - transform.position).normalized;
        float speed = 5f;
        // Apply a force to move the enemy
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    public bool LookForPlayer()
    {
        Vector3 directionToPlayer = playerObject.transform.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);
        float distance = directionToPlayer.magnitude;

        if (distance < 20f && angle < 60f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, 20f))
            {
                if (hit.collider.gameObject == playerObject)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void RotateTowards(Vector3 targetPosition)
{
    Vector3 direction = (targetPosition - transform.position).normalized;
    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Flatten the rotation on the y-axis
    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
}
void Wander()
{
    float wanderRadius = 10f;
    float wanderDistance = 5f;
    float wanderJitter = 1f;

    wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);
    wanderTarget.Normalize();
    wanderTarget *= wanderRadius;

    Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
    Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal);

    RotateTowards(targetWorld + transform.position); // Rotate towards the wander target
    MoveToPosition(targetWorld + transform.position); // Move towards the wander target
}

void MoveToPosition(Vector3 targetPosition)
{
    Vector3 direction = (targetPosition - transform.position).normalized;
    rb.MovePosition(rb.position + direction * (speed * 0.5f) * Time.fixedDeltaTime); // Move at a slower speed when wandering
}

}
