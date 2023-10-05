using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] Animator transition;
    public float transitionTime = 1.0f;
    GameObject Bgm;
    RespawnCounter respawnCounter;
    GameManager gameManager;
    CoinTextUpdater coinText;

    private void Awake() 
    {
        Bgm=GameObject.FindGameObjectWithTag("AudioManager");
        respawnCounter = FindObjectOfType<RespawnCounter>();
        coinText = FindObjectOfType<CoinTextUpdater>();
    }

    private void Start()
    {
        gameManager=GameManager.instance;
    }
    
    public void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1);
    }
    
    public void LoadScene(string sceneName)
    {
        Time.timeScale=1.0f;
        Bgm.GetComponent<AudioSource>().pitch=1.0f;
        Bgm.GetComponent<AudioSource>().volume=0.5f;
        if(respawnCounter != null)
        {
            respawnCounter.count = 0;
            respawnCounter.respawnPrice = 2;
        }
        if(gameManager != null)
        {
            gameManager.canSave = true;
        }
        StartCoroutine(LoadLevel(sceneName));
    }

    public void LoadSceneUsingIndex(int sceneIndex)
    {
        Time.timeScale=1.0f;
        Bgm.GetComponent<AudioSource>().pitch=1.0f;
        Bgm.GetComponent<AudioSource>().volume=0.5f;
        if(respawnCounter != null)
        {
            respawnCounter.count = 0;
            respawnCounter.respawnPrice = 2;
        }
        if(gameManager != null)
        {
            gameManager.canSave = true;
        }
        StartCoroutine(LoadNextLevel(sceneIndex));
    }

    public void GoToMainMenu()
    {
        Time.timeScale=1.0f;
        Bgm.GetComponent<AudioSource>().pitch=1.0f;
        Bgm.GetComponent<AudioSource>().volume=0.5f;
        SceneManager.LoadSceneAsync("Main Menu");
    }

    IEnumerator LoadLevel(string sceneName)
    {
        //Play Scene Transition Animation
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadSceneAsync(sceneName);
    }

    IEnumerator LoadNextLevel(int sceneIndex)
    {
        //Play Scene Transition Animation
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
