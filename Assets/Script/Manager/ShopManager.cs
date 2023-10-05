using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour,IData
{
    #region Variables
    [SerializeField] GameObject shopWindow;
    [SerializeField] Text[] levelText;
    [SerializeField] Text[] priceText;
    [SerializeField] Text[] damageText;
    [SerializeField] Text[] nextLevelDamageText;
    int swordUpgradePrice;
    int fireballPrice;
    int upgradeFireballPrice;
    [SerializeField] Button[] fireballButton;
    [SerializeField] Button swordButton;
    [SerializeField] GameObject fireballNotUnlock;
    [SerializeField] GameObject fireballUnlock;
    [SerializeField] GameObject swordUnlock;
    [SerializeField] GameObject swordLevelMax;
    [SerializeField] GameObject fireballLevelMax;
    MainMenuUI coins;

    bool fireballUnlocked;
    int fireballLevel;
    int swordLevel;
    float fireballDamage;
    float swordDamage;
    float upgradeSwordDamage;
    float upgradeFireballDamage;
    int coinCount;
    public bool canSave;
    public bool canLoad;
    int maxLevel;
    string maxText;
    #endregion

    private void Awake() 
    {
        coins=FindObjectOfType<MainMenuUI>();
        fireballPrice=10;
        shopWindow.SetActive(false);
        canLoad=true;
        canSave=false;
        maxLevel=10;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(fireballUnlocked)
        {
            if(fireballLevel<maxLevel)
            {
                fireballNotUnlock.SetActive(false);
                fireballUnlock.SetActive(true);
                fireballLevelMax.SetActive(false);
                levelText[1].text="Lv. "+fireballLevel.ToString()+" / "+maxLevel.ToString();
                priceText[1].text=upgradeFireballPrice.ToString();
                damageText[1].text="Damage : "+fireballDamage.ToString()+" - "+(fireballDamage+0.5f).ToString();
                nextLevelDamageText[1].text="Next Level Damage : "+upgradeFireballDamage.ToString()+" - "+(upgradeFireballDamage+0.5f).ToString();
            }
            else if(fireballLevel==maxLevel)
            {
                fireballNotUnlock.SetActive(false);
                fireballUnlock.SetActive(false);
                fireballLevelMax.SetActive(true);
                damageText[4].text="Damage : "+fireballDamage.ToString()+" - "+(fireballDamage+0.5f).ToString();
            }
        }
        else if(!fireballUnlocked)
        {
            fireballNotUnlock.SetActive(true);
            fireballUnlock.SetActive(false);
            fireballLevelMax.SetActive(false);
            priceText[2].text=fireballPrice.ToString();
            damageText[2].text="Damage : "+fireballDamage.ToString()+" - "+(fireballDamage+0.5f).ToString();
        }
        else
        {
            if(swordLevel < maxLevel)
            {
                swordUnlock.SetActive(true);
                swordLevelMax.SetActive(false);
                levelText[0].text="Lv. "+swordLevel.ToString()+" / "+maxLevel.ToString();
                priceText[0].text=swordUpgradePrice.ToString();
                damageText[0].text="Damage : "+swordDamage.ToString()+" - "+(swordDamage+0.5f).ToString();
                nextLevelDamageText[0].text="Next Level Damage : "+upgradeSwordDamage.ToString()+" - "+(upgradeSwordDamage+0.5f).ToString();
            }
            else if(swordLevel==maxLevel)
            {
                swordUnlock.SetActive(false);
                swordLevelMax.SetActive(true);
                damageText[3].text="Damage : "+swordDamage.ToString()+" - "+(swordDamage+0.5f).ToString();
            }
        }
        buyOrUpgradeFireballCheck();
        upgradeSwordCheck();
    }

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            data.fireballLevel=this.fireballLevel;
            data.fireballUnlock=this.fireballUnlocked;
            data.swordLevel=this.swordLevel;
            data.swordDamage=this.swordDamage;
            data.fireballDamage=this.fireballDamage;
            data.upgradeFireballPrice=this.upgradeFireballPrice;
            data.upgradeFireballDamage=this.upgradeFireballDamage;
            data.upgradeSwordPrice=this.swordUpgradePrice;
            data.upgradeSwordDamage=this.upgradeSwordDamage;
            canSave=false;
            canLoad=true;
        }
    }

    public void LoadData(GameData data)
    {
        if(canLoad)
        {
            this.coinCount=data.coinCount;
            this.fireballUnlocked=data.fireballUnlock;
            this.fireballLevel=data.fireballLevel;
            this.swordLevel=data.swordLevel;
            this.fireballDamage=data.fireballDamage;
            this.swordDamage=data.swordDamage;
            this.upgradeFireballDamage=data.upgradeFireballDamage;
            this.upgradeFireballPrice=data.upgradeFireballPrice;
            this.upgradeSwordDamage=data.upgradeSwordDamage;
            this.swordUpgradePrice=data.upgradeSwordPrice;
            canLoad=false;
            buyOrUpgradeFireballCheck();
            upgradeSwordCheck();
            UpdateSwordUI();
            updateFireballUI();
        }
    }

    void buyOrUpgradeFireballCheck()
    {
        if(!fireballUnlocked)
        {
            if(coinCount>=fireballPrice)
            {
                fireballButton[1].interactable=true;
            }
            else if(this.coinCount<=fireballPrice)
            {
                fireballButton[1].interactable=false;
            }
        }
        else if(fireballUnlocked)
        {
            if(coinCount>=upgradeFireballPrice)
            {
                fireballButton[0].interactable=true;
            }
            else if(coinCount<=upgradeFireballPrice)
            {
                fireballButton[0].interactable=false;
            }
        }
    }

    void upgradeSwordCheck()
    {
        if(coinCount>=swordUpgradePrice)
        {
            swordButton.interactable=true;
        }
        else if(coinCount<=swordUpgradePrice)
        {
            swordButton.interactable=false;
        }
    }

    public void buyorUpgradeFireball()
    {
        if(!fireballUnlocked)
        {
            this.coinCount-=fireballPrice;
            this.fireballLevel+=1;
            this.fireballUnlocked=true;
            coins.UpdateCoin(this.coinCount);
            canSave=true;
        }
        else if(fireballUnlocked)
        {
            if(this.fireballLevel < maxLevel)
            {
                this.coinCount-=this.upgradeFireballPrice;
                coins.UpdateCoin(this.coinCount);
                this.fireballLevel+=1;
                this.fireballDamage=this.upgradeFireballDamage;
                this.upgradeFireballDamage+=0.5f;
                this.upgradeFireballPrice+=10;
                canSave=true;
            }
        }
    }

    void updateFireballUI()
    {
        if(fireballUnlocked)
        {
            if(this.fireballLevel < maxLevel)
            {
                fireballNotUnlock.SetActive(false);
                fireballUnlock.SetActive(true);
                fireballLevelMax.SetActive(false);
                priceText[1].text=upgradeFireballPrice.ToString();
                levelText[1].text="Lv. "+fireballLevel.ToString()+" / "+maxLevel.ToString();
                damageText[1].text="Damage : "+fireballDamage.ToString()+" - "+(fireballDamage+0.5f).ToString();
                nextLevelDamageText[1].text="Next Level Damage : "+upgradeFireballDamage.ToString()+" - "+(upgradeFireballDamage+0.5f).ToString();
            }
            else if(this.fireballLevel==maxLevel)
            {
                fireballNotUnlock.SetActive(false);
                fireballUnlock.SetActive(false);
                fireballLevelMax.SetActive(true);
                damageText[4].text="Damage : "+fireballDamage.ToString()+" - "+(fireballDamage+0.5f).ToString();
            }
        }
    }
    public void upgradeSword()
    {
        if(this.swordLevel < maxLevel)
        {
            this.coinCount-=this.swordUpgradePrice;
            coins.UpdateCoin(this.coinCount);
            this.swordLevel+=1;
            this.swordDamage=this.upgradeSwordDamage;
            this.upgradeSwordDamage+=0.5f;
            this.swordUpgradePrice+=10;
            canSave=true;
        }
    }

    void UpdateSwordUI()
    {
        if(this.swordLevel < maxLevel)
        {
            swordUnlock.SetActive(true);
            swordLevelMax.SetActive(false);
            priceText[0].text=swordUpgradePrice.ToString();
            levelText[0].text="Lv. "+swordLevel.ToString()+" / "+maxLevel.ToString();
            damageText[0].text="Damage : "+swordDamage.ToString()+" - "+(swordDamage+0.5f).ToString();
            nextLevelDamageText[0].text="Next Level Damage : "+upgradeSwordDamage.ToString()+" - "+(upgradeSwordDamage+0.5f).ToString();
        }
        else if(this.swordLevel==maxLevel)
        {
            swordUnlock.SetActive(false);
            swordLevelMax.SetActive(true);
            damageText[3].text="Damage : "+swordDamage.ToString()+" - "+(swordDamage+0.5f).ToString();
        }
    }
}
