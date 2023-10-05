using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeAttack : MonoBehaviour
{
    #region Variables
    [Header("Attack Parameters")]
    [SerializeField] float attackCooldown;
    [SerializeField] float range;
    [Header("Range Attack")]
    [SerializeField] Transform firepoint;
    [SerializeField] GameObject fireball;
    [Header("Collider Parameters")]
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] float colliderRange;
    [Header("Player Layer")]
    [SerializeField] LayerMask layer;
    float cooldownTimer;
    Animator animator;

    //References
    LivesCount playerHealth;
    EnemyChaseAI enemyChaseAI;
    ObjectPoolManager objectPoolManager;
    FlexiblePatrol flexiblePatrol;
    AudioManager audioManager;
    #endregion

    private void Awake() 
    {
        audioManager = AudioManager.instance;
        animator=GetComponent<Animator>();
        enemyChaseAI=GetComponent<EnemyChaseAI>();
        flexiblePatrol=GetComponent<FlexiblePatrol>();
    }

    private void Start() 
    {
        objectPoolManager=ObjectPoolManager.instance;
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
                //Attack
                cooldownTimer=0;
                animator.SetTrigger("Range");
            }
        }
        else if(!PlayerInSight())
        {
            flexiblePatrol.enabled = true;
            animator.ResetTrigger("Range");
        }
    }

    public void rangeAttack()
    {
        GameObject enemyFireball = objectPoolManager.GetPooledObject("EnemyFireball");

        if(enemyFireball!=null)
        {
            audioManager.Play("FireballWhoosh");
            enemyFireball.transform.position=firepoint.position;
            enemyFireball.transform.localScale=this.gameObject.transform.localScale;
            enemyFireball.SetActive(true);
        }
    }

    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center+transform.right*range*transform.localScale.x*colliderRange,new Vector3(boxCollider.bounds.size.x*range,boxCollider.bounds.size.y,boxCollider.bounds.size.z),0,Vector2.left,0,layer);

        return hit.collider!=null;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center+transform.right*range*transform.localScale.x*colliderRange,new Vector3(boxCollider.bounds.size.x*range,boxCollider.bounds.size.y,boxCollider.bounds.size.z));
    }
}
