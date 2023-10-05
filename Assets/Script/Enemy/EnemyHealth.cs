using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour,IData
{
    public HealthBar healthBar;
    public float healthMultiplier;
    public float currentHealth;
    public float fullHealth;
    float damage;
    public bool CanMove;
    Animator animator;
    AudioManager audioManager;
    bool isDamage;
    FlexiblePatrol patrol;
    EnemyChaseAI enemyChaseAI;

    private void Awake() 
    {
        animator = GetComponent<Animator>();
        patrol = GetComponent<FlexiblePatrol>();
        enemyChaseAI = GetComponent<EnemyChaseAI>();
        audioManager=AudioManager.instance;
        fullHealth=2*healthMultiplier;
        currentHealth=fullHealth;
        healthBar.SetSize(currentHealth/fullHealth);
        healthBar.SetColor(Color.green);
    }

    public void LoadData(GameData data)
    {
        this.damage=data.fireballDamage;
    }

    public void SaveData(GameData data)
    {

    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Fireball"))
        {   
            if(!isDamage)
            {
                isDamage = true;

                PlayerFireball playerFireball = other.GetComponent<PlayerFireball>();

                if(playerFireball != null)
                {
                    if(!playerFireball.isHitting)
                    {
                        StartCoroutine(playerFireball.hit(0.6f));
                    }
                }

                if(other.transform.localScale.x == 1 && this.gameObject.transform.localScale.x == 1)
                {
                    if(patrol != null)
                    {
                        patrol.ChangeDirectionWithoutIdle();
                        patrol.MoveToNextPos();
                    }
                }
                else if(other.transform.localScale.x == -1 && this.gameObject.transform.localScale.x == -1)
                {
                    if(patrol != null)
                    {
                        patrol.ChangeDirectionWithoutIdle();
                        patrol.MoveToNextPos();
                    }
                }

                if(enemyChaseAI != null)
                {
                    enemyChaseAI.agroRange*=2;
                }
                
                float randomDamage=Mathf.Round(Random.Range(0.1f,0.5f)*10.0f)*0.1f;
                currentHealth-=(damage+randomDamage);
                if(currentHealth/fullHealth <= 0.5f)
                {
                    healthBar.SetColor(Color.yellow);
                }
                if(currentHealth/fullHealth <= 0.25f)
                {
                    healthBar.SetColor(Color.red);
                }
                if(currentHealth<=0)
                {
                    healthBar.SetSize(0);
                }
                else
                {
                    healthBar.SetSize(currentHealth/fullHealth);
                }
                audioManager.Play("DragonHurt");
                animator.SetTrigger("Hurt");
                StartCoroutine(stun(0.6f));
                StartCoroutine(rageTime(4.0f));
            }
        }
    } 

    IEnumerator stun(float time)
    {
        if(patrol != null)
        {
            patrol.CanMove = false;
        }
        this.GetComponent<EnemyHealthChecker>().isDie();
        yield return new WaitForSeconds(time);
        if(patrol != null)
        {
            patrol.CanMove = true;
        }
        isDamage = false;
    }

    IEnumerator rageTime(float time)
    {
        yield return new WaitForSeconds(time);

        if(enemyChaseAI != null)
        {
            enemyChaseAI.agroRange/=2;
        }
    }
}
