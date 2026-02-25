using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro; 

public class playerController : MonoBehaviour
{
    [Header("movement settings")]
    public float speed = 10f;
    public float jumpForce = 5f;
    public float rotationSpeed = 10f;

    [Header("health system")]
    public int health = 100;
    public TextMeshProUGUI healthText; 

    private Rigidbody rb;
    private bool onGround;
    private Vector2 inputMovement;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateUI(); 
    }

    public void OnMove(InputValue value)
    {
        if (isDead) return;
        inputMovement = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if (onGround && !isDead)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            onGround = false;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(10);
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            RotateTaggedObjects();
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        Vector3 moveDirection = new Vector3(inputMovement.x, 0, inputMovement.y);
        Vector3 finalVelocity = moveDirection * speed;
        rb.linearVelocity = new Vector3(finalVelocity.x, rb.linearVelocity.y, finalVelocity.z);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            enemyController enemyScript = hit.transform.GetComponent<enemyController>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(20);
            }
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        UpdateUI(); 

        if (health <= 0)
        {
            health = 0;
            isDead = true;
            Debug.Log("died");
            RestartScene();
        }
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "health: " + health;
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RotateTaggedObjects()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("cubes");
        foreach (GameObject obj in cubes)
        {
            obj.transform.Rotate(0, 45f, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            onGround = true;
        }
    }
}

