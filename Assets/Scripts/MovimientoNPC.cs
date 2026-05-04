using UnityEngine;
using System.Collections;

public class MovimientoNPC : MonoBehaviour
{
    public float speed = 2f;
    public int maxHealth = 2;

    public float checkDistance = 0.5f;
    public LayerMask wallLayer;

    public bool spriteInvertido = true;

    public PlayerScore playerScore;

    [Header("Sonido")]
    public AudioClip sonidoGolpe;
    [Range(0f, 1f)] public float volumen = 1f;

    [Header("Knockback jugador")]
    public float knockbackX = 6f;
    public float knockbackY = 4f;

    [Header("Movimiento físico del jugador")]
    public float knockbackDuration = 0.12f;

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
        float dir = movingRight ? 1f : -1f;

        transform.position += Vector3.right * dir * speed * Time.deltaTime;

        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.1f;
        Vector2 rayDir = movingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(origin, rayDir, checkDistance, wallLayer);

        Debug.DrawRay(origin, rayDir * checkDistance, Color.red);

        if (hit.collider != null)
            Flip();
    }

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        float dir = movingRight ? 1f : -1f;

        if (spriteInvertido) dir *= -1f;

        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (playerScore != null)
            playerScore.SumarPuntos(500);

        if (sonidoGolpe != null)
            AudioSource.PlayClipAtPoint(sonidoGolpe, transform.position, volumen);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb == null) return;

        // Golpe desde arriba
        if (collision.contacts[0].normal.y < -0.5f)
        {
            TakeDamage(1);
            return;
        }

        // Daño al jugador
        player.TakeDamage(1);

        float dirX = collision.transform.position.x > transform.position.x ? 1f : -1f;

        StartCoroutine(Knockback(rb, dirX));
    }

    IEnumerator Knockback(Rigidbody2D rb, float dirX)
    {
        if (rb == null) yield break;

        Vector2 force = new Vector2(dirX * knockbackX, knockbackY);

        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        // amortiguación suave para que no sea un rebote infinito
        rb.linearVelocity *= 0.5f;
    }
}