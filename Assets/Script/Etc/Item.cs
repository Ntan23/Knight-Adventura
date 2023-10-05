using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{
    public enum InteractionType{None,PickUp,Examine,GrabNDrop}
    public enum ItemType{Static,Consumables}

    [Header("Attributes")]
    public InteractionType type;
    public ItemType itemType;
    public bool isStackable;

    [Header("Pick Up")]
    public string itemDescription;
    
    [Header("Examine")]
    public string descriptionText;
    
    [Header("Custom Events")]
    public UnityEvent customEvent;

    [Header("Consume Events")]
    public UnityEvent consumeEvent;

    InventorySystem inventory;
    InteractionSystem interact;
    AudioManager audioManager;
    [SerializeField] Lever leverScript;
    private void Awake() 
    {
        inventory=FindObjectOfType<InventorySystem>();
        interact=FindObjectOfType<InteractionSystem>();
    }

    private void Start()
    {
        audioManager = AudioManager.instance;
        
        switch(type)
        {
            case InteractionType.PickUp :
                this.tag = "PickUp";
                break;
            case InteractionType.Examine :
                this.tag = "Examine";
                break; 
            case InteractionType.GrabNDrop :
                this.tag = "GrabNDrop";
                break;
            default :
                break;
        }
    }

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger=true;
        gameObject.layer=7;
    }

    public void InteractObject()
    {
        switch(type)
        {
            case InteractionType.PickUp :
                if(!inventory.CanPickUpItems())
                {
                    return;
                }
                //Add The Item To The Picked Up Items List
                inventory.PickUpItems(gameObject);
                //Delete The Item
                gameObject.SetActive(false);
                break;
            case InteractionType.Examine :
                //Call The Examine Item Function In The Interaction System
                interact.ExamineItem(this);
                break; 
            case InteractionType.GrabNDrop :
                interact.GrabAndDrop();
                break;
            default :
                if(leverScript != null)
                {
                    if(leverScript.isOpenClose)
                    {
                        return;
                    }
                    else if(!leverScript.isOpenClose)
                    {
                        audioManager.Play("Interact");
                    }
                }
                else if(leverScript == null)
                {
                    if(this.gameObject.name == "Goal")
                    {
                        if(this.gameObject.GetComponent<Goal>() == null)
                        {
                            return;
                        }
                        else if(this.gameObject.GetComponent<Goal>() != null)
                        {
                            if(!this.gameObject.GetComponent<Goal>().isComplete)
                            {
                                audioManager.Play("Interact");
                            }
                            else if(this.gameObject.GetComponent<Goal>().isComplete)
                            {
                                return;
                            }
                        }
                    }

                    audioManager.Play("Interact");
                }
                break;
        }

        //Invoke(Call) Custom Events
        if(!interact.isExamining)
        {
            customEvent?.Invoke();
        }
    }
}
