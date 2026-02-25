using UnityEngine;
using System.Collections;

public class enemyController : MonoBehaviour
{
    [Header("enemy stats")]
    public int health = 50;
    public float speed = 3f;
    public int damage = 10;

    private Renderer enemyRenderer;
    private Color originalColor;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;
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
        else
        {
            Debug.Log("enemy health: " + health + " | damage received: " + amount);
        }
    }

    IEnumerator FlashRedEffect()
    {
        enemyRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        enemyRenderer.material.color = originalColor;
    }
}