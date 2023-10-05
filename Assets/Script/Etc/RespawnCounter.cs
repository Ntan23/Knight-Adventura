using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnCounter : MonoBehaviour,IData
{
    [HideInInspector] public int count = 0;
    int coinCount;
    [HideInInspector] public int respawnPrice = 2;
    [SerializeField] Text coinText;
    [SerializeField] Text respawnPriceText;
    [SerializeField] Text remainingText;
    [SerializeField] Button yesButton;
    [SerializeField] GameObject price;
    public bool canLoad;
    public bool canSave;
    GameManager gameManager;

    private void Awake()
    {
        canLoad = false;
        canSave = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager=GameManager.instance;
    }
    public void LoadData(GameData data)
    {
        if(canLoad)
        {
            this.coinCount = data.coinCount;
            showData();
            CheckCount(false);
            canLoad = false;
        }
    }

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            data.coinCount = this.coinCount;
            canSave = false;
        }
    }

    public void loadData()
    {
        canLoad = true;
    }

    void showData()
    {
        coinText.text = coinCount.ToString();
        remainingText.text = count.ToString() + " / 2";
        respawnPriceText.text = respawnPrice.ToString();
    }

    public void CheckCount(bool isClick)
    {
        if(count < 2)
        {
            if(coinCount >= respawnPrice && isClick)
            {
                coinCount -= respawnPrice;
                canSave = true;
                respawnPrice+=2;
                gameManager.Restart();
                count++;
            }
            else if(coinCount < respawnPrice)
            {
                yesButton.interactable = false;
            }
        }
        else if(count >= 2)
        {
            price.SetActive(false);
            yesButton.interactable = false;
        }
    }
}
