using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour,IData
{
    public string id;
    
    [ContextMenu("Generate Random Coin ID")]
    private void GenerateGuid() 
    {
        id = System.Guid.NewGuid().ToString();
    }

    AudioManager audioManager;
    CoinTextUpdater coinsText;
    public bool collected=false;

    private void Awake() 
    {
        audioManager=AudioManager.instance;
        coinsText=FindObjectOfType<CoinTextUpdater>();
    }

    public void LoadData(GameData data) 
    {
        data.coinsCollected.TryGetValue(id,out collected);
        if(collected) 
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SaveData(GameData data) 
    {
        if(data.coinsCollected.ContainsKey(id))
        {
            data.coinsCollected.Remove(id);
        }
        data.coinsCollected.Add(id,collected);
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            audioManager.Play("Coin");
            collected=true;
            this.gameObject.SetActive(false);
            coinsText.coinCount += 1;
            coinsText.isCollect = true;
        }
    }
}
