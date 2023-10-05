using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] float attackCooldown;
    [SerializeField] float range;
    [Header("Collider Parameters")]
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] float colliderRange;
    [Header("Player Layer")]
    [SerializeField] LayerMask layer;
    float cooldownTimer = Mathf.Infinity;
    Animator animator;

    //References
    LivesCount playerHealth;
    InteractionSystem playerInteractionSystem;
    //Patrol enemyPatrol;
    EnemyChaseAI enemyChaseAI;
    FlexiblePatrol flexiblePatrol;
    AudioManager audioManager;

    private void Awake() 
    {
        animator=GetComponent<Animator>();
        enemyChaseAI=GetComponent<EnemyChaseAI>();
        flexiblePatrol=GetComponent<FlexiblePatrol>();
        audioManager = AudioManager.instance;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(enemyChaseAI != null)
        {
            enemyChaseAI.enabled = !PlayerInSight();
        }

        cooldownTimer+=Time.deltaTime;

        if(PlayerInSight())
        {
            flexiblePatrol.enabled = false;
            if(cooldownTimer>=attackCooldown)
            {
                //Melee Attack
                cooldownTimer=0;
                audioManager.Play("MeleeAttack");
                animator.SetTrigger("Attack");
            }
        }
        else if(!PlayerInSight())
        {
            flexiblePatrol.enabled = true;
            animator.ResetTrigger("Attack");
        }
    }

    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center+transform.right*range*transform.localScale.x*colliderRange,new Vector3(boxCollider.bounds.size.x*range,boxCollider.bounds.size.y,boxCollider.bounds.size.z),0,Vector2.left,0,layer);

        if(hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<LivesCount>();
            playerInteractionSystem = hit.transform.GetComponent<InteractionSystem>();
        }  

        return hit.collider!=null;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center+transform.right*range*transform.localScale.x*colliderRange,new Vector3(boxCollider.bounds.size.x*range,boxCollider.bounds.size.y,boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if(PlayerInSight())
        {
            if(playerInteractionSystem.isGrabbing)
            {
                //Drop the grabbed object
                playerInteractionSystem.GrabAndDrop();
            }
            if(!playerHealth.isInvunerable)
            {
                //Damage Player
                playerHealth.LoseLive();
            }
        }
    }
}
