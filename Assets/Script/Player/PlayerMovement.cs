using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    Rigidbody2D rb;
    SpriteRenderer SpriteRenderer;
    Animator animator;
    public Collider2D standCollider,crouchingCollider;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform overHeadCheckCollider;
    [SerializeField] Transform wallCheckerCollider;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask objectLayer;
    [SerializeField] LayerMask wallLayer;
    const float groundCheckerRadius=0.1f;
    const float overHeadCheckRadius=0.1f;
    const float wallCheckerRadius = 0.1f;
    float horizontalValue;
    public float speed;
    public float jumpForce;
    public float slidefactor = 0.2f;
    public int totalJumps;
    [SerializeField] float coyoteTime;
    public int availableJump;
    public float runSpeedModifier;
    public float crouchSpeedModifier;
    bool isRun;
    bool isGrounded;
    public bool isCrouch;
    bool multipleJump;
    bool coyoteJump;
    public bool isAirborne;
    bool isCrouching;
    bool isSpawn;
    bool isSliding;
    public bool isEntering;
    public int count;
    float startTime;
    //Reference
    InteractionSystem interaction;
    InventorySystem inventory;
    GameManager gameManager;
    Shooting playerRangeAttack;
    PlayerMeleeAttack playerMeleeAttack;
    Goal goal;
    [SerializeField] GameObject jumpLandEffect;
    [SerializeField] GameObject portal;
    [SerializeField] GameObject endPortal;
    Animator jumpLandEffectAnimator;
    AudioManager audioManager;
    [SerializeField] GameObject winWindow;
    [SerializeField] GameObject winButton;
    bool firstTime;
    DrownInWater drownInWater;
    LivesCount livesCount;
    PauseMenu pause;
    #endregion
    
    private void Awake() 
    {
        rb=GetComponent<Rigidbody2D>();
        SpriteRenderer=GetComponent<SpriteRenderer>();
        animator=GetComponent<Animator>();
        interaction=GetComponent<InteractionSystem>();
        inventory=GetComponent<InventorySystem>();
        goal = FindObjectOfType<Goal>();
        playerRangeAttack=GetComponent<Shooting>();
        playerMeleeAttack=GetComponent<PlayerMeleeAttack>();
        pause = FindObjectOfType<PauseMenu>();
        jumpLandEffectAnimator=jumpLandEffect.GetComponent<Animator>();
        isSpawn=true;
        availableJump=totalJumps;
        this.gameObject.transform.position = portal.transform.position;
        drownInWater=FindObjectOfType<DrownInWater>();
        livesCount = GetComponent<LivesCount>();
    }
    
    // Start is called before the first frame update
    void Start()
    {   
        gameManager=GameManager.instance;
        audioManager=AudioManager.instance;
        firstTime = true;
        startTime=Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(isSpawn)
        {
            spawnFromPortal();
        }
        if(isEntering)
        {
            InToPortal();
        }

        if(firstTime)
        {
            if(!isGrounded)
            {
                return;
            }
            else if(isGrounded)
            {
                firstTime = false;
            }
        }

        if(pause.isPaused)
        {
            return;
        }
        //Input 
        horizontalValue=Input.GetAxisRaw("Horizontal");
        //If Press L-Shift Enable Run (isRun true)
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRun=true;
        }
        //If Release L-Shift Disable Run (isRun false)
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRun=false;
        }
        //If Press Jump Button Call Jump Function/Method 
        if(Input.GetButtonDown("Jump") && !goal.isComplete)
        {
            Jump();
        }  
        //If Press Crouch Button Enable Crouch (isCrouch true)
        if(Input.GetButtonDown("Crouch") && !playerRangeAttack.isShooting)
        {
            isCrouch=true;
        } 
        //If Release Crouch Button Disable Crouch (isCrouch false)
        else if(Input.GetButtonUp("Crouch") && !playerRangeAttack.isShooting)
        {
            isCrouch=false;
        }

        //Jump Land Effect Position
        if(this.gameObject.transform.localScale.x==-1)
        {
            jumpLandEffect.transform.localScale=new Vector3(-0.2f,0.2f,1.0f);
        }
        else if(this.gameObject.transform.localScale.x==1)
        {
            jumpLandEffect.transform.localScale=new Vector3(0.2f,0.2f,1.0f);
        }
        
        if(!CanMove())
        {
            horizontalValue=0;
        }

        //Set Jump or Fall Animation According To Y-axis Velocity
        animator.SetFloat("Yvelocity",rb.velocity.y);

        // check touch wall and then slide
        WallChecker();
    }

    void FixedUpdate()
    {
        //Check Are We On The Ground
        GroundCheck();
        //Move Our Character
        Move(horizontalValue,isCrouch);
    }

    void GroundCheck()
    {
        bool wasGrounded=isGrounded;
        isGrounded=false;
        Collider2D collide = Physics2D.OverlapCircle(groundCheckCollider.position,groundCheckerRadius,groundLayer);
        Collider2D objectCollide = Physics2D.OverlapCircle(groundCheckCollider.position,groundCheckerRadius,objectLayer);

        if(collide != null)
        {
            isGrounded=true;
            if(!wasGrounded)
            {
                isAirborne=false;
                saveEffectPosition();
                availableJump=totalJumps;
                multipleJump=false;
                jumpLandEffectAnimator.SetTrigger("Land");
            }

            //Check if any of the colliders is movingplatform
            //Parent it to this transform
            if(collide.CompareTag("MovingPlatform"))
            {
                transform.parent = collide.transform;
                jumpLandEffectAnimator.ResetTrigger("Land");
            }   
        }  
        else if(objectCollide != null)
        {
            if(objectCollide.gameObject.name == "Crates")
            {
                isGrounded=true;
                if(!wasGrounded)
                {
                    isAirborne=false;
                    saveEffectPosition();
                    availableJump=totalJumps;
                    multipleJump=false;
                    jumpLandEffectAnimator.SetTrigger("Land");
                }
            }
        }  
        //Check The groundCheckCollider Is Colliding With Other Objects That Are In Ground Layer
        if(collide == null && objectCollide == null)
        {
            isAirborne = true;

            //Un-parent the transform
            transform.parent = null;

            if(wasGrounded)
            {
                StartCoroutine(CoyoteJumpDelay(coyoteTime));
            }
        }

        //If Player isShooting Stop The Jump & Fall Animation (Chnage To Cast Animation)
        if(playerRangeAttack.isShooting)
        {
            animator.SetBool("Jump",false);
        }
        else
        {
            //As Long As We Are On The Ground (Grounded) The Animator "Jump" Value Set To False
            animator.SetBool("Jump",!isGrounded);
        }
    }

    void WallChecker()
    {
        Collider2D wallCollide = Physics2D.OverlapCircle(wallCheckerCollider.position,wallCheckerRadius,wallLayer);
        // If touching a wall, moving towards the wall
        // and falling also not Grounded
        // slide on the wall
        if(wallCollide != null && Mathf.Abs(horizontalValue) > 0 && rb.velocity.y < 0 && !isGrounded)
        {
            if(!isSliding)
            {
                availableJump = totalJumps;
                multipleJump = true;
            }

            //Debug.Log("Slide");
            Vector2 v = rb.velocity;
            v.y = -slidefactor;
            rb.velocity = v;
            isSliding = true;

            if(Input.GetButtonDown("Jump"))
            {
                audioManager.Play("Jump");
                isAirborne=true;
                saveEffectPosition();
                availableJump--;
                rb.velocity = Vector2.up * jumpForce;
                animator.SetBool("Jump",true);
                jumpLandEffectAnimator.SetTrigger("Jump");
            }
        }
        else if(rb.velocity.y > 0)
        {
            isSliding = false;
            // Debug.Log("No Slide");
        }
    }

    #region Jump
    void Jump()
    {
        //If Character On The Ground And Not Is Attack And Press Jump Button (Usually Space) Jump
        if(isGrounded && !playerMeleeAttack.isAttack && !playerRangeAttack.isShooting)
        {
            if(!isCrouching)
            {   
                isAirborne=true;
                saveEffectPosition();       
                audioManager.Play("Jump");
                multipleJump=true;
                availableJump--;
                rb.velocity=Vector2.up*jumpForce;
                animator.SetBool("Jump",true);
                jumpLandEffectAnimator.SetTrigger("Jump");
            }
        }
        else if(!isGrounded && !playerMeleeAttack.isAttack && !playerRangeAttack.isShooting)
        {
            if(coyoteJump)
            {
                audioManager.Play("Jump");
                isAirborne=true;
                saveEffectPosition();
                multipleJump=true;
                availableJump--;
                rb.velocity=Vector2.up*jumpForce;
                animator.SetBool("Jump",true);
                jumpLandEffectAnimator.SetTrigger("Jump");
            }
            //If We Still Have Available Jump And Multiple Jump true We Can Jump Again
            if(availableJump > 0 && multipleJump)
            {
                audioManager.Play("Jump");
                isAirborne=true;
                saveEffectPosition();
                availableJump--;
                rb.velocity=Vector2.up*jumpForce;
                animator.SetBool("Jump",true);
                jumpLandEffectAnimator.SetTrigger("Jump");
            }
        }
    }

    IEnumerator CoyoteJumpDelay(float time)
    {
        coyoteJump=true;
        yield return new WaitForSeconds(time);
        coyoteJump=false;
    }
    #endregion

    void Move(float dir,bool crouch)
    {
        float xValue,yValue;
        xValue=dir*speed*100*Time.fixedDeltaTime;
        yValue=rb.velocity.y;
        //Crouch
        Collider2D collide = Physics2D.OverlapCircle(overHeadCheckCollider.position,overHeadCheckRadius,groundLayer);
        //If We Are Crouching Or Disable Crouching Check Overhead For Collision with "Ground" Layer Object
        //If Yes Stay Crouch If No Uncrouch
        if(!crouch)
        {
            if(collide != null)
            {
                crouch=true;
                isCrouching=true;
                animator.ResetTrigger("Hurt");
            }
            else if(collide == null)
            {
                isCrouching = false;
            }
        }
        
        //If Player In The Air Cannot Crouch
        if(isAirborne)
        {
            crouch = false;
        }
        
        //If We Press Crouch Button Disable The Standing Collider And Animate Crouching
        //If Realeased Return The Original Speed , Enable The Standing Collider And Disable Crouch Animation
        //Set Crouch Animation Using A Crouch Check (Boolean)
        animator.SetBool("Crouch",crouch);
        standCollider.enabled=!crouch;
        crouchingCollider.enabled=crouch;

        //Move and Run
        //If isRun true , Multiple The X-Value With Run Speed Modifier
        if(isRun)
        {
            xValue=xValue*runSpeedModifier;
        }
        //If isCrouch true , Multiply The X-Value With Crouch Speed Modifier , Player Can't Run And Player Can't Jump
        //Reduce The Speed Using Crouch Speed Modifier
        if(crouch)
        {
            animator.ResetTrigger("MeleeAttack");
            animator.ResetTrigger("Cast");
            isRun = false;
            xValue=xValue*crouchSpeedModifier;
            yValue=0;
        }
        //Create A vEctor2 For The Velocity
        Vector2 targetVelocity = new Vector2(xValue,yValue);
        //Set The Velocity
        rb.velocity=targetVelocity;

        if(dir<0)
        {
            //Face Left
            transform.localScale = new Vector3(-1,1,1);
        }
        if(dir>0)
        {
            //Face Right
            transform.localScale = new Vector3(1,1,1);
        }

        //Set Idle , Walk or Run Animation According To X-Axis Velocity
        float xVelocity = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Xvelocity",xVelocity);
    }

    bool CanMove()
    {
        bool canMove=true;

        if(isSpawn)
        {
            canMove = false;
        }
        if(livesCount.isHurt)
        {
            canMove = false;
        }
        if(interaction.isExamining)
        {
            canMove = false;
        }
        if(inventory.isOpen)
        {
            canMove = false;
        }
        if(gameManager.isDead)
        {
            canMove = false;
        }
        if(playerRangeAttack.isShooting)
        {
            canMove = false;
        }
        if(playerMeleeAttack.isAttack)
        {
            canMove = false;
        }
        if(goal != null)
        {
            if(goal.isComplete)
            {
                rb.velocity = new Vector2(0,0);
                canMove=false;
            }
        }
        if(isEntering)
        {
            canMove = false;
        }
        if(drownInWater != null)
        {
            if(drownInWater.inTheWater)
            {
                canMove = false;
            }
        }
        
        return canMove;
    }

    //Draw Gizmos On Selected Object
    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawSphere(groundCheckCollider.position,groundCheckerRadius);
        Gizmos.color=Color.white;
        Gizmos.DrawSphere(overHeadCheckCollider.position,overHeadCheckRadius);
    }

    public void saveEffectPosition()
    {
        //if isAirborne save jump effect position if not airbone save land effect position
        if(isAirborne)
        {
            if(this.gameObject.transform.localScale.x == 1)
            {
                jumpLandEffect.transform.position=this.gameObject.transform.position-new Vector3(0.3f,1.0f,0);
            }
            else if(this.gameObject.transform.localScale.x == -1)
            {
                jumpLandEffect.transform.position=this.gameObject.transform.position+new Vector3(0.25f,-1.0f,0);
            }
        }
        else if(!isAirborne)
        {
            jumpLandEffect.transform.position=this.gameObject.transform.position+new Vector3(0.05f,-1.05f,0);
        }
    }

    #region Portal
    void spawnFromPortal()
    {     
        isAirborne = true;
        rb.gravityScale=0;
        if(Time.time - startTime <= 1.5f)
        {
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position,portal.transform.position+new Vector3(0,0.25f,0),2.0f*Time.deltaTime);
            this.gameObject.transform.localScale=new Vector3(0,0,1);
        }
        else
        {
            if(this.gameObject.transform.localScale.x < 1)
            {
                //Set the scale from small to normal
                this.gameObject.transform.localScale += new Vector3(0.5f,0.5f,0) * Time.deltaTime;
                
                //move position according to the scale
                if(this.gameObject.transform.localScale.x < 0.3f)
                {
                    this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position,portal.transform.position+new Vector3(0,0.25f,0),2.0f*Time.deltaTime);
                }
                else if(this.gameObject.transform.localScale.x > 0.3f && this.gameObject.transform.localScale.x <= 0.5f)
                {
                    this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position,portal.transform.position+new Vector3(0,0.25f,0),2.0f*Time.deltaTime);
                }
                else if(this.gameObject.transform.localScale.x > 0.5f)
                {
                    this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position,portal.transform.position+new Vector3(0,0.25f,0),2.0f*Time.deltaTime);
                    // this.gameObject.transform.position += new Vector3(0,0.5f,0) * Time.deltaTime;
                }

                //if scale is more than 1 make it to 1
                if(this.gameObject.transform.localScale.x > 1)
                {
                    this.gameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
                }
            }
        
            if(this.gameObject.transform.localScale.x == 1)
            {
                rb.gravityScale=1;
                isSpawn=false;
            }
        }
    }

    void InToPortal()
    {
        animator.ResetTrigger("Cast");
        animator.ResetTrigger("MeleeAttack");
        winWindow.SetActive(false);
        winButton.SetActive(false);
        rb.gravityScale=0;
        animator.SetBool("Win",false); 
        if(Mathf.Abs(this.gameObject.transform.localScale.x) > 0)
        {
            //Set the scale from normal to tiny
            if(this.gameObject.transform.localScale.x > 0)
            {
                this.gameObject.transform.localScale -= new Vector3(0.4f,0.4f,0) * Time.deltaTime;
            }
            else if(this.gameObject.transform.localScale.x < 0)
            {
                this.gameObject.transform.localScale += new Vector3(0.4f,-0.4f,0) * Time.deltaTime;
            }
            
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position,endPortal.transform.position,0.5f);
        
            //move position according to the scale
            if(Mathf.Abs(this.gameObject.transform.localScale.x) <= 0.3f)
            {
                 this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position,endPortal.transform.position,0.5f);
            }
            if(Mathf.Abs(this.gameObject.transform.localScale.x) > 0.3f)
            {
                this.gameObject.transform.position += new Vector3(0,0.05f,0) * Time.deltaTime;
            }
            if(Mathf.Abs(this.gameObject.transform.localScale.x)< 0.2f)
            {
                this.gameObject.transform.localScale = new Vector3(0,0,1);
            }
        }
        
        if(this.gameObject.transform.localScale.x == 0)
        {
            if(goal == null)
            {
                goal = FindObjectOfType<Goal>();
            }

            goal.LoadNextLevel();
            isEntering=false;
        }
    }
    public void enterPortal()
    {    
        isEntering=true;
    } 
    #endregion
}
