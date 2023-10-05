using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseAI : MonoBehaviour
{
    Animator animator;
    [SerializeField] bool isPatrol;

    [Header("Chasing")]
    [SerializeField] bool chaseWithPatrol;
    [SerializeField] Transform player;
    GameManager gameManager;
    [SerializeField] float speed = 2.0f;
    Vector3 startPosition;
    public float agroRange;

    //Patrol patrol;
    [Header("Ground Check")]
    [SerializeField] Transform groundCheckCollider;
    const float groundCheckerRadius=0.1f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isGrounded;
    public bool canChase = true;
    FlexiblePatrol flexiblePatrol;

    private void Awake() 
    {
        startPosition=GetComponent<Transform>().position;
        animator=GetComponent<Animator>();
        flexiblePatrol=this.gameObject.GetComponent<FlexiblePatrol>();
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        
        if(chaseWithPatrol)
        {
            isPatrol = true;
        }
        if(flexiblePatrol!=null)
        {
            isPatrol = true;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(!canChase)
        {
            return;
        }
        
        float distance=Vector2.Distance(transform.position,player.position);

        if(!chaseWithPatrol)
        {
            if(flexiblePatrol!=null)
            {
                flexiblePatrol.enabled = false;
            }

            if(distance < agroRange)
            {
                //chase player
                chase();
            }
            else if(distance >= agroRange)
            {
                //Back to Start Position
                stopChase();
            }
        }
        else if(chaseWithPatrol)
        {
            if(flexiblePatrol != null && isPatrol)
            {
                flexiblePatrol.enabled = true;
            }
            else if(flexiblePatrol != null && !isPatrol)
            {
                flexiblePatrol.enabled = false;
            }

            if(isPatrol)
            {
                if(distance < agroRange)
                {
                    chase();
                    isPatrol = false;
                }
            }
            else if(!isPatrol)
            {
                if(distance < agroRange)
                {
                    chase();
                    isPatrol = false;
                }
                else if(distance >= agroRange)
                {
                    stopChase();
                    if(transform.position == startPosition)
                    {
                        isPatrol = true;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if(!isPatrol)
        {
            GroundCheck();
        }
    }

    void chase()
    {
        if(transform.position.x > player.position.x)
        {
            transform.position = Vector2.MoveTowards(transform.position,player.position,speed*Time.deltaTime);
            transform.localScale=new Vector2(-1,1);
            animator.SetBool("Move",true);
        }
        else if(transform.position.x < player.position.x)
        {
            transform.position = Vector2.MoveTowards(transform.position,player.position,speed*Time.deltaTime);
            transform.localScale=new Vector2(1,1);
            animator.SetBool("Move",true);
        }
    }

    void stopChase()
    {
        if(Vector2.Distance(transform.position,startPosition) <= 0)
        {
            animator.SetBool("Move",false);
        }
        else if(Vector2.Distance(transform.position,startPosition) > 0)
        {
            if(transform.position.x > startPosition.x)
            {
                transform.position = Vector2.MoveTowards(transform.position,startPosition,speed*Time.deltaTime);
                transform.localScale = new Vector2(-1,1);
                animator.SetBool("Move",true);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position,startPosition,speed*Time.deltaTime);
                transform.localScale = new Vector2(1,1);
                animator.SetBool("Move",true);
            }
        }
    }

    void GroundCheck()
    {
        isGrounded=false;
        Collider2D colliders =Physics2D.OverlapCircle(groundCheckCollider.position,groundCheckerRadius,groundLayer);

        //Check The groundCheckCollider Is Colliding With Other Objects That Are In "Ground" Layer
        if(colliders != null)
        {
            isGrounded = true;  
        }
        else if(colliders == null)
        {
           isGrounded = false;
        }

        if(isGrounded)
        {
            animator.SetBool("Fly",false);
        }
        else if(!isGrounded)
        {
            animator.SetBool("Fly",true);
        }
    }
}
