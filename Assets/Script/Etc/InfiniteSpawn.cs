using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteSpawn : MonoBehaviour
{
    Vector3 intialPos;
    Quaternion rotationValue;
    [SerializeField] float intialXForce;
    private void Awake()
    {
        intialPos = this.gameObject.transform.position;
        rotationValue = this.gameObject.transform.rotation;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("SpawnTrigger"))
        {
            this.gameObject.transform.position = intialPos;
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(intialXForce,0));
            this.gameObject.transform.rotation = rotationValue;
        }
    }
}
