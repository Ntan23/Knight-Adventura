using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour,IData
{
    #region Variables
    [Header("Attack Parameters")]
    float damage;
    [SerializeField] float attackCooldown;
    [SerializeField] float range;
    [Header("Collider Parameters")]
    [SerializeField] Collider2D playerCollider;
    [SerializeField] float colliderRange;
    [Header("Enemy Layer")]
    [SerializeField] LayerMask layer;

    float cooldownTimer = Mathf.Infinity;
    public bool isAttack;
    bool jumpAttack;
    Animator animator;
    GameData gameData;
    EnemyHealth enemyHealth;
    PlayerMovement playerMovement;
    AudioManager audioManager;
    GameObject enemy;
    FlexiblePatrol patrol;
    ModeChanger modeChanger;
    Goal goal;
    InteractionSystem interact;
    PauseMenu pause;
    GameManager gameManager;
    #endregion

    private void Awake()
    {
        modeChanger = GetComponent<ModeChanger>();
        animator=GetComponent<Animator>();
        playerMovement=GetComponent<PlayerMovement>();
        goal = FindObjectOfType<Goal>();
        interact = GetComponent<InteractionSystem>();
        pause = FindObjectOfType<PauseMenu>();
        audioManager=AudioManager.instance;
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public void LoadData(GameData data)
    {
        this.damage=data.swordDamage;
    }

    public void SaveData(GameData data)
    {
        
    }
    
    // Update is called once per frame
    private void Update() 
    {
        if(goal.isComplete)
        {
            return;
        }

        if(pause.isPaused)
        {
            return;
        }

        if(gameManager.isDead)
        {
            return;
        }

        attack();
    }

    void attack()
    {
        cooldownTimer+=Time.deltaTime;
        
        if(!playerMovement.isAirborne && Input.GetKeyDown(KeyCode.W))
        {
            if(cooldownTimer>=attackCooldown && modeChanger.canAttack && !interact.isGrabbing)
            {
                //Melee Attack
                cooldownTimer=0; 
                animator.SetTrigger("MeleeAttack");
            }
        }
    }

    void playSound()
    {
        audioManager.Play("SwordSwing");
    }

    void attackChecker(int value)
    {
        if(value==1)
        {
            isAttack=true;
        }
        else if(value==0)
        {
            isAttack=false;
        }
    }

    bool EnemyInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center+transform.right*range*transform.localScale.x*colliderRange,new Vector3(playerCollider.bounds.size.x*range,playerCollider.bounds.size.y,playerCollider.bounds.size.z),0,Vector2.left,0,layer); 

        if(hit.collider!=null)
        {
            enemy = hit.transform.gameObject;
            patrol = hit.transform.GetComponent<FlexiblePatrol>();
            enemyHealth=hit.transform.GetComponent<EnemyHealth>();
        }  

        return hit.collider!=null;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerCollider.bounds.center+transform.right*range*transform.localScale.x*colliderRange,new Vector3(playerCollider.bounds.size.x*range,playerCollider.bounds.size.y,playerCollider.bounds.size.z));
    }

    private void DamageEnemy()
    {
        if(EnemyInSight())
        {
            if(enemy.transform.localScale.x == 1 && this.gameObject.transform.localScale.x == 1)
            {
                if(patrol != null)
                {
                    patrol.ChangeDirectionWithoutIdle();
                    patrol.MoveToNextPos();
                }
            }
            else if(enemy.transform.localScale.x == -1 && this.gameObject.transform.localScale.x == -1)
            {
                if(patrol != null)
                {
                    patrol.ChangeDirectionWithoutIdle();
                    patrol.MoveToNextPos();
                }
            }
            //Damage Enemy
            float randomDamage=Mathf.Round(Random.Range(0.1f,0.5f)*10.0f)*0.1f;
            enemyHealth.currentHealth-=damage+randomDamage;

            //Health Bar Color
            if(enemyHealth.currentHealth/enemyHealth.fullHealth<=0.5f)
            {
                enemyHealth.healthBar.SetColor(Color.yellow);
            }
            if(enemyHealth.currentHealth/enemyHealth.fullHealth<=0.25f)
            {
                enemyHealth.healthBar.SetColor(Color.red);
            }

            //Health Bar Size
            if(enemyHealth.currentHealth<=0)
            {
                enemyHealth.healthBar.SetSize(0);
            }
            else
            {
                enemyHealth.healthBar.SetSize(enemyHealth.currentHealth/enemyHealth.fullHealth);
            }
            audioManager.Play("DragonHurt");
            enemyHealth.GetComponent<Animator>().SetTrigger("Hurt");
            StartCoroutine(stun(0.5f));
        }
    }

    IEnumerator stun(float time)
    {
        if(enemyHealth.gameObject.GetComponentInParent<FlexiblePatrol>()!=null)
        {
           enemyHealth.gameObject.GetComponentInParent<FlexiblePatrol>().CanMove = false;
        }
        if(enemyHealth.gameObject.GetComponent<EnemyChaseAI>() != null)
        {
            enemyHealth.gameObject.GetComponent<EnemyChaseAI>().canChase = false;
        }
        yield return new WaitForSeconds(time);
        if(enemyHealth.gameObject.GetComponent<FlexiblePatrol>()!= null)
        {
            enemyHealth.gameObject.GetComponent<FlexiblePatrol>().CanMove = true;
        }
        if(enemyHealth.gameObject.GetComponent<EnemyChaseAI>() != null)
        {
            enemyHealth.gameObject.GetComponent<EnemyChaseAI>().canChase = true;
        }
    }
}
