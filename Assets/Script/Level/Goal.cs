using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour,IData
{
    #region Variables
    public int LevelAmount;
    int currentLevel;
    int NextLevel;
    bool[] levelsUnlocked = new bool[15];
    [SerializeField] GameObject textContainer;
    [SerializeField] GameObject winWindow;
    [SerializeField] GameObject livesUI; 
    [SerializeField] GameObject fireballsUI;
    [SerializeField] GameObject swordSprite;
    [SerializeField] GameObject buttonUI;
    public bool isComplete;
    GameObject player;
    Animator animator1;
    Animator animator2;
    GameData gameData;
    Item item;
    AudioManager audioManager;
    CoinTextUpdater coinsText;
    SceneController sceneController;
    GameManager gameManager;
    LivesCount livesCount;
    public bool canSave;
    [SerializeField] EnemyChaseAI[] enemyChaseAI;
    #endregion

    private void Awake()
    {
        player=GameObject.FindGameObjectWithTag("Player");
        animator1=GetComponent<Animator>();
        animator2=player.GetComponent<Animator>();
        livesCount=player.GetComponent<LivesCount>();
        coinsText=FindObjectOfType<CoinTextUpdater>();
        sceneController=FindObjectOfType<SceneController>();
        winWindow.SetActive(false);
        buttonUI.SetActive(false);
        canSave = false;
    }

    void Start ()
    {
        audioManager=AudioManager.instance;
        gameManager=GameManager.instance;
    }

    public void LoadData(GameData data)
    {

    }

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            data.levelsUnlocked[currentLevel] = this.levelsUnlocked[currentLevel];
            canSave = false;
        }   
    }
    public void Check()
    {
        if(item == null)
        {
            item = GetComponent<Item>();
        }

        if(item.type == Item.InteractionType.None)
        {
            player.gameObject.layer = 0;
            livesCount.isInvunerable = true;
            if(enemyChaseAI.Length > 0)
            {
                for(int i=0;i<enemyChaseAI.Length;i++)
                {
                    enemyChaseAI[i].agroRange = 0;
                }
            }

            if(!isComplete)
            {
                audioManager.Play("Win");
            }
            
            animator1.SetBool("Open",true);
            animator2.SetBool("Win",true);
            textContainer.SetActive(false);
            livesUI.SetActive(false);
            swordSprite.SetActive(false);
            fireballsUI.SetActive(false);
            winWindow.SetActive(true);
            buttonUI.SetActive(true);
            isComplete=true;
            CheckLevel();
        }
    }

    void CheckLevel()
    {
        for(int i=0;i<=LevelAmount;i++)
        {
            if(SceneManager.GetActiveScene().name=="Level"+i)
            {
                currentLevel=i;
                SaveLevel();
            }
        }
    }

    void SaveLevel()
    {
        NextLevel=currentLevel+1;
        if(NextLevel <= LevelAmount)
        {
            this.levelsUnlocked[NextLevel-1] = true;
            canSave = true;
        }
        else if(NextLevel > LevelAmount)
        {
            return;
        }   
    }

    public void LoadNextLevel()
    {
        int nextLevel=currentLevel+1;
        if(nextLevel <= LevelAmount)
        {
            sceneController.LoadSceneUsingIndex(SceneManager.GetActiveScene().buildIndex+1);
        }
        else if(nextLevel > LevelAmount)
        {
            sceneController.LoadScene("Main Menu");
        }
    }
}
