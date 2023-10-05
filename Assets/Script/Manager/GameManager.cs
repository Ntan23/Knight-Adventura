using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour,IData
{
    public static GameManager instance;
    [HideInInspector] public bool isDead;
    GameObject player,enemy,traps;
    [HideInInspector] public Vector3 savedPosition;
    Animator playerAnimator;
    [SerializeField] GameObject loseButton;
    [SerializeField] GameObject loseWindow;
    Shooting playerRangeAttack;
    InteractionSystem interact;
    InventorySystem inventory;
    PlayerMeleeAttack playerMeleeAttack;
    LivesCount playerHealth;
    Collider2D playerCollider;
    Collider2D enemyCollider;
    Collider2D trapCollider;
    PlayerMovement playerMovement;
    CameraFollow cameraFollow;
    [SerializeField] GameObject coins;
    [SerializeField] GameObject jumpLandEffect;
    Animator jumpLandEffectAnimator;
    [SerializeField] EnemyChaseAI[] enemyChaseAI;
    Rigidbody2D rb;
    public float[] savedAgroRange;
    bool fireballUnlock;
    DrownInWater drownInWater;
    RespawnCounter respawnCounter;
    CoinTextUpdater coinText; 
    public bool canSave;
    bool isSaved;
    public bool restart;
    AudioManager audioManager;
    Shooting shoot;
    PlayerMeleeAttack meleeAttack;
    [HideInInspector] public int savedCoinCount = 0;
    private void Awake()
    {
        if(instance == null)
        {
            instance=this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioManager = AudioManager.instance;
        player=GameObject.FindGameObjectWithTag("Player");
        rb=player.GetComponent<Rigidbody2D>();
        enemy=GameObject.FindGameObjectWithTag("Enemy");
        traps=GameObject.FindGameObjectWithTag("Traps");
        playerAnimator=player.GetComponent<Animator>();
        playerRangeAttack=player.GetComponent<Shooting>();
        interact=player.GetComponent<InteractionSystem>();
        inventory=player.GetComponent<InventorySystem>();
        playerMeleeAttack=player.GetComponent<PlayerMeleeAttack>();
        playerHealth=player.GetComponent<LivesCount>();
        playerCollider=player.GetComponent<Collider2D>();
        enemyCollider=enemy?.GetComponent<Collider2D>();
        trapCollider=traps?.GetComponent<Collider2D>();
        playerMovement=player.GetComponent<PlayerMovement>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        jumpLandEffectAnimator = jumpLandEffect.GetComponent<Animator>();
        loseButton.SetActive(false);
        loseWindow.SetActive(false);
        drownInWater = FindObjectOfType<DrownInWater>();
        respawnCounter = FindObjectOfType<RespawnCounter>();
        coinText = FindObjectOfType<CoinTextUpdater>();
        shoot = player.GetComponent<Shooting>();
        meleeAttack = player.GetComponent<PlayerMeleeAttack>();
        isDead = false;
        canSave = false;
        isSaved = false;
    }

    public void LoadData(GameData gameData)
    {
        this.fireballUnlock=gameData.fireballUnlock;
    }

    public void SaveData(GameData gameData)
    {
        if(canSave)
        {
            if(!isSaved)
            {
                gameData.coinCount += coinText.coinCount;
                isSaved = true;
            }
            canSave = false;
        }
    }

    public void Die()
    {
        audioManager.Play("Lose");
        rb.bodyType = RigidbodyType2D.Static;
        isDead=true;
        savedCoinCount += coinText.coinCount;
        canSave = true;
        
        if(enemyChaseAI != null)
        {
            for(int i=0;i<enemyChaseAI.Length;i++)
            {
                savedAgroRange[i]=enemyChaseAI[i].agroRange;
                enemyChaseAI[i].agroRange = 0;
            }
        }
        
        if(interact.isGrabbing)
        {
            interact.GrabAndDrop();
        }

        if(playerMovement.isCrouch)
        {
            playerAnimator.SetBool("Crouch",false);
            playerMovement.isCrouch = false;
        }

        if(shoot.isShooting)
        {
            shoot.isShooting = false;
        }

        if(meleeAttack.isAttack)
        {
            meleeAttack.isAttack = false;
        }

        jumpLandEffectAnimator.ResetTrigger("Jump");
        jumpLandEffectAnimator.ResetTrigger("Land");
        StartCoroutine(animatorDelay());
        playerAnimator.SetBool("Jump",false);
        playerAnimator.ResetTrigger("Hurt");
        playerAnimator.ResetTrigger("MeleeAttack");
        playerAnimator.ResetTrigger("Cast");
        playerAnimator.SetTrigger("Die");
        player.gameObject.layer=0;
        playerMovement.isAirborne = false;
        playerMovement.enabled = false;
        cameraFollow.enabled = false;
        if(enemyCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider,enemyCollider,true);
        }
        if(trapCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider,trapCollider,true);
        }
        loseWindow.SetActive(true);
        loseButton.SetActive(true);
        respawnCounter.loadData();
        if(fireballUnlock)
        {
            playerRangeAttack.enabled=false;
        }
    
        interact.enabled=false;
        inventory.enabled=false;
        playerMeleeAttack.enabled=false;
    }

    public void Restart()
    {
        isSaved = false;
        restart = true;

        if(enemyChaseAI!=null)
        {
            for(int i=0;i<enemyChaseAI.Length;i++)
            {
                enemyChaseAI[i].agroRange = savedAgroRange[i];
            }
        }

        jumpLandEffectAnimator.enabled = true;
        jumpLandEffectAnimator.ResetTrigger("Jump");
        jumpLandEffectAnimator.ResetTrigger("Land");
        rb.bodyType = RigidbodyType2D.Dynamic;
        loseButton.SetActive(false);
        loseWindow.SetActive(false);
        player.transform.position = savedPosition;
        playerMovement.enabled = true;
        cameraFollow.enabled = true;
        playerAnimator.SetTrigger("Respawn");
        playerHealth.respawn();
        StartCoroutine(wait(1.5f));
        player.gameObject.layer=8;
        if(enemyCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider,enemyCollider,false);
        }
        if(trapCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider,trapCollider,false);
        }
        playerHealth.Restore();
        if(fireballUnlock)
        {
            playerRangeAttack.enabled=true;
            playerRangeAttack.ResetFireball();
        }
        interact.enabled=true;
        inventory.enabled=true;
        playerMeleeAttack.enabled=true;
    }

    IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);
        isDead=false;
    }

    IEnumerator animatorDelay()
    {
        yield return new WaitForSeconds(1.0f);
        jumpLandEffectAnimator.enabled = false;
    }

    public void Respawn()
    {
        if(playerRangeAttack.isShooting)
        {
            playerRangeAttack.isShooting = false;
        }
        if(interact.isGrabbing)
        {
            interact.GrabAndDrop();
        }
        if(!playerHealth.isInvunerable)
        {
            playerHealth.LoseLive();
        }
        if(playerHealth.livesRemaining > 0)
        {
            isDead = true;
            StartCoroutine(wait(1.0f));
            player.transform.position = savedPosition;
            playerAnimator.ResetTrigger("Hurt");
            playerAnimator.SetTrigger("Respawn");
            playerHealth.respawn();
        }
    }
}
