using UnityEngine;

public class PoopMoving : MonoBehaviour
{
    public Sprite landedSprite; //лежача какашки
    public float fallSpeed = 6f; // Швидкість падіння

    private bool isFalling = true;
    private float groundY;
    private SpriteRenderer spriteRenderer;
    private Collider2D poopCollider; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        poopCollider = GetComponent<Collider2D>();

        if (poopCollider != null)
        {
            poopCollider.enabled = false;
        }

        groundY = Random.Range(-4f, 1f);
    }

    void Update()
    {
        if (isFalling)
        {
            // Рух вниз
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            //Перевірка досягнення
            if (transform.position.y <= groundY)
            {
                Land();
            }
        }
    }

    void Land()
    {
        isFalling = false;

        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);

        //лежача какаха
        if (spriteRenderer != null && landedSprite != null)
        {
            spriteRenderer.sprite = landedSprite;
        }

        //врублення колайдеру і тригер пастка
        if (poopCollider != null)
        {
            poopCollider.enabled = true;
        }

        //Видалення через 5 секунд після приземлення
        Destroy(gameObject, 5f);
    }
}