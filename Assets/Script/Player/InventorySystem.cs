using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour,IData
{
    [System.Serializable]
    //Inventory Item Class
    public class InventoryItem
    {
        public GameObject obj;
        public int stack=1;

        public InventoryItem(GameObject o,int s=1)
        {
            obj=o;
            stack=s;
        }
    }

    #region Variables
    [Header("General")]
    //List Of Picked Up Items
    public List<InventoryItem> items = new List<InventoryItem>();
    public bool isOpen;
    [Header("UI Item")]
    //Inventory System Window
    public GameObject inventoryWindow;
    public GameObject lives;
    public GameObject fireball;
    public GameObject sword;
    public Text restoreText;
    public Image[] itemImages;
    public Text[] count;
    [Header("UI Description")]
    public Image descriptionImage;
    public Text itemName;
    public Text itemDescription;
    [SerializeField] GameObject coinUI;
    InteractionSystem interact;
    ModeChanger mode;
    Shooting playerRangeAttack;
    public bool canConsume;
    bool fireballUnlocked;
    private float startTime;
    [SerializeField] float duration;
    PlayerMovement playerMovement;
    AudioManager audioManager;
    Goal goal;
    PauseMenu pause;
    #endregion

    private void Awake() 
    {
        audioManager = AudioManager.instance;
        goal = FindObjectOfType<Goal>();
        inventoryWindow.SetActive(false);
        interact=GetComponent<InteractionSystem>();
        mode=GetComponent<ModeChanger>();
        playerRangeAttack=GetComponent<Shooting>();
        playerMovement = GetComponent<PlayerMovement>();
        pause = FindObjectOfType<PauseMenu>();
    }

    void Start()
    {
        startTime=Time.time;
    }

    public void LoadData(GameData data)
    {
        this.fireballUnlocked=data.fireballUnlock;
    }

    public void SaveData(GameData data)
    {

    }
    
    private void Update()
    {
        if(Time.time-startTime < duration)
        {
            return;
        }

        if(pause.isPaused)
        {
            return;
        }

        if(goal.isComplete)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            if(!isOpen)
            {
                audioManager.Play("Interact");
            }
            else if(isOpen)
            {
                audioManager.Play("Interact2");
            }
            
            isOpen=!isOpen;
        }
        
        inventoryWindow.SetActive(isOpen);
        lives.SetActive(!isOpen);
        coinUI.SetActive(!isOpen);

        if(!fireballUnlocked)
        {
            sword.SetActive(!isOpen);
        }
        else if(fireballUnlocked)
        {
            if(mode.swordMode)
            {
                sword.SetActive(!isOpen);
            }
            else if(!mode.swordMode)
            {
                fireball.SetActive(!isOpen);
            
                if(playerRangeAttack.isRestore)
                {
                    restoreText.gameObject.SetActive(!isOpen);
                }
            }
        }
    }

    //Add The Item To The List
    public void PickUpItems(GameObject item)
    {
        audioManager.Play("Interact");
        //If Item Is Stackable
        if(item.GetComponent<Item>().isStackable)
        {
            //Check If We Have An Existing Item In The Inventory
            InventoryItem existingItem=items.Find(x=>x.obj.name==item.name);
            //If Yes Stack It 
            if(existingItem!=null)
            {
                existingItem.stack++;
            }   
            //If No Add It As A New Item
            else
            {
                InventoryItem inventory = new InventoryItem(item);
                items.Add(inventory);
            }
        }
        //If Item Isn't Stackable
        else if(!item.GetComponent<Item>().isStackable)
        {
            InventoryItem inventory = new InventoryItem(item);
            items.Add(inventory);
        }
    
        UpdateUI();
    }

    public bool CanPickUpItems()
    {
        if(items.Count>=itemImages.Length)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Refresh UI Element In Inventory Window
    void UpdateUI()
    {
        HideAll();
        //For each item in the "items" list
        //Show it in the slot in the "itemImages" And "count"
        for(int i=0;i<items.Count;i++)
        {
            itemImages[i].sprite=items[i].obj.GetComponent<SpriteRenderer>().sprite;
            count[i].text="x "+items[i].stack;
            itemImages[i].gameObject.SetActive(true);
            count[i].gameObject.SetActive(true);
        }
    }

    //Hide All The Items UI Images
    void HideAll()
    {
        foreach(var image in itemImages)
        {
            image.gameObject.SetActive(false);
        }
        foreach(var text in count)
        {
            text.gameObject.SetActive(false);
        }
        HideDescription();
    }

    public void showDescription(int id)
    {
        //Set The Image
        descriptionImage.sprite=itemImages[id].sprite;
        //Set The Item Name
        itemName.text=items[id].obj.name;
        itemDescription.text=items[id].obj.GetComponent<Item>().itemDescription;
        //Show The Window
        descriptionImage.gameObject.SetActive(true);
        itemName.gameObject.SetActive(true);
        itemDescription.gameObject.SetActive(true);
    }

    public void HideDescription()
    {
        descriptionImage.gameObject.SetActive(false);
        itemName.gameObject.SetActive(false);
        itemDescription.gameObject.SetActive(false);
    }

    public void Consume(int id)
    {
        if(items[id].obj.GetComponent<Item>().itemType==Item.ItemType.Consumables)
        {
            //Invoke Consume Custom Event
            items[id].obj.GetComponent<Item>().consumeEvent?.Invoke();
            if(canConsume)
            {
                //Reduce The Stack of Items
                items[id].stack--;
                if(items[id].stack==0)
                {
                    //Destroy The Item In Very Tiny Time
                    Destroy(items[id].obj,0.2f);
                    //Clear The Item From The List
                    items.RemoveAt(id);
                }
                //Update UI
                UpdateUI();
            }
        }
    }
}
