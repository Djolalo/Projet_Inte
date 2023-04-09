using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public bool isInvincible = false;

    public SpriteRenderer graphics; 
    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if(!isInvincible)
        {
          currentHealth -= damage;
          healthBar.SetHealth(currentHealth);  
          isInvincible = true;
          StartCoroutine(InvincibilityFlash());
          StartCoroutine(HandleInvicibilityDelay());
        }
        
    }

    public void FallDamage(int damage){
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public IEnumerator InvincibilityFlash()
    {
        while(isInvincible)
        {
            graphics.color = new Color(1f,1f,1f,0f);
            yield return new WaitForSeconds(0.1f);
            graphics.color = new Color(1f,1f,1f,1f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator HandleInvicibilityDelay()
    {
        yield return new WaitForSeconds(2f);
        isInvincible = false;
    }
}