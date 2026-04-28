using UnityEngine;

public class MovimientoNPC : MonoBehaviour
{
    public float speed = 2f;
    public int maxHealth = 2;

    public float checkDistance = 0.5f;
    public LayerMask wallLayer;

    public bool spriteInvertido = true;

    public PlayerScore playerScore;

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

        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, checkDistance, wallLayer);

        Debug.DrawRay(origin, rayDirection * checkDistance, Color.red);

        if (hit.collider != null)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;

        float dir = movingRight ? 1 : -1;
        if (spriteInvertido) dir *= -1;

        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
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
        // DAR PUNTOS AL JUGADOR
        if (playerScore != null)
        {
            playerScore.SumarPuntos(500);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

            if (collision.contacts[0].normal.y < -0.5f)
            {
                TakeDamage(1);
            }
            else
            {
                player.TakeDamage(1);
            }
        }
    }
}