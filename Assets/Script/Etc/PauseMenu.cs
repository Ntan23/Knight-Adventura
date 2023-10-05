using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private float startTime;
    public bool isPaused;
    [SerializeField] float duration;
    [SerializeField] GameObject pauseWindow;
    Animator transition;
    private GameObject Bgm;
    GameManager gameManager;
    InteractionSystem interact;
    InventorySystem inventory;
    Goal goal;

    private void Awake() 
    {
        Bgm=GameObject.FindGameObjectWithTag("AudioManager");
        pauseWindow.SetActive(false);
        transition=pauseWindow.GetComponent<Animator>();
        interact=FindObjectOfType<InteractionSystem>();
        inventory=FindObjectOfType<InventorySystem>();
        goal = FindObjectOfType<Goal>();
    }

    private void Start() 
    {
        gameManager=GameManager.instance;
        startTime=Time.time;
    }

    void Update()
    {
        if(Time.time-startTime < duration)
        {
            return;
        }

        if(gameManager.isDead)
        {
            return;
        }

        if(interact.isExamining)
        {
            return;
        }

        if(inventory.isOpen)
        {
            return;
        }

        if(goal.isComplete)
        {
            return;
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {   
            PauseGame();
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale=1.0f;
        pauseWindow.SetActive(false);
        Bgm.GetComponent<AudioSource>().pitch=1.0f;
        Bgm.GetComponent<AudioSource>().volume=0.1f;
    }

    public void PauseGame()
    {
        pauseWindow.SetActive(true);
        Bgm.GetComponent<AudioSource>().pitch=0.8f;
        Bgm.GetComponent<AudioSource>().volume=0.05f;
        isPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
