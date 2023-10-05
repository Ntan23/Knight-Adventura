using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    #region Variables
    Animator animator;
    public Text restoringText;
    public Image[] fireBalls;
    public int fireBallRemaining;
    public GameObject shootingItem;
    public Transform shootingPoint;
    [SerializeField] float shootCooldown;
    float cooldownTimer;
    public bool isRestore;
    public bool isShooting;
    ObjectPoolManager objectPoolManager;
    AudioManager audioManager;
    ModeChanger modeChanger;
    DrownInWater drownInWater;
    Goal goal;
    InteractionSystem interact;
    PauseMenu pause;
    GameManager gameManager;
    #endregion

    void Awake()
    {
        modeChanger = GetComponent<ModeChanger>();
        animator=GetComponent<Animator>();
        drownInWater = FindObjectOfType<DrownInWater>();
        goal = FindObjectOfType<Goal>();
        interact = GetComponent<InteractionSystem>();
        pause = FindObjectOfType<PauseMenu>();
        fireBallRemaining=3;
        restoringText.gameObject.SetActive(false);
        isShooting=false;
    }

    // Start is called before the first frame update
    void Start()
    {
        objectPoolManager=ObjectPoolManager.instance;
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if(goal.isComplete)
        {
            return;
        }

        if(pause.isPaused)
        {
            return;
        }

        if(gameManager.isDead)
        {
            return;
        }

        cooldownTimer+=Time.deltaTime;

        if(cooldownTimer>=shootCooldown && modeChanger.canAttack && !interact.isGrabbing)
        {
            if(Input.GetKeyDown(KeyCode.W) && fireBallRemaining!=0 && !isRestore)
            {
                if(drownInWater != null)
                {
                    if(drownInWater.inTheWater)
                    {
                        return;
                    }
                    else if(!drownInWater.inTheWater)
                    {
                        animator.SetTrigger("Cast");
                        cooldownTimer=0;
                    }
                }
                else if(drownInWater == null)
                {
                    animator.SetTrigger("Cast");
                    cooldownTimer=0;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.R) && fireBallRemaining < 3 && fireBallRemaining != 0 && !isRestore && !isShooting)
        {
            StartCoroutine(RestoreFireball(2.0f));
        }
    }

    void PlaySFX()
    {
        audioManager.Play("FireballWhoosh");
    }

    public void shootChecker(int value)
    {
        if(value==1)
        {
            isShooting=true;
        }
        else if(value==0)
        {
            isShooting=false;
        }
    }

    public void Shoot()
    {
        if(fireBallRemaining == 0)
        {
            return;
        }

        //Object Pool
        GameObject playerFireball = objectPoolManager.GetPooledObject("Fireball");

        if(playerFireball != null)
        {   
            playerFireball.transform.position=shootingPoint.position;
            playerFireball.transform.localScale=this.gameObject.transform.localScale;
            playerFireball.SetActive(true);
        }

        fireBallRemaining--;
        fireBalls[fireBallRemaining].GetComponent<Image>().color = new Color32(144,144,144,255);

        if(fireBallRemaining == 0 && !isRestore)
        {
            StartCoroutine(RestoreFireball(2.0f));
        }
    }

    IEnumerator RestoreFireball(float time)
    {
        restoringText.gameObject.SetActive(true);
        isRestore=true;
        for(int i=fireBallRemaining;i<3;i++)
        {
            yield return new WaitForSeconds(time);
            fireBalls[i].GetComponent<Image>().color = new Color32(255,255,255,255);
        }
        fireBallRemaining=3;
        restoringText.gameObject.SetActive(false);
        isRestore=false;
    }

    public void ResetFireball()
    {
        for(int i=0;i<3;i++)
        {
            fireBalls[i].GetComponent<Image>().color = new Color32(255,255,255,255);
        }
        fireBallRemaining=3;
    }
}
