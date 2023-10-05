using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool[] levelsUnlocked;
    public float fireballDamage;
    public float swordDamage;
    public int coinCount;
    public bool fireballUnlock;
    public int fireballLevel;
    public int swordLevel;
    public float upgradeFireballDamage;
    public float upgradeSwordDamage;
    public int upgradeSwordPrice;
    public int upgradeFireballPrice;
    public SerializableDictionary<string,bool> coinsCollected;

    public GameData()
    {
        levelsUnlocked=new bool[15];
        levelsUnlocked[0]=true;
        fireballDamage=0.5f;
        swordDamage=0.5f;
        coinCount=0;
        fireballUnlock=false;
        fireballLevel=0;
        swordLevel=1;
        upgradeFireballDamage=1;
        upgradeSwordDamage=1;
        upgradeSwordPrice=15;
        upgradeFireballPrice=20;
        coinsCollected = new SerializableDictionary<string,bool>();
    }
}
