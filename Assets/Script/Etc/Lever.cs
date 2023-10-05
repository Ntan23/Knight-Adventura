using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    Animator animator;
    [SerializeField] GameObject door;
    [SerializeField] Animator otherLeverAnimator;
    [SerializeField] Lever otherLeverLeverScript;
    float scaleSpeed = 2.0f;
    public bool isActivated;
    public UnityEvent leverActivate;
    public UnityEvent leverDeactivate;
    float doorYScale;
    [HideInInspector] public bool isOpenClose;
    // Start is called before the first frame update
    void Start()
    {
        isActivated = false;
        animator=GetComponent<Animator>();
        if(door != null)
        {
            doorYScale = door.transform.localScale.y;
        }
        FindObjectOfType<InteractionSystem>().examineText.SetActive(false);
    }

    // Update is called once per frame
    public void PlayAnimation()
    {
        animator.SetBool("Activate",true);
        FindObjectOfType<InteractionSystem>().interactText.SetActive(false);
        ChangeType();
    }

    void ChangeType()
    {
        this.GetComponent<Item>().type=Item.InteractionType.Examine;
        this.tag="Examine";
    }

    public void openDoor()
    {
        if(door != null)
        {
            StartCoroutine(OpenDoor());
        }
    }

    public void closeDoor()
    {
        if(door != null)
        {
            StartCoroutine(CloseDoor());
        }
    }
    IEnumerator OpenDoor()
    {
        while(door.transform.localScale.y > 0.1f)
        {
            isOpenClose = true;
            door.transform.localScale -= new Vector3(0,scaleSpeed,0) * Time.deltaTime;
            if(door.transform.localScale.y == 0)
            {
                yield break;
            }
            yield return null;
        }
        isOpenClose = false;
    }

    IEnumerator CloseDoor()
    {
        while(door.transform.localScale.y < doorYScale)
        {
            isOpenClose = true;
            door.transform.localScale += new Vector3(0,scaleSpeed,0) * Time.deltaTime;
            if(door.transform.localScale.y == doorYScale)
            {
                yield break;
            }
            yield return null;
        }
        isOpenClose = false;
    }

    public void ChangeWaterDirection()
    {
        if(isActivated)
        {
            animator.SetBool("Active",false);
            isActivated = false;
            otherLeverAnimator.SetBool("Active",true);
            otherLeverLeverScript.isActivated = true;
            leverDeactivate?.Invoke();
        }
        else if(!isActivated)
        {
            animator.SetBool("Active",true);
            isActivated =  true;
            otherLeverAnimator.SetBool("Active",false);
            otherLeverLeverScript.isActivated = false;
            leverActivate?.Invoke();
        }
    }

    public void ActivateDeactivate()
    {
        if(isOpenClose)
        {
            return;
        }

        if(isActivated)
        {
            isActivated = false;
            animator.SetBool("Activate",false);
            leverDeactivate?.Invoke();
        }
        else if(!isActivated)
        {
            isActivated = true;
            animator.SetBool("Activate",true);
            leverActivate?.Invoke();
        }
    }
}
