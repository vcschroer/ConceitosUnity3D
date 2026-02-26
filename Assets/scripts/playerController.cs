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
    public Transform attackPoint; // Arraste aqui o objeto de referência para o ataque
    public float attackRange = 2f;

    [Header("Health System")]
    public int health = 100;
    public TextMeshProUGUI healthText;

    private Transform playerCamera; // Arraste sua câmera para cá no Inspector
    private Rigidbody rb;
    private bool onGround;
    private Vector2 inputMovement;
    private Vector2 inputLook;
    private float xRotation = 0f; // Para limitar o olhar para cima/baixo
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>().transform; // Tenta encontrar a câmera como filha do Player
        rb.freezeRotation = true;

        // Trava o mouse no centro da tela e o esconde
        Cursor.lockState = CursorLockMode.Locked;

        UpdateUI();
    }

    // Chamado pelo Input System (Mouse Delta)
    public void OnLook(InputValue value)
    {
        if (isDead) return;
        inputLook = value.Get<Vector2>();
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

        HandleRotation();

        // Atalhos de teste
        if (Keyboard.current.hKey.wasPressedThisFrame) TakeDamage(10);
        if (Keyboard.current.tKey.wasPressedThisFrame) RotateTaggedObjects();
        if (Keyboard.current.kKey.wasPressedThisFrame) Attack();
    }

    private void HandleRotation()
    {
        // Gira apenas no eixo Y (olhar para os lados)
        // Ignoramos completamente o inputLook.y para não olhar para cima/baixo
        float mouseX = inputLook.x * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // Movimento relativo à frente do personagem
        Vector3 moveDirection = (transform.right * inputMovement.x) + (transform.forward * inputMovement.y);
        Vector3 finalVelocity = moveDirection * speed;

        rb.linearVelocity = new Vector3(finalVelocity.x, rb.linearVelocity.y, finalVelocity.z);
    }

    private void Attack()
    {
        RaycastHit hit;
        // O ataque agora nasce no attackPoint e segue a direção que ele aponta
        if (Physics.Raycast(attackPoint.position, attackPoint.forward, out hit, attackRange))
        {

            enemyController enemyScript = hit.transform.GetComponent<enemyController>();

            if (enemyScript != null)

            {
                enemyScript.TakeDamage(20);
            }

        }

        // Debug visual para você ver o alcance no Editor
        Debug.DrawRay(attackPoint.position, attackPoint.forward * attackRange, Color.red, 0.5f);
    }

    // --- Sistema de Saúde e UI ---

    private void TakeDamage(int damage)
    {
        health -= damage;
        UpdateUI();

        if (health <= 0)
        {
            health = 0;
            isDead = true;
            Cursor.lockState = CursorLockMode.None;
            RestartScene();
        }
    }

    private void UpdateUI()
    {
        if (healthText != null) healthText.text = "Health: " + health;
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RotateTaggedObjects()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("cubes");
        foreach (GameObject obj in cubes) obj.transform.Rotate(0, 45f, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground")) onGround = true;
    }
}
