using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    InteractionSystem interact;

    private void Awake()
    {
        interact = FindObjectOfType<InteractionSystem>();
    }
    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger=true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(interact.isGrabbing)
            {
                interact.GrabAndDrop();
            }
            if(!collision.GetComponent<LivesCount>().isInvunerable)
            {
                collision.GetComponent<LivesCount>().LoseLive();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(interact.isGrabbing)
            {
                interact.GrabAndDrop();
            }
            if(!other.gameObject.GetComponent<LivesCount>().isInvunerable)
            {
                other.gameObject.GetComponent<LivesCount>().LoseLive();
            }
        }
    }
}
