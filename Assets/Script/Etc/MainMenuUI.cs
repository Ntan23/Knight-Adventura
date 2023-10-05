using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour,IData
{
    [SerializeField] Text coinText;
    int coinCount;
    public bool canSave;
    public bool canLoad;

    private void Awake() 
    {
        canLoad=true;
    }
    // Start is called before the first frame update
    void Start()
    {
        coinText.text=coinCount.ToString();
    }

    public void LoadData(GameData data)
    {
        if(canLoad)
        {
            this.coinCount=data.coinCount;
            canLoad=false;
        }
    }

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            data.coinCount=this.coinCount;
            canSave=false;
            canLoad=true;
        }
    }

    // Update is called once per frame
    public void UpdateCoin(int value)
    {
        coinText.text=value.ToString();
        this.coinCount=value;
        canSave=true;
    }
}
