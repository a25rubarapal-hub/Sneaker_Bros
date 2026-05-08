using UnityEngine;

public class slimequenosecae : MonoBehaviour
{
    public float speed = 2f;
    public int maxHealth = 2;

    [Header("Detección de pared")]
    public float checkDistance = 0.5f;
    public LayerMask wallLayer;

    [Header("Detección de suelo")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    public bool spriteInvertido = true;

    public PlayerScore playerScore;

    [Header("Sonido")]
    public AudioClip sonidoGolpe;
    [Range(0f, 1f)] public float volumen = 1f;

    private int currentHealth;
    private bool movingRight = true;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float direction = movingRight ? 1f : -1f;

        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Raycast pared
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.1f;

        RaycastHit2D wallHit = Physics2D.Raycast(
            origin,
            rayDirection,
            checkDistance,
            wallLayer
        );

        Debug.DrawRay(origin, rayDirection * checkDistance, Color.red);

        // Raycast suelo
        RaycastHit2D groundHit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        Debug.DrawRay(
            groundCheck.position,
            Vector2.down * groundCheckDistance,
            Color.blue
        );

        // Girar si hay pared o no hay suelo
        if (wallHit.collider != null || groundHit.collider == null)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;

        float dir = movingRight ? 1 : -1;

        if (spriteInvertido)
            dir *= -1;

        scale.x = Mathf.Abs(scale.x) * dir;

        transform.localScale = scale;

        // Cambiar posición del detector de suelo
        Vector3 groundPos = groundCheck.localPosition;
        groundPos.x *= -1;
        groundCheck.localPosition = groundPos;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (playerScore != null)
        {
            playerScore.SumarPuntos(500);
        }

        if (sonidoGolpe != null)
        {
            AudioSource.PlayClipAtPoint(
                sonidoGolpe,
                transform.position,
                volumen
            );
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth player =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (collision.contacts[0].normal.y < -0.5f)
            {
                TakeDamage(1);
            }
            else
            {
                if (player != null)
                {
                    player.TakeDamage(1);
                }
            }
        }
    }
}