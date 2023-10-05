using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LivesCount : MonoBehaviour
{
    #region  Variables
    [Header("Health")]
    public Image[] lives;
    public int livesRemaining;
    Animator animator;
    GameObject player,traps;
    public bool isInvunerable;

    [Header("Hurt iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private float numberOfFlashes;

    [Header("Respawn iFrames")]
    [SerializeField] private float respawnIFramesDuration;
    [SerializeField] private float respawnNumberOfFlashes;
    private SpriteRenderer spriteRenderer;
    GameManager gameManager;
    InventorySystem inventory;
    AudioManager audioManager;
    public bool isHurt;
    InteractionSystem interact;
    #endregion

    private void Awake()
    {
        animator=GetComponent<Animator>();
        spriteRenderer=GetComponent<SpriteRenderer>();
        player=GameObject.FindGameObjectWithTag("Player");
        traps=GameObject.FindGameObjectWithTag("Traps");
        gameManager=FindObjectOfType<GameManager>();
        inventory=GetComponent<InventorySystem>();
        livesRemaining=3;
        numberOfFlashes = iFramesDuration*2;
        respawnNumberOfFlashes = respawnIFramesDuration*3;
        isInvunerable=false;
        interact=GetComponent<InteractionSystem>();
    }

    private void Start()
    {
        audioManager=AudioManager.instance;
    }
    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene,LoadSceneMode mode) 
    {
        isInvunerable = false;
    }

    public void LoseLive()
    {
        if(livesRemaining==0)
        {
            return;
        }
        isHurt = true;
        if(interact.isGrabbing)
        {
            interact.GrabAndDrop();
        }
        audioManager.Play("KnightHurt");
        StartCoroutine(stun());
        //Decrease The Value Of Lives Remaining
        livesRemaining--;
        //Play Hurt Animation
        animator.SetTrigger("Hurt");
        StartCoroutine(Invunerability());
        //Hide One Of The Lives Images
        lives[livesRemaining].gameObject.SetActive(false);
        //If We Run Out Of Lives We Lose The Game
        if(livesRemaining == 0)
        {
            gameManager.Die();
        }
    }

    public void Restore()
    {
        livesRemaining=3;
        for(int i=0;i<livesRemaining;i++)
        {
            lives[i].gameObject.SetActive(true);
        }
    }

    public void potion()
    {
        if(livesRemaining==3)
        {
            inventory.canConsume=false;
        }
        else if(livesRemaining<3)
        {
            inventory.canConsume=true;
            lives[livesRemaining].gameObject.SetActive(true);
            livesRemaining+=1;
        }
    }

    public void respawn()
    {
        StartCoroutine(RespawnInvunerability());
    }

    public void invicible(float invisibleDuration)
    {
        inventory.canConsume = true;
        StartCoroutine(InvicibleMode(invisibleDuration));
    }
    
    IEnumerator Invunerability()
    {
        isInvunerable=true;
        Physics2D.IgnoreLayerCollision(8,11,true);
        for (int i=0;i<numberOfFlashes;i++)
        {
            spriteRenderer.color = new Color(1,0,0,0.8f);
            yield return new WaitForSeconds(iFramesDuration/(numberOfFlashes*2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration/(numberOfFlashes*2));
        }
        Physics2D.IgnoreLayerCollision(8,11,false);
        isInvunerable=false;
    }

    IEnumerator RespawnInvunerability()
    {
        isInvunerable=true;
        Physics2D.IgnoreLayerCollision(8,11,true);
        for (int i=0;i<respawnNumberOfFlashes;i++)
        {
            spriteRenderer.color = new Color(1.0f,1.0f,0,0.8f);
            yield return new WaitForSeconds(respawnIFramesDuration/(respawnNumberOfFlashes*2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(respawnIFramesDuration/(respawnNumberOfFlashes*2));
        }
        Physics2D.IgnoreLayerCollision(8,11,false);
        isInvunerable=false;
    }

    IEnumerator InvicibleMode(float invicibleIFrameDuration)
    {
        isInvunerable=true;
        Physics2D.IgnoreLayerCollision(8,11,true);

        for(int i=0;i<invicibleIFrameDuration;i++)
        {
            spriteRenderer.color = new Color(1.0f,1.0f,1.0f,0.8f);
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
        
        Physics2D.IgnoreLayerCollision(8,11,false);
        isInvunerable=false;
    }

    IEnumerator stun()
    {
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }
}
