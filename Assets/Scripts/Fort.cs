using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fort : MonoBehaviour
{
    public Sprite[] warriorSprites;
    public Text healthText;
    public Image healthSlider;
    public Turn fortOwner;
    public GameState typingMode;
    public int fortHealth   = 100;
    public int streak       = 0;
    public int attack       = 10;
    public float shakeOffset = .2f;

    public SpriteRenderer[] warriorStandingPosition;

    private Transform originalPosition;

    private void Start()
    {
        healthSlider.color = Color.green;
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        originalPosition = transform;

        InitWarrior();
    }

    void InitWarrior()
    {
        for (int i = 0; i < 3; i++)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            int randomIndex = Random.Range(0, warriorSprites.Length);
            Debug.Log(randomIndex);
            warriorStandingPosition[i].sprite = warriorSprites[randomIndex];
        }
    }

    public void Attack(Fort fort)
    {
        fort.TakeDamage(attack + (attack/10 * streak));
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(SubstractHealth(fortHealth, damage));
        StartCoroutine(FrontWarriorDead());
        fortHealth = Mathf.Clamp(fortHealth -= damage, 0, fortHealth);
    }

    IEnumerator SubstractHealth(int health, int damage)
    {
        float oldHealth = (float)health;
        float newHealth = health - damage;

        while (oldHealth > newHealth)
        {
            oldHealth--;
            healthText.text = oldHealth + "%";
            healthSlider.fillAmount = oldHealth / 100;
            healthSlider.color = Color.Lerp(Color.red, Color.green, oldHealth / 100);
            yield return new WaitForSeconds(.05f);
        }

        
        //StartCoroutine(Shake());
    }

    IEnumerator FrontWarriorDead()
    {

        Color tmp = warriorStandingPosition[0].GetComponent<SpriteRenderer>().color;
        while(tmp.a > 0)
        {
            Color.Lerp(tmp, Color.clear, .5f);
            warriorStandingPosition[0].GetComponent<SpriteRenderer>().color = tmp;
            yield return null;
        }
    }

    void MoveSoldier()
    {

    }

    //IEnumerator Shake()
    //{
    //    transform.DOShakePosition(.5f);
    //}
}
