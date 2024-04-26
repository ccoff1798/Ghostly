using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject shieldPrefab;
    public Transform firePoint;
    public Transform shieldPoint;
    public float shootForce = 10f;
    public bool isDefending = false;
    private GameObject currentShield;

    public LayerMask canPhaseThrough;
    public bool isPhasing = false;
    public float phaseDuration = 5.0f;

    public float pushBackForce = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !isDefending)
        {
            Shoot();
        }
        
        if (Input.GetButtonDown("Fire2") && !isDefending)
        {
            Defend();
        }
        
        if (Input.GetButtonUp("Fire2") && isDefending)
        {
            CancelDefend();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            PhaseThrough();
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * shootForce);
        Destroy(projectile, 3.0f);
    }

   void Defend()
{
    isDefending = true;
    currentShield = Instantiate(shieldPrefab, shieldPoint.position, shieldPoint.rotation);
    currentShield.transform.SetParent(transform, false);
    Rigidbody shieldRigidbody = currentShield.GetComponent<Rigidbody>();
    shieldRigidbody.isKinematic = true;

    // Reset local position and rotation if necessary
    currentShield.transform.localPosition = new Vector3(0, 0, 1); // Adjust if needed
    currentShield.transform.localRotation = Quaternion.identity; // Adjust if needed

    // Add a Collider and a new Script to the shield prefab to handle collision events
    ShieldCollision shieldCollision = currentShield.AddComponent<ShieldCollision>();
    shieldCollision.Initialize(pushBackForce, this.gameObject);
}


    void CancelDefend()
    {
        isDefending = false;
        if (currentShield != null)
        {
            Destroy(currentShield); // Destroy the shield object
            currentShield = null; // Clear the reference
        }
    }

    void PhaseThrough()
    {
        // Implement phase through ability here
        RaycastHit hit;
        canPhaseThrough = LayerMask.GetMask("canPhase");

        if (Physics.Raycast(transform.position, transform.forward, out hit, 3.0f,canPhaseThrough))
        {
            Debug.Log("Phasing through: " + hit.collider.gameObject.name);
            StartCoroutine(ToggleCollider(hit.collider));

        }

    }
     IEnumerator ToggleCollider(Collider collider)
    {
        collider.enabled = false; // Disable the collider
        yield return new WaitForSeconds(phaseDuration); // Wait for the phase duration
        collider.enabled = true; // Re-enable the collider
    }
}
