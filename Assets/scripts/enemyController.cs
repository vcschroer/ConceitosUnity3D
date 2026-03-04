using UnityEngine;
using System.Collections;

public class enemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 50;
    public float speed = 3f;
    public int damage = 10;

    [Header("Follow Settings")]
    private Transform playerTransform;
    public bool lockVerticalRotation = true; 

    private Renderer enemyRenderer;
    private Color originalColor;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            LookAtPlayer();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 targetPosition = playerTransform.position;

        if (lockVerticalRotation)
        {
            targetPosition.y = transform.position.y;
        }

        transform.LookAt(targetPosition);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(FlashRedEffect());

        if (health <= 0)
        {
            health = 0;
            Debug.Log("enemy died");
            Destroy(gameObject, 0.1f);
        }
    }

    IEnumerator FlashRedEffect()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            enemyRenderer.material.color = originalColor;
        }
    }
}