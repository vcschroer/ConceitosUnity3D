using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 20;
    public float lifeTime = 3f; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyController enemy = other.GetComponent<enemyController>();
        Debug.Log("bala colidiu");
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            Destroy(gameObject);
            return; 
        }

        if (other.CompareTag("ground") || other.CompareTag("Untagged"))
        {
            Destroy(gameObject);
        }
    }
}