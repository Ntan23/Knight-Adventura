using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(!other.GetComponent<LivesCount>().isInvunerable)
            {
                other.GetComponent<LivesCount>().LoseLive();
            }
        }
    }
}
