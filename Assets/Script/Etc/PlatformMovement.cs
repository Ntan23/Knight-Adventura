using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public List<Transform> positions;
    public float speed = 2;
    public Transform platform;
    bool firstTime;
    [SerializeField] bool idleMode;
    float idleTimer;
    float idleDuration;
    int NextLocation;
    Transform goalpos;
    Animator playerAnimator;
    private int LocationChangerValue = 1;
    
    void Awake()
    {
        firstTime = true;
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        goalpos = positions[NextLocation];

        if(idleMode)
        {
            if(Vector2.Distance(platform.transform.position,goalpos.position) > 0)
            {
                MovingToNextPos();
            }
            else if(Vector2.Distance(platform.transform.position,goalpos.position) == 0)
            {
                ChangeDirectionWithIdle();
            }
        }
        else if(!idleMode)
        {
            if(Vector2.Distance(platform.transform.position,goalpos.position) > 0)
            {
                MovingToNextPos();
            }
            else if(Vector2.Distance(platform.transform.position,goalpos.position) == 0)
            {
                ChangeDirectionWithoutIdle();
            }
        }
    }

    void MovingToNextPos()
    {
        if(idleMode)
        {
            idleTimer = 0;
        }
        // Change platform position toward the goalPos
        platform.position = Vector2.MoveTowards(platform.position,goalpos.position,Time.deltaTime*speed);
    }

    void ChangeDirectionWithIdle()
    {
        if(firstTime)
        {
            idleDuration=Random.Range(1.0f,2.0f);
            firstTime=false;
        }

        idleTimer+=Time.deltaTime;
                    
        if(idleTimer > idleDuration)
        {
            idleDuration=Random.Range(1.0f,2.0f);
            // Check jika kita sudah di end of the line (ubah -1)
            // 2 Location(0,1) NextLocation == points.count(2)-1
            if(NextLocation == positions.Count-1)
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

    void ChangeDirectionWithoutIdle()
    {
        // Check jika kita sudah di end of the line (ubah -1)
        // 2 Location(0,1) NextLocation == points.count(2)-1
        if(NextLocation == positions.Count-1)
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerAnimator.SetBool("Jump",false);
        }
    } 
}
