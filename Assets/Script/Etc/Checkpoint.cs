using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameManager gameManager;
    Animator animator;
    Item item;
    bool saved=false;
    [SerializeField] ParticleSystem particle;
    // [HideInInspector] public bool isActive;

    private void Awake()
    {
        //gameManager=FindObjectOfType<GameManager>();
        animator=GetComponent<Animator>();
        item=GetComponent<Item>();
        particle.Pause();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    
    public void CheckPoint()
    {
        particle.Play();
        if(!saved)
        {
            animator.SetBool("Activate",true);
            gameManager.savedPosition = this.transform.position + new Vector3(0,0.5f,0);
            saved=true;
        }
        // isActive=true;
    }
}
