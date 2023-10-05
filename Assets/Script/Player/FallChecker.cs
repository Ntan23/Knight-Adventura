using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallChecker : MonoBehaviour
{
    GameObject player;
    GameManager gameManager;
    [SerializeField] Vector3 initialPosition;
    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start() 
    {
        gameManager=GameManager.instance;
        gameManager.savedPosition = initialPosition;
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("FallBoundaries"))
        {
            gameManager.Respawn();
        }
    }
}
