using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour,IData
{
    public GameObject LoadingScene;
    bool[] levelsUnlocked = new bool[15];

    [System.Serializable]
    public class Level
    {
        public string LevelText;
        public bool Unlock=false;
        public bool isInteractable=false;
        public Button.ButtonClickedEvent OnClick;
    }

    public GameObject LevelButton;
    public Transform Spacer;
    public Level[] LevelList = new Level[15];
    [SerializeField] Animator sceneAnimator;
    SceneController sceneController;

    private void Awake() 
    {
        sceneController=FindObjectOfType<SceneController>();
    }
    
    // Use this for initialization
    void Start()
    {
        FillList();
	}

    public void SaveData(GameData data)
    {
        
    }

    public void LoadData(GameData data)
    {
        for(int i=0;i<LevelList.Length;i++)
        {
            this.levelsUnlocked[i]=data.levelsUnlocked[i];
        }
    }

	void FillList()
    {
        foreach(var level in LevelList)
        {
            GameObject newButton = Instantiate(LevelButton) as GameObject;
            LevelButton button = newButton.GetComponent<LevelButton>();

            button.LevelText.text = level.LevelText;

            int.TryParse(button.LevelText.text,out int result);
           
            if(levelsUnlocked[result-1])
            {
                level.Unlock=true;
                level.isInteractable=true;
            }

            button.unlocked=level.Unlock;
            button.GetComponent<Button>().interactable=level.isInteractable;
            button.GetComponent<Button>().onClick.AddListener(() => LoadLevel("Level" + button.LevelText.text));
            newButton.transform.SetParent(Spacer);
        }
    }

    void LoadLevel(string value)
    {
        sceneController.LoadScene(value);
    }
}
