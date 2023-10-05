using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    #region Variables
    [Header("Detection")]
    public Transform detectionPoint;
    private const float detectionRadius=0.3f;
    public LayerMask detectionLayer;
    public GameObject detectedObject;
    [Header("Examine")]
    public GameObject examineWindow;
    public Image itemImage;
    public Text description;
    public bool isExamining;
    [Header("Grab & Drop")]
    public GameObject grabbedObject;
    public Transform grabPoint;
    Transform savedParent;
    bool isDestroy = false;
    public bool isGrabbing = false;
    [Header("Others")]
    //Variable For Show Text (Type Of Interaction)
    [SerializeField] GameObject textContainer;
    public GameObject pickUpText;
    public GameObject examineText;
    public GameObject interactText;
    public GameObject inventoryFullText;
    [SerializeField]  GameObject grabText;
    [SerializeField]  GameObject dropText;
    [SerializeField] Vector3 textOffset;

    private GameObject player;
    InventorySystem inventory;
    AudioManager audioManager;
    Activator activator;
    Goal goal;
    PauseMenu pause;
    InventorySystem inventorySystem;
    #endregion

    private void Awake()
    {
        audioManager = AudioManager.instance;
        player=GameObject.FindGameObjectWithTag("Player");
        inventory=GetComponent<InventorySystem>();
        activator=FindObjectOfType<Activator>();
        goal = FindObjectOfType<Goal>();
        pause = FindObjectOfType<PauseMenu>();
        inventorySystem = GetComponent<InventorySystem>();
        examineWindow.SetActive(false);
        pickUpText.SetActive(false);
        examineText.SetActive(false);
        interactText.SetActive(false);
        inventoryFullText.SetActive(false);
        grabText.SetActive(false);
        dropText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    { 
        if(goal.isComplete)
        {
            return;
        }

        if(pause.isPaused)
        {
            return;
        }

        if(inventory.isOpen)
        {
            return;
        }
    
        if(DetectObjects())
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(isGrabbing)
                {
                    GrabAndDrop();
                    return;
                }
                
                detectedObject.GetComponent<Item>().InteractObject();
            }
            
            return;
        }
        else if(!DetectObjects())
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(isExamining)
                {
                    DisableExamineWindow();
                }
            }
        }
    }
    bool DetectObjects()
    {
        Collider2D detected = Physics2D.OverlapCircle(detectionPoint.position,detectionRadius,detectionLayer);
        
        if(detected == null)
        {
            detectedObject = null;
            pickUpText.SetActive(false);
            examineText.SetActive(false);
            interactText.SetActive(false);
            inventoryFullText.SetActive(false);
            grabText.SetActive(false);
            dropText.SetActive(false);
            return false;
        }
        
        if(detected != null)
        {
            detectedObject = detected.gameObject;

            if(isGrabbing)
            {
                grabText?.SetActive(false);
            }

            showText();
        } 
        return true;
    }

    //Show Type Of Interaction
    void showText()
    {
        Vector3 targetPosition=detectedObject.transform.position+textOffset;

        switch(detectedObject.tag)
        {
            case "PickUp" :
                textContainer.transform.position=targetPosition;
                pickUpText.SetActive(true);
                examineText.SetActive(false);
                interactText.SetActive(false);
                inventoryFullText.SetActive(false);
                grabText.SetActive(false);
                if(isExamining)
                {
                    pickUpText.SetActive(false);
                    examineText.SetActive(false);
                }
                if(isGrabbing)
                {
                    pickUpText.SetActive(false);
                }
                if(!inventory.CanPickUpItems())
                {
                    targetPosition=player.transform.position+new Vector3(0,1,0);
                    textContainer.transform.position=targetPosition;
                    inventoryFullText.SetActive(true);
                    pickUpText.SetActive(false);
                }
                break;
            case "Examine" :
                textContainer.transform.position=targetPosition;
                examineText.SetActive(true);
                pickUpText.SetActive(false);
                interactText.SetActive(false);
                inventoryFullText.SetActive(false);
                grabText.SetActive(false);
                if(isExamining)
                {
                    examineText.SetActive(false);
                }
                if(isGrabbing)
                {
                    examineText.SetActive(false);
                }
                break;
            case "GrabNDrop" :
                textContainer.transform.position=targetPosition;
                pickUpText.SetActive(false);
                examineText.SetActive(false);
                interactText.SetActive(false);
                inventoryFullText.SetActive(false);
                grabText.SetActive(true);
                if(isExamining)
                {
                    examineText.SetActive(false);
                }
                if(isGrabbing)
                {
                    grabText.SetActive(false);
                    dropText.SetActive(true);
                }
                break;
            default :
                textContainer.transform.position=targetPosition;
                interactText.SetActive(true);
                pickUpText.SetActive(false);
                examineText.SetActive(false);
                inventoryFullText.SetActive(false);
                grabText.SetActive(false);
                if(isExamining)
                {
                    interactText.SetActive(false);
                    examineText.SetActive(false);
                }
                if(isGrabbing)
                {
                    interactText.SetActive(false);
                }
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.black;
        Gizmos.DrawSphere(detectionPoint.position,detectionRadius);
    }

    public void ExamineItem(Item item)
    {
        if(!isExamining)
        {
            audioManager.Play("Interact");
            //Show The Item's Image In The Middle
            itemImage.sprite=item.GetComponent<SpriteRenderer>().sprite;
            //Write Description Text Underneath The Image
            description.text=item.descriptionText;
            //Display An Examine Window
            examineWindow.SetActive(true);
            //Enable The isExamining Boolean
            isExamining = true;
        }
        else if(isExamining)
        {
            DisableExamineWindow();
        }
    }

    void DisableExamineWindow()
    {
        audioManager.Play("Interact2");
        //Hide the Examine Window
        examineWindow.SetActive(false);
        //disable the isExamining boolean
        isExamining = false;
    }

    public void GrabAndDrop()
    {
        audioManager.Play("Interact");
        //Check if we have a grabbed object , if have drop it
        if(isGrabbing)
        {
            //Disable the text after drop it
            pickUpText.SetActive(false);
            examineText.SetActive(false);
            interactText.SetActive(false);
            inventoryFullText.SetActive(false);
            grabText.SetActive(false);
            dropText.SetActive(false);
            //make isGrabbing false
            isGrabbing = false;
            //Parent the grabbed object to the previous parent
            grabbedObject.transform.parent = savedParent;            
            //set the y position to its origin
            grabbedObject.transform.position = new Vector3(grabbedObject.transform.position.x,grabbedObject.transform.position.y,grabbedObject.transform.position.z);
            //If there is no rigidbody and the rigidbody has been destroy because of grab , add the rigidbody back to the grabbedObject back when drop it
            if(grabbedObject.GetComponent<Rigidbody2D>() == null && isDestroy)
            {
                grabbedObject.AddComponent<Rigidbody2D>();
                isDestroy = false;
            }
            //null the grabbed object reference
            grabbedObject = null;
        }
        //Check if we have no grabbed object , grab the detected item
        else if(!isGrabbing)
        {
            //Enable the isGrabbing bool
            isGrabbing = true;
            //assign the grabbed object to the object itself
            grabbedObject = detectedObject;
            //If there is rigidbody , destroy it from the grabbedObject
            if(grabbedObject.GetComponent<Rigidbody2D>() != null)
            {
                isDestroy = true;
                Destroy(grabbedObject.GetComponent<Rigidbody2D>());
            }
            
            if(activator != null)
            {
                if(activator.isCollide)
                {
                    activator.isCollide = false;
                }
            }
    
            //Save the grabbed object parent
            savedParent = grabbedObject.transform.parent;
            //Parent the grabbed object to the player
            grabbedObject.transform.parent = this.gameObject.transform;
            //Adjust the position of the grabbed object to be closer to hands                        
            grabbedObject.transform.position = grabPoint.position;
        }
    }
}
