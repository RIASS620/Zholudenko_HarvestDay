using UnityEngine;

public class BirdController : MonoBehaviour
{
    public GameObject poopPrefab;
    public float speed = 4f;
    public float dropInterval = 3f;

    private float timer;
    private bool movingRight = true;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        timer = dropInterval;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Рух вліво вправо
        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);

            // вправо
            if (spriteRenderer != null) spriteRenderer.flipX = false;

            if (transform.position.x > 8f) movingRight = false;
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);

            // вліво з розворотм
            if (spriteRenderer != null) spriteRenderer.flipX = true;

            if (transform.position.x < -8f) movingRight = true;
        }

        // Таймер для какашки
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            DropPoop();
            timer = dropInterval;
        }
    }

    void DropPoop()
    {
        //поява какашки де пролетіла пташка
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 0.5f, 0f);

        // Створення какашки
        GameObject poop = Instantiate(poopPrefab, spawnPos, Quaternion.identity);
    }
}