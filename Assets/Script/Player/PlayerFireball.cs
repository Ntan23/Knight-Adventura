using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireball : MonoBehaviour
{
    public float speed;
    Animator animator;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 otherOffset;
    [HideInInspector] public bool move;

    [Header("Collision Detector")]
    [SerializeField] Transform collisionPoint;
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] LayerMask objectLayer;
    float collisionRadius = 0.1f;
    ObjectPoolManager objectPoolManager;
    AudioManager audioManager;
    bool canPlaySound = true;
    public bool isHitting;

    private void Awake() 
    {
        audioManager = AudioManager.instance;
        animator=GetComponent<Animator>();
    }

    void Start()
    {
        move=true;
        objectPoolManager=ObjectPoolManager.instance;
    }

    private void Update()
    {
        if(!move)
        {
            return;
        }

        Move();
        CollisionChecker();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("EnemyFireball"))
        {
            StartCoroutine(hit(0.6f));
        }

        if(other.CompareTag("Object"))
        {
            StartCoroutine(hit(0.6f));
        }

        if(other.CompareTag("Traps"))
        {
            StartCoroutine(hit(0.6f));
        }
    }

    void CollisionChecker()
    {
        Collider2D collide = Physics2D.OverlapCircle(collisionPoint.position,collisionRadius,collisionLayer);
        Collider2D objectCollide = Physics2D.OverlapCircle(collisionPoint.position,collisionRadius,objectLayer);

        if(objectCollide != null)
        {   
            if(objectCollide.name == "Chest")
            {
                StartCoroutine(hit(0.6f));
            }

            if(objectCollide.name == "Crates")
            {
                GetComponent<CapsuleCollider2D>().isTrigger = false;
                StartCoroutine(hit(0.6f));
            }
        }

        if(collide != null)
        {  
            if(collide.CompareTag("Door") || collide.CompareTag("MovingPlatform"))
            {
                StartCoroutine(hit(0.6f));
            }
        
            if(collide.CompareTag("Enemy")) 
            {
                StartCoroutine(hit(0.6f));
            }
        }

        if(collide == null || objectCollide == null)
        {
            return;
        }
    }

    private void Move()
    {
        transform.Translate(transform.right*transform.localScale.x*Time.deltaTime*speed);
    }
    
    public IEnumerator hit(float time)
    {
        isHitting = true;
        if(canPlaySound)
        {
            audioManager.Play("Explosion");
            canPlaySound = false;
        }
        animator.SetBool("Hit",true);
        move=false;
        if(this.transform.localScale.x==-1 )
        {
            this.transform.position+=otherOffset;
        }
        if(this.transform.localScale.x==1 )
        {
            this.transform.position+=offset;
        }
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        move=true;
        animator.SetBool("Hit",false);
        canPlaySound = true;
        isHitting = false;
        GetComponent<CapsuleCollider2D>().isTrigger = true;
    }
}
