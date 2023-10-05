using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexiblePatrol : MonoBehaviour
{
    [Header("Waypoints")]
    // Waypoints
    public List<Transform> points;
    int NextLocation = 0;
    private int LocationChangerValue = 1;
    Transform goalpos;
    Animator animator;

    [Header("Movement Parameter")]
    [SerializeField] float speed = 2.0f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheckCollider;
    const float groundCheckerRadius=0.1f;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;

    [Header("Idle Mode")]
    float idleDuration;
    float idleTimer;
    bool firstTime;
    [SerializeField] bool idleMode;

    public bool CanMove;
    
    private void Awake() 
    {
        animator = this.gameObject.GetComponent<Animator>();
        firstTime = true;
        CanMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!CanMove)
        {
            return;
        }

        goalpos = points[NextLocation];

        if(idleMode)
        {
            if(Vector2.Distance(transform.position,goalpos.position) > 0)
            {
                MoveToNextPos();
            }
            else if(Vector2.Distance(transform.position,goalpos.position) == 0)
            {
                ChangeDirectionWithIdle();
            }
        }
        else if(!idleMode)
        {
            if(Vector2.Distance(transform.position,goalpos.position) > 0)
            {
                MoveToNextPos();
            }
            else if(Vector2.Distance(transform.position,goalpos.position) == 0)
            {
                ChangeDirectionWithoutIdle();
            }
        }
    }

    private void FixedUpdate() 
    {
        GroundCheck();
    }

    private void OnDisable() 
    {
        animator.SetBool("Move",false);
    }

    void GroundCheck()
    {
        isGrounded=false;
        Collider2D colliders = Physics2D.OverlapCircle(groundCheckCollider.position,groundCheckerRadius,groundLayer);

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
    
    public void MoveToNextPos()
    {
        if(idleMode)
        {
            idleTimer = 0;
        }
        
        animator.SetBool("Move",true);
        // Flip enemy trans sesuai pos
        if(goalpos.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else if(goalpos.transform.position.x <= transform.position.x)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        // move enemy sesuai dengan position
        transform.position = Vector2.MoveTowards(transform.position, goalpos.position,speed*Time.deltaTime);
    }

    void ChangeDirectionWithIdle()
    {
        animator.SetBool("Move",false);
        
        if(firstTime)
        {
            idleDuration=Random.Range(1.0f,3.0f);
            firstTime=false;
        }

        idleTimer+=Time.deltaTime;
            
        if(idleTimer > idleDuration)
        {
            idleDuration=Random.Range(1.0f,3.0f);
            // Check jika kita sudah di end of the line (ubah -1)
            // 2 Location(0,1) NextLocation == points.count(2)-1
            if(NextLocation == points.Count-1)
            {
                LocationChangerValue = -1;
            }

            // check jika kita sudah di awal of line (ubah +1)
            if(NextLocation == 0)
            {
                LocationChangerValue = 1;
            }
            // apply perubahan NextLocation
            NextLocation += LocationChangerValue;
        }
    }

    public void ChangeDirectionWithoutIdle()
    {
        animator.SetBool("Move",false);
        
        // Check jika kita sudah di end of the line (ubah -1)
        // 2 Location(0,1) NextLocation == points.count(2)-1
        if(NextLocation == points.Count-1)
        {
            LocationChangerValue = -1;
        }

        // check jika kita sudah di awal of line (ubah +1)
        if(NextLocation == 0)
        {
            LocationChangerValue = 1;
        }
        // apply perubahan NextLocation
        NextLocation += LocationChangerValue;
    }

    private void Reset()
    {
        Initialize();
    }

    public void Initialize()
    {
        // buat box collider trigger
        GetComponent<BoxCollider2D>().isTrigger = true;

        // buat objek root
        GameObject root = new GameObject(name+" (Patrol)");

        // Reset posisi root menjadi posisi Enemy
        root.transform.position = transform.position;

        // Set object enemy sebagai child root
        transform.SetParent(root.transform);

        // GameObject Waypoints
        GameObject waypoints = new GameObject("Waypoints");
        // Reset position waypoints ke root

        // Buat object waypoints jadi child dari root
        waypoints.transform.SetParent(root.transform);
        waypoints.transform.position = root.transform.position;
        // buat 2 point untuk arah geraknya
        // buat point ini sebagai child dari waypoints
        GameObject pos1 = new GameObject("Position1");
        pos1.transform.SetParent(waypoints.transform);
        pos1.transform.position = root.transform.position;
        GameObject pos2 = new GameObject("Position2");
        pos2.transform.SetParent(waypoints.transform);
        pos2.transform.position = root.transform.position;

        // Init list pint dan add points
        points = new List<Transform>();
        points.Add(pos1.transform);
        points.Add(pos2.transform);
    }
}
