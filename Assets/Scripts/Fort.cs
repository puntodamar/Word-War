using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fort : MonoBehaviour
{
    public Text healthText;
    public Turn fortOwner;
    public GameState typingMode;
    public int fortHealth   = 100;
    public int streak       = 0;
    public int attack       = 10;

    public void Attack(Fort fort)
    {
        fort.TakeDamage(attack + (attack/10 * streak));
    }
    public void TakeDamage(int damage)
    {
        StartCoroutine(SubstractHealth(fortHealth, damage));
        fortHealth = Mathf.Clamp(fortHealth -= damage, 0, fortHealth);
    }

    IEnumerator SubstractHealth(int health, int damage)
    {
        float oldHealth = (float)health;
        float newHealth = health - damage;

        while (oldHealth > newHealth)
        {
            oldHealth--;
            healthText.text = "Health : " + oldHealth + "%";
            yield return new WaitForSeconds(.05f);
        }
    }
}
