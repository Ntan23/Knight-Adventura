using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinTextUpdater : MonoBehaviour
{
    public Text coinText;
    [HideInInspector] public int coinCount;
    int coinsCollectedCount;
    [HideInInspector] public bool isCollect;
    int totalCoins;
    [SerializeField] Coin[] coins;
    GameManager gameManager;
    private void Awake() 
    {
        totalCoins=coins.Length;
        //gameManager = FindObjectOfType<GameManager>();
    }
    private void Start() 
    {
        gameManager = GameManager.instance;
        coinCount = 0;
        for(int i=0;i<totalCoins;i++)
        {
            if(coins[i].collected)
            {
                coinsCollectedCount++;
            }
        }

        gameManager.savedCoinCount = coinsCollectedCount;
        coinText.text=coinsCollectedCount.ToString()+" / "+totalCoins.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.restart)
        {
            coinsCollectedCount = gameManager.savedCoinCount;
            coinCount = 0;
            coinText.text=(coinsCollectedCount).ToString()+" / "+totalCoins.ToString();
            gameManager.restart = false;
        }
        else if(!gameManager.restart)
        {
            if(isCollect)
            {
                coinText.text=(coinsCollectedCount+coinCount).ToString()+" / "+totalCoins.ToString();
                isCollect=false;
            }
            else if(!isCollect)
            {
                return;
            }
        }
    }
}
