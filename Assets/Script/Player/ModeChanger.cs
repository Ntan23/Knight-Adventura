using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeChanger : MonoBehaviour,IData
{
    GameObject player;
    public bool swordMode;
    [SerializeField] GameObject fireballsUI;
    [SerializeField] GameObject swordImage;

    Shooting shoot;
    PlayerMeleeAttack playerMeleeAttack;
    bool fireballUnlocked;
    float startTime;
    float duration = 2.5f;
    public bool canAttack;
    Goal goal;
    PauseMenu pause;

    private void Awake()
    {
        player=GameObject.FindGameObjectWithTag("Player");
        shoot=player.GetComponent<Shooting>();
        playerMeleeAttack=player.GetComponent<PlayerMeleeAttack>();
        goal = FindObjectOfType<Goal>();
        pause = FindObjectOfType<PauseMenu>();
        fireballsUI.SetActive(false);
        swordMode=true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    public void SaveData(GameData data)
    {
        
    }

    public void LoadData(GameData data)
    {
        this.fireballUnlocked=data.fireballUnlock;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime < duration)
        {
            canAttack = false;
            return;
        }

        if(goal.isComplete)
        {
            return;
        }

        if(pause.isPaused)
        {
            return;
        }

        canAttack = true;
        
        if(Input.GetKey("1"))
        {
            swordMode=true;
        }
        if(Input.GetKey("2"))
        {
            swordMode=false;
        }

        checkMode();
    }   

    void checkMode()
    {
        if(!fireballUnlocked)
        {
            swordImage.SetActive(true);
            fireballsUI.SetActive(false);
            shoot.enabled=false;
            playerMeleeAttack.enabled=true;
        }
        else if(fireballUnlocked)
        {
            if(swordMode)
            {
                fireballsUI.SetActive(false);
                swordImage.SetActive(true);
                shoot.enabled=false;
                playerMeleeAttack.enabled=true;
            }
            else if(!swordMode)
            {
                swordImage.SetActive(false);
                fireballsUI.SetActive(true);
                shoot.enabled=true;
                playerMeleeAttack.enabled=false;
            }
        }
    }
}
