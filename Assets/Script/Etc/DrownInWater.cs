using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrownInWater : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rb;
    public bool inTheWater = false;
    [Header("Use For Drown")]
    [SerializeField] float massMultiplier;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody2D>();
        inTheWater = false;
    }

    private void Update()
    {
        if(!inTheWater)
        {
            return;
        }

        if(inTheWater)
        {
            rb.mass+=massMultiplier*Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inTheWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
             inTheWater = false;
            rb.mass = 1;
        }
    }
}
