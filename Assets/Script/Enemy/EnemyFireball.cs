using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireball : MonoBehaviour
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
    bool canPlay = true;

    ObjectPoolManager objectPoolManager;
    AudioManager audioManager;

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
        if(other.CompareTag("Fireball"))
        {
            StartCoroutine(hit(0.6f));
        }

        if(other.CompareTag("Player"))
        {
            StartCoroutine(hit(0.6f));
            if(other.gameObject.GetComponent<InteractionSystem>().isGrabbing)
            {
                other.gameObject.GetComponent<InteractionSystem>().GrabAndDrop();
            }
            if(!other.gameObject.GetComponent<LivesCount>().isInvunerable)
            {
                other.gameObject.GetComponent<LivesCount>().LoseLive();
            }
        }

        if(other.CompareTag("Traps"))
        {
            StartCoroutine(hit(0.6f));
        }

        if(other.CompareTag("Object"))
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
            if(objectCollide.gameObject.name == "Crates")
            {
                GetComponent<CapsuleCollider2D>().isTrigger = false;
                StartCoroutine(hit(0.6f));
            }

            if(objectCollide.name == "Chest")
            {
                StartCoroutine(hit(0.6f));
            }
        }

        if(collide != null)
        {   
            if(collide.CompareTag("Door") || collide.CompareTag("MovingPlatform"))
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

    IEnumerator hit(float time)
    {
        if(canPlay)
        {
            audioManager.Play("Explosion");
            canPlay = false;
        }
        animator.SetBool("Hit",true);
        if(this.transform.localScale.x==-1)
        {
            this.transform.position+=otherOffset;
        }
        else if(this.transform.localScale.x==1)
        {
            this.transform.position+=offset;
        }
        move=false;
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        move=true;
        animator.SetBool("Hit",false);
        canPlay = true;
        GetComponent<CapsuleCollider2D>().isTrigger = true;
    }
}
