using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro; 

public class playerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 0.5f;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 2f;
    public int meleeDamage = 20;

    [Header("Shoot Settings")]
    public GameObject bulletPrefab;     
    public GameObject specialBulletPrefab;
    public float fireRate = 0.25f;
    private float nextFireTime = 0f;

    [Header("Charge Settings")]
    public float chargeTimeThreshold = 1.5f; 
    private float buttonPressStartTime;
    private bool isCharging = false;

    [Header("Health System")]
    public int health = 100;
    public TextMeshProUGUI healthText;

    private Rigidbody rb;
    private bool onGround;
    private Vector2 inputMovement;
    private Vector2 inputLook;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        UpdateUI();
    }

    public void OnLook(InputValue value) => inputLook = value.Get<Vector2>();
    public void OnMove(InputValue value) => inputMovement = value.Get<Vector2>();

    public void OnShoot(InputValue value)
    {
        if (isDead) return;

        if (value.isPressed)
        {
            buttonPressStartTime = Time.time;
            isCharging = true;
            Debug.Log("carregando ataque");
        }
        else
        {
            float holdDuration = Time.time - buttonPressStartTime;
            isCharging = false;

            if (holdDuration >= chargeTimeThreshold)
            {
                ShootSpecial();
            }
            else
            {
                ShootNormal();
            }
        }
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
        HandleRotation();

        if (Keyboard.current.kKey.wasPressedThisFrame) AttackMelee();
        if (Keyboard.current.hKey.wasPressedThisFrame) TakeDamage(10);

        if (isCharging && (Time.time - buttonPressStartTime) >= chargeTimeThreshold)
        {
        }
    }

    private void HandleRotation()
    {
        float mouseX = inputLook.x * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        Vector3 moveDirection = (transform.right * inputMovement.x) + (transform.forward * inputMovement.y);
        rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
    }

    private void ShootNormal()
    {
        if (Time.time >= nextFireTime && bulletPrefab != null && attackPoint != null)
        {
            Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);
            nextFireTime = Time.time + fireRate;
            Debug.Log("tiro normal");
        }
    }

    private void ShootSpecial()
    {
        if (specialBulletPrefab != null && attackPoint != null)
        {
            Instantiate(specialBulletPrefab, attackPoint.position, attackPoint.rotation);
            Debug.Log("ataque especial lançado");
        }
    }

    private void AttackMelee()
    {
        if (attackPoint == null) return;
        RaycastHit hit;
        if (Physics.Raycast(attackPoint.position, attackPoint.forward, out hit, attackRange))
        {
            if (hit.transform.TryGetComponent<enemyController>(out var enemy))
            {
                enemy.TakeDamage(meleeDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateUI();
        if (health <= 0) { isDead = true; Cursor.lockState = CursorLockMode.None; RestartScene(); }
    }

    private void UpdateUI() { if (healthText != null) healthText.text = "Health: " + health; }
    private void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground")) onGround = true;
    }
}