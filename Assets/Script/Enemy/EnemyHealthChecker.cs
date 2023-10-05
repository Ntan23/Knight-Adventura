using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealthChecker : MonoBehaviour
{
    [SerializeField] GameObject healthBar;
    bool isDead;
    // bool isPlay;
    Animator animator;
    AudioManager audioManager;
    EnemyHealth enemyHealth;
    EnemyChaseAI enemyChaseAI;
    FlexiblePatrol patrol;
    EnemyRangeAttack enemyRange;
    // public UnityEvent action;

    private void Awake() 
    {
        animator=GetComponent<Animator>();
        audioManager=AudioManager.instance;
        enemyHealth=GetComponent<EnemyHealth>();
        enemyChaseAI=GetComponent<EnemyChaseAI>();
        patrol = GetComponent<FlexiblePatrol>();
    }
    void Start()
    {
        // isPlay=false;
    }

    public bool isDie()
    {
        if(enemyHealth.currentHealth<=0)
        {
            // if(!isPlay)
            // {
            //     // audioManager.Play("DragonDie");
            //     isPlay=true;
            // }
            return true;
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isDie())
        {
            if(enemyChaseAI != null)
            {
                animator.ResetTrigger("Range");
                enemyChaseAI.agroRange = 0;
            }
            
            StartCoroutine(wait(1.15f));
        }
        else if(!isDie())
        {
            return;
        }
    }

    IEnumerator wait(float time)
    {
        if(patrol != null)
        {
            patrol.CanMove = false;
        }
        if(enemyChaseAI != null)
        {
            enemyChaseAI.canChase = false;
        }
        if(enemyRange != null)
        {
            enemyRange.enabled = false;
            animator.ResetTrigger("Range");
            animator.SetBool("Fly",false);
            animator.SetTrigger("Hurt");
        }
        this.gameObject.GetComponent<BoxCollider2D>().enabled=false;
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
        healthBar.SetActive(false);
    }
}
