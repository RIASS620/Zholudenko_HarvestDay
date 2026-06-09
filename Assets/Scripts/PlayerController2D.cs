using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    [Header("Обмеження руху")]
    public float minX = -8.2f;
    public float maxX = 8.2f;
    public float minY = -4.5f;
    public float maxY = 1.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (animator != null)
        {
            bool isMovingNow = (moveInput.x != 0 || moveInput.y != 0);
            animator.SetBool("isMoving", isMovingNow);
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveY", moveInput.y);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * speed;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            GameManager2D.Instance.AddScore(1);
            //звук звичайної моркви
            GameManager2D.Instance.PlaySound(GameManager2D.Instance.carrotSound);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Finish"))
        {
            GameManager2D.Instance.AddScore(5);
            //звук золотої моркви
            GameManager2D.Instance.PlaySound(GameManager2D.Instance.goldenCarrotSound);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Poop"))
        {
            GameManager2D.Instance.HitPoop();
            //звук какашки
            GameManager2D.Instance.PlaySound(GameManager2D.Instance.poopSound);
            Destroy(collision.gameObject);
        }
    }
}