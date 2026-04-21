using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int health;

    private bool isInvincible = false;
    public float invincibleTime = 1f;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // 🔹 NUEVO

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        StartCoroutine(Invincibility()); // 🔹 NUEVO
    }

    public void Heal(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
}