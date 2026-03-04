using UnityEngine;

public class allyController : MonoBehaviour
{
    [Header("Ally Settings")]
    public float moveSpeed = 5f;
    public float stoppingDistance = 3f; 
    public float rotationSpeed = 10f;

    private Transform playerTransform;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null) rb.freezeRotation = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        LookAtPlayer();

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > stoppingDistance)
        {
            FollowPlayer();
        }
        else
        {
            StopMoving();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 targetPosition = playerTransform.position;
        targetPosition.y = transform.position.y; 

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FollowPlayer()
    {
        Vector3 moveDir = transform.forward * moveSpeed;

        if (rb != null)
        {
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
        }
        else
        {
            transform.position += moveDir * Time.deltaTime;
        }
    }
    private void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
}
