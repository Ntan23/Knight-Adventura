using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] float jumpPadForce;
    Animator animator;
    AudioManager audioManager;
    PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            playerMovement.availableJump = 0;
            audioManager.Play("Interact");
            animator.SetTrigger("Activate");
            playerRb.velocity = Vector2.up * jumpPadForce;
        }
    }
}
