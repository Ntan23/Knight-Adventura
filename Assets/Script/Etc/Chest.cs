using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    Animator animator;
    public Vector3 startPosition;
    public float stepScale;
    [SerializeField] GameObject key;
    Key keyScript;
    public bool isCollect = false;
    bool canMove = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        FindObjectOfType<InteractionSystem>().examineText.SetActive(false);
        keyScript = key?.GetComponent<Key>();
        isCollect = false;
        canMove = false;
    }

    private void Update() 
    {
        if(!canMove)
        {
            return;
        }
        if(canMove)
        {
            keyScript.PlayAnimation();
        }
    }

    // Update is called once per frame
    public void PlayAnimation()
    {
        FindObjectOfType<InteractionSystem>().pickUpText.SetActive(false);
        FindObjectOfType<InteractionSystem>().examineText.SetActive(false);
        FindObjectOfType<InteractionSystem>().interactText.SetActive(false);
        animator.SetBool("ChestOpen",true);
        ChangeType();
    }

    void ChangeType()
    {
        this.GetComponent<Item>().type=Item.InteractionType.Examine;
        this.tag="Examine";
        Check();
    }

    void Check()
    {
        if(!isCollect)
        {
            key.SetActive(true);
    
            startPosition = key.transform.position;

            if(keyScript != null)
            {
                float distance = Vector3.Distance(startPosition,keyScript.dropPosition.transform.position);

                stepScale = keyScript.speed/distance;
                canMove = true;
            }
        }
        else if(isCollect)
        {
            key.SetActive(false);
            canMove = false;
        }
    }
}
